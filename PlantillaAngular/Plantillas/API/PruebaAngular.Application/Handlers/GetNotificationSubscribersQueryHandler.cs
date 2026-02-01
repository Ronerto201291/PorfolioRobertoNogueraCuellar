using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PruebaAngular.Application.Queries;
using PruebaAngular.Domain.AggregateModels;
using PruebaAngular.Infrastructure.Data;

namespace PruebaAngular.Application.Handlers
{
    public class GetNotificationSubscribersQueryHandler : IRequestHandler<GetNotificationSubscribersQuery, IReadOnlyList<string>>
    {
        private readonly PruebaAngularContext _db;

        public GetNotificationSubscribersQueryHandler(PruebaAngularContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<string>> Handle(GetNotificationSubscribersQuery request, CancellationToken cancellationToken)
        {
            return await _db.Set<NotificationSubscriber>()
                .Select(s => s.Email)
                .ToListAsync(cancellationToken);
        }
    }
}
