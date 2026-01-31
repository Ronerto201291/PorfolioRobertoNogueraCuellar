using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PruebaAngular.Application.Commands;
using PruebaAngular.Infrastructure.Data;
using PruebaAngular.Domain.AggregateModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PruebaAngular.Infrastructure.Messaging;
using PruebaAngular.Domain.Events;

namespace PruebaAngular.Application.Handlers
{
    public class SubscribeNotificationCommandHandler : IRequestHandler<SubscribeNotificationCommand, SubscribeNotificationResult>
    {
        private readonly PruebaAngularContext _db;
        private readonly IEventBus _eventBus;

        public SubscribeNotificationCommandHandler(PruebaAngularContext db, IEventBus eventBus)
        {
            _db = db;
            _eventBus = eventBus;
        }

        public async Task<SubscribeNotificationResult> Handle(SubscribeNotificationCommand request, CancellationToken cancellationToken)
        {
            var email = request.Email?.Trim();
            if (string.IsNullOrWhiteSpace(email))
                return new SubscribeNotificationResult { Success = false, Message = "Email required" };

            var exists = await _db.Set<NotificationSubscriber>().AnyAsync(s => s.Email == email, cancellationToken);
            if (exists)
                return new SubscribeNotificationResult { Success = false, Message = "Email already subscribed" };

            var subscriber = new NotificationSubscriber { Email = email };
            _db.Add(subscriber);
            await _db.SaveChangesAsync(cancellationToken);

            // Publish domain event for subscribers
            var ev = new NotificationSubscribedEvent { SubscriberId = subscriber.SubscriberId, Email = subscriber.Email };
            await _eventBus.PublishAsync(ev, cancellationToken);

            return new SubscribeNotificationResult { Success = true };
        }
    }
}
