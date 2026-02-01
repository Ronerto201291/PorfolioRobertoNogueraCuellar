using System;
using System.ComponentModel.DataAnnotations;

namespace PruebaAngular.Domain.AggregateModels
{
    public class NotificationSubscriber
    {
        [Key]
        public Guid SubscriberId { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = string.Empty;
    }
}
