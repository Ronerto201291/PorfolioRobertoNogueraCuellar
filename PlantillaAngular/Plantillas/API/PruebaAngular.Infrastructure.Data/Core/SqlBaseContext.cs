using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using PruebaAngular.Domain.Core;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;

namespace PruebaAngular.Infrastructure.Data.Core
{
    public interface ICurrentSessionProvider
    {
        string GetCurrentUser();
    }

    public class CurrentApiUserSessionProvider : ICurrentSessionProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentApiUserSessionProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                return user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? user.FindFirst(ClaimTypes.Name)?.Value 
                    ?? "Unknown";
            }
            return "Anonymous";
        }
    }

    public abstract class SqlBaseContext : DbContext
    {
        private readonly IMediator _mediator;
        private readonly ICurrentSessionProvider _currentSessionProvider;

        protected SqlBaseContext(DbContextOptions options, IMediator mediator, ICurrentSessionProvider currentSessionProvider) : base(options)
        {
            _mediator = mediator;
            _currentSessionProvider = currentSessionProvider;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Dispatch domain events before saving
            if (_mediator != null)
            {
                await DispatchDomainEventsAsync();
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            // Dispatch domain events before saving
            if (_mediator != null)
            {
                DispatchDomainEventsAsync().GetAwaiter().GetResult();
            }

            return base.SaveChanges();
        }

        private async Task DispatchDomainEventsAsync()
        {
            var domainEntities = ChangeTracker
                .Entries<IAggregateRoot>()
                .Where(x => x.Entity.GetType().GetProperty("DomainEvents") != null)
                .ToList();

            foreach (var entry in domainEntities)
            {
                var entity = entry.Entity;
                var domainEventsProperty = entity.GetType().GetProperty("DomainEvents");
                if (domainEventsProperty != null)
                {
                    var domainEvents = domainEventsProperty.GetValue(entity) as System.Collections.IEnumerable;
                    if (domainEvents != null)
                    {
                        foreach (var domainEvent in domainEvents)
                        {
                            if (domainEvent is INotification notification)
                            {
                                await _mediator.Publish(notification);
                            }
                        }
                        
                        // Clear domain events
                        var clearMethod = domainEventsProperty.PropertyType.GetMethod("Clear");
                        clearMethod?.Invoke(domainEventsProperty.GetValue(entity), null);
                    }
                }
            }
        }
    }
}
