using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PruebaAngular.Domain.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PruebaAngular.Infrastructure.Messaging
{
    public class RabbitMqEventConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly ILogger<RabbitMqEventConsumer> _logger;
        private IChannel? _channel;
        private const string ExchangeName = "activity.events";
        private const string RetryExchangeName = "activity.events.retry";
        private const string DeadLetterExchangeName = "activity.events.dlq";
        private const string QueueName = "activity.events";
        private const string RetryQueueName = "activity.events.retry";
        private const string DeadLetterQueueName = "activity.events.dlq";
        private const int MaxRetries = 3;
        private const int RetryDelayMilliseconds = 5000;

        public RabbitMqEventConsumer(
            IConnection connection,
            ILogger<RabbitMqEventConsumer> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[RabbitMQ] Consumer iniciando...");

            try
            {
                _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

                await _channel.ExchangeDeclareAsync(
                    exchange: ExchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false,
                    cancellationToken: stoppingToken);

                await _channel.ExchangeDeclareAsync(
                    exchange: RetryExchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false,
                    cancellationToken: stoppingToken);

                await _channel.ExchangeDeclareAsync(
                    exchange: DeadLetterExchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false,
                    cancellationToken: stoppingToken);

                DeclareQueueWithReset(
                    QueueName,
                    new Dictionary<string, object>
                    {
                        ["x-dead-letter-exchange"] = RetryExchangeName
                    },
                    stoppingToken);

                DeclareQueueWithReset(
                    RetryQueueName,
                    new Dictionary<string, object>
                    {
                        ["x-message-ttl"] = RetryDelayMilliseconds,
                        ["x-dead-letter-exchange"] = ExchangeName
                    },
                    stoppingToken);

                DeclareQueueWithReset(DeadLetterQueueName, null, stoppingToken);

                await _channel.QueueBindAsync(
                    queue: QueueName,
                    exchange: ExchangeName,
                    routingKey: "#",
                    cancellationToken: stoppingToken);

                await _channel.QueueBindAsync(
                    queue: RetryQueueName,
                    exchange: RetryExchangeName,
                    routingKey: "#",
                    cancellationToken: stoppingToken);

                await _channel.QueueBindAsync(
                    queue: DeadLetterQueueName,
                    exchange: DeadLetterExchangeName,
                    routingKey: "#",
                    cancellationToken: stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (sender, args) =>
                {
                    await ProcessMessageAsync(args);
                };

                await _channel.BasicConsumeAsync(
                    queue: QueueName,
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: stoppingToken);

                _logger.LogInformation("[Consumer] Listening Queue={Queue}", QueueName);


                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("[RabbitMQ] Consumer detenido.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RabbitMQ] Error en consumer: {Error}", ex.Message);
            }
        }

        private async Task ProcessMessageAsync(BasicDeliverEventArgs args)
        {
            try
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = args.RoutingKey;
                var correlationId = args.BasicProperties.CorrelationId
                    ?? (args.BasicProperties.Headers != null && args.BasicProperties.Headers.TryGetValue("correlation-id", out var headerValue)
                        ? Encoding.UTF8.GetString((byte[])headerValue)
                        : "unknown");

                using var doc = JsonDocument.Parse(message);
                var eventType = doc.RootElement.TryGetProperty("eventType", out var eventTypeProp)
                    ? eventTypeProp.GetString()
                    : routingKey;

                _logger.LogInformation(
                    "[Consumer] Event={EventType} CorrelationId={CorrelationId} Payload={Payload}",
                    eventType,
                    correlationId,
                    message);

                if (_channel != null)
                {
                    await _channel.BasicAckAsync(args.DeliveryTag, multiple: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[Consumer] Error processing message: {Error}",
                    ex.Message);

                if (_channel != null)
                {
                    var retryCount = GetRetryCount(args.BasicProperties);
                    var correlationId = args.BasicProperties.CorrelationId
                        ?? (args.BasicProperties.Headers != null && args.BasicProperties.Headers.TryGetValue("correlation-id", out var headerValue)
                            ? Encoding.UTF8.GetString((byte[])headerValue)
                            : "unknown");

                    if (retryCount >= MaxRetries)
                    {
                        var deadLetterProperties = ClonePropertiesWithRetry(args.BasicProperties, retryCount);
                        await _channel.BasicPublishAsync(
                            exchange: DeadLetterExchangeName,
                            routingKey: args.RoutingKey,
                            mandatory: false,
                            basicProperties: deadLetterProperties,
                            body: args.Body);

                        await _channel.BasicAckAsync(args.DeliveryTag, multiple: false);

                        _logger.LogWarning(
                            "[Consumer] Sent to DLQ after retries Event={EventType} CorrelationId={CorrelationId}",
                            args.RoutingKey,
                            correlationId);
                        return;
                    }

                    var nextProperties = ClonePropertiesWithRetry(args.BasicProperties, retryCount + 1);
                    await _channel.BasicPublishAsync(
                        exchange: RetryExchangeName,
                        routingKey: args.RoutingKey,
                        mandatory: false,
                        basicProperties: nextProperties,
                        body: args.Body);

                    await _channel.BasicAckAsync(args.DeliveryTag, multiple: false);

                    _logger.LogWarning(
                        "[Consumer] Retry scheduled Event={EventType} CorrelationId={CorrelationId} Retry={Retry}",
                        args.RoutingKey,
                        correlationId,
                        retryCount + 1);
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[RabbitMQ] Consumer deteniendo...");
            
            if (_channel != null)
            {
                await _channel.CloseAsync(cancellationToken);
            }
            
            await base.StopAsync(cancellationToken);
        }

        private static int GetRetryCount(IReadOnlyBasicProperties properties)
        {
            if (properties.Headers != null && properties.Headers.TryGetValue("x-retry-count", out var value))
            {
                return int.TryParse(GetHeaderValue(value), out var retryCount)
                    ? retryCount
                    : 0;
            }

            return 0;
        }

        private static BasicProperties ClonePropertiesWithRetry(IReadOnlyBasicProperties properties, int retryCount)
        {
            var clone = new BasicProperties
            {
                ContentType = properties.ContentType,
                DeliveryMode = properties.DeliveryMode,
                MessageId = properties.MessageId,
                CorrelationId = properties.CorrelationId,
                Timestamp = properties.Timestamp
            };

            clone.Headers = properties.Headers != null
                ? new Dictionary<string, object>(properties.Headers)
                : new Dictionary<string, object>();

            clone.Headers["x-retry-count"] = retryCount.ToString();

            return clone;
        }

        private void DeclareQueueWithReset(string queueName, IDictionary<string, object>? arguments, CancellationToken cancellationToken)
        {
            try
            {
                _channel?.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: arguments,
                    cancellationToken: cancellationToken).GetAwaiter().GetResult();
            }
            catch (RabbitMQ.Client.Exceptions.OperationInterruptedException)
            {
                ResetChannel();
                TryDeleteQueue(queueName, cancellationToken);
                _channel?.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: arguments,
                    cancellationToken: cancellationToken).GetAwaiter().GetResult();
            }
        }

        private void TryDeleteQueue(string queueName, CancellationToken cancellationToken)
        {
            try
            {
                _channel?.QueueDeleteAsync(queue: queueName, ifUnused: false, ifEmpty: false, cancellationToken: cancellationToken).GetAwaiter().GetResult();
            }
            catch (RabbitMQ.Client.Exceptions.OperationInterruptedException)
            {
                ResetChannel();
            }
        }

        private void ResetChannel()
        {
            if (_channel is { IsOpen: true })
            {
                return;
            }

            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        private static string GetHeaderValue(object value)
        {
            return value switch
            {
                byte[] bytes => Encoding.UTF8.GetString(bytes),
                ReadOnlyMemory<byte> memory => Encoding.UTF8.GetString(memory.Span),
                string text => text,
                _ => value?.ToString() ?? string.Empty
            };
        }
    }
}
