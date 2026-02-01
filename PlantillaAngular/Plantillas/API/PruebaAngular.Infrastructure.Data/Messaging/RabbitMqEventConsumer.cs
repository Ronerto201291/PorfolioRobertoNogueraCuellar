using System;
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
        private const string QueueName = "activity.events";

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

                await _channel.QueueDeclareAsync(
                    queue: QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    cancellationToken: stoppingToken);

                await _channel.QueueBindAsync(
                    queue: QueueName,
                    exchange: ExchangeName,
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

                _logger.LogInformation("[RabbitMQ] Consumer escuchando en cola: {Queue}", QueueName);

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
                var messageId = args.BasicProperties.MessageId ?? "unknown";

                using var doc = JsonDocument.Parse(message);
                var eventType = doc.RootElement.TryGetProperty("eventType", out var eventTypeProp)
                    ? eventTypeProp.GetString()
                    : routingKey;
                var entityName = doc.RootElement.TryGetProperty("entityName", out var entityNameProp)
                    ? entityNameProp.GetString()
                    : null;

                _logger.LogInformation(
                    "[Activity] {EventType} - {EntityName}",
                    eventType,
                    string.IsNullOrWhiteSpace(entityName) ? "(sin nombre)" : entityName);

                if (_channel != null)
                {
                    await _channel.BasicAckAsync(args.DeliveryTag, multiple: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[RabbitMQ] Error processing message: {Error}",
                    ex.Message);

                if (_channel != null)
                {
                    await _channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: true);
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
    }
}
