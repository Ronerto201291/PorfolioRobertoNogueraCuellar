using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PruebaAngular.Domain.Events;
using RabbitMQ.Client;

namespace PruebaAngular.Infrastructure.Messaging
{
    /// <summary>
    /// Implementación del Event Bus usando RabbitMQ.
    /// Incluye logs estructurados para visibilidad en Aspire.
    /// </summary>
    public class RabbitMqEventBus : IEventBus, IDisposable
    {
        private readonly IConnection _connection;
        private IChannel _channel;
        private readonly ILogger<RabbitMqEventBus> _logger;
        private readonly ConcurrentQueue<EventActivity> _activityLog;
        private const string ExchangeName = "activity.events";
        private const string RetryExchangeName = "activity.events.retry";
        private const string DeadLetterExchangeName = "activity.events.dlq";
        private const string QueueName = "activity.events";
        private const string RetryQueueName = "activity.events.retry";
        private const string DeadLetterQueueName = "activity.events.dlq";
        private const int RetryDelayMilliseconds = 5000;
        private const int MaxActivityLogSize = 100;

        public RabbitMqEventBus(
            IConnection connection,
            ILogger<RabbitMqEventBus> logger)
        {
            _connection = connection;
            _channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
            _logger = logger;
            _activityLog = new ConcurrentQueue<EventActivity>();

            DeclareTopology();

            _logger.LogInformation("[RabbitMQ] EventBus inicializado. Exchange: {Exchange}", ExchangeName);
        }

        public async Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
            where TEvent : IDomainEvent
        {
            try
            {
                var routingKey = domainEvent.EventType.ToLowerInvariant();
                var correlationId = domainEvent.EventId.ToString();
                var messageBody = JsonSerializer.Serialize(domainEvent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                var body = Encoding.UTF8.GetBytes(messageBody);

                var properties = new BasicProperties
                {
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent,
                    MessageId = domainEvent.EventId.ToString(),
                    CorrelationId = correlationId,
                    Timestamp = new AmqpTimestamp(domainEvent.OccurredAt.ToUnixTimeSeconds())
                };

                properties.Headers ??= new Dictionary<string, object>();
                properties.Headers["correlation-id"] = correlationId;

                await _channel.BasicPublishAsync(
                    exchange: ExchangeName,
                    routingKey: routingKey,
                    mandatory: false,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "[PortfolioApi] Published {EventType} CorrelationId={CorrelationId} Exchange={Exchange} RoutingKey={RoutingKey}",
                    domainEvent.EventType,
                    correlationId,
                    ExchangeName,
                    routingKey);

                RecordActivity(new EventActivity
                {
                    EventId = domainEvent.EventId,
                    EventType = domainEvent.EventType,
                    Timestamp = DateTimeOffset.UtcNow,
                    Status = EventActivityStatus.Published,
                    Details = $"Exchange={ExchangeName}; RoutingKey={routingKey}; CorrelationId={correlationId}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[RabbitMQ] Failed to publish {EventType} (EventId={EventId}): {Error}",
                    domainEvent.EventType,
                    domainEvent.EventId,
                    ex.Message);

                RecordActivity(new EventActivity
                {
                    EventId = domainEvent.EventId,
                    EventType = domainEvent.EventType,
                    Timestamp = DateTimeOffset.UtcNow,
                    Status = EventActivityStatus.Failed,
                    Details = ex.Message
                });

                throw;
            }
        }

        public IReadOnlyList<EventActivity> GetRecentActivity(int count = 50)
        {
            return _activityLog.TakeLast(count).Reverse().ToList();
        }

        private void RecordActivity(EventActivity activity)
        {
            _activityLog.Enqueue(activity);

            // Limitar tamaño del log
            while (_activityLog.Count > MaxActivityLogSize)
            {
                _activityLog.TryDequeue(out _);
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }

        private void DeclareTopology()
        {
            _channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false).GetAwaiter().GetResult();

            _channel.ExchangeDeclareAsync(
                exchange: RetryExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false).GetAwaiter().GetResult();

            _channel.ExchangeDeclareAsync(
                exchange: DeadLetterExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false).GetAwaiter().GetResult();

            DeclareQueueWithReset(
                QueueName,
                new Dictionary<string, object>
                {
                    ["x-dead-letter-exchange"] = RetryExchangeName
                });

            DeclareQueueWithReset(
                RetryQueueName,
                new Dictionary<string, object>
                {
                    ["x-message-ttl"] = RetryDelayMilliseconds,
                    ["x-dead-letter-exchange"] = ExchangeName
                });

            DeclareQueueWithReset(DeadLetterQueueName, null);

            _channel.QueueBindAsync(
                queue: QueueName,
                exchange: ExchangeName,
                routingKey: "#").GetAwaiter().GetResult();

            _channel.QueueBindAsync(
                queue: RetryQueueName,
                exchange: RetryExchangeName,
                routingKey: "#").GetAwaiter().GetResult();

            _channel.QueueBindAsync(
                queue: DeadLetterQueueName,
                exchange: DeadLetterExchangeName,
                routingKey: "#").GetAwaiter().GetResult();
        }

        private void DeclareQueueWithReset(string queueName, IDictionary<string, object>? arguments)
        {
            try
            {
                _channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: arguments).GetAwaiter().GetResult();
            }
            catch (RabbitMQ.Client.Exceptions.OperationInterruptedException)
            {
                ResetChannel();
                TryDeleteQueue(queueName);
                _channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: arguments).GetAwaiter().GetResult();
            }
        }

        private void TryDeleteQueue(string queueName)
        {
            try
            {
                _channel.QueueDeleteAsync(queue: queueName, ifUnused: false, ifEmpty: false).GetAwaiter().GetResult();
            }
            catch (RabbitMQ.Client.Exceptions.OperationInterruptedException)
            {
                ResetChannel();
            }
        }

        private void ResetChannel()
        {
            if (_channel.IsOpen)
            {
                return;
            }

            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }
    }
}
