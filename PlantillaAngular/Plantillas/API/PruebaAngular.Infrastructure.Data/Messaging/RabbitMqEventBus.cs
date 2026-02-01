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
        private readonly IChannel _channel;
        private readonly ILogger<RabbitMqEventBus> _logger;
        private readonly ConcurrentQueue<EventActivity> _activityLog;
        private const string ExchangeName = "activity.events";
        private const int MaxActivityLogSize = 100;

        public RabbitMqEventBus(
            IConnection connection,
            ILogger<RabbitMqEventBus> logger)
        {
            _connection = connection;
            _channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
            _logger = logger;
            _activityLog = new ConcurrentQueue<EventActivity>();

            _channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false).GetAwaiter().GetResult();

            _logger.LogInformation("[RabbitMQ] EventBus inicializado. Exchange: {Exchange}", ExchangeName);
        }

        public async Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
            where TEvent : IDomainEvent
        {
            try
            {
                var routingKey = domainEvent.EventType.ToLowerInvariant();
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
                    Timestamp = new AmqpTimestamp(domainEvent.OccurredAt.ToUnixTimeSeconds())
                };

                await _channel.BasicPublishAsync(
                    exchange: ExchangeName,
                    routingKey: routingKey,
                    mandatory: false,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "[RabbitMQ] Published {EventType} (EventId={EventId})",
                    domainEvent.EventType,
                    domainEvent.EventId);

                RecordActivity(new EventActivity
                {
                    EventId = domainEvent.EventId,
                    EventType = domainEvent.EventType,
                    Timestamp = DateTimeOffset.UtcNow,
                    Status = EventActivityStatus.Published,
                    Details = $"RoutingKey: {routingKey}"
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
    }
}
