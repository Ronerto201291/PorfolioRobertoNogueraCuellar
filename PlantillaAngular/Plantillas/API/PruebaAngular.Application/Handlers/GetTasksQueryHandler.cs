using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PruebaAngular.Application.Queries;
using PruebaAngular.Domain.AggregateModels.Portfolio;
using PruebaAngular.Infrastructure.Data;

namespace PruebaAngular.Application.Handlers
{
    public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, IReadOnlyList<PortfolioTask>>
    {
        private readonly PruebaAngularContext _context;

        public GetTasksQueryHandler(PruebaAngularContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<PortfolioTask>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
        {
            return await _context.Tasks
                .AsNoTracking()
                .Include(t => t.Project)
                .ToListAsync(cancellationToken);
        }
    }
}
