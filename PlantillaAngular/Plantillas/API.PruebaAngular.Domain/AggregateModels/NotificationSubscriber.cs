using System;

namespace PruebaAngular.Domain.AggregateModels
{
    public class NotificationSubscriber
    {
        public Guid SubscriberId { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
