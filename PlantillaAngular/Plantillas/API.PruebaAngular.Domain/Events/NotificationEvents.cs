using System;

namespace PruebaAngular.Domain.Events
{
    public class NotificationSubscribedEvent : DomainEvent
    {
        // Use a stable routing key for RabbitMQ consumers
        public override string EventType => "notification.subscribed";
        public Guid SubscriberId { get; init; }
        public string Email { get; init; } = string.Empty;
    }
}
