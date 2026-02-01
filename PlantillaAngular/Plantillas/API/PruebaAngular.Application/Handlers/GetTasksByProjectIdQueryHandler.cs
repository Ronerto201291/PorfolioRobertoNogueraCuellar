using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PruebaAngular.Application.Queries;
using PruebaAngular.Domain.AggregateModels.Portfolio;
using PruebaAngular.Infrastructure.Data;

namespace PruebaAngular.Application.Handlers
{
    public class GetTasksByProjectIdQueryHandler : IRequestHandler<GetTasksByProjectIdQuery, IReadOnlyList<PortfolioTask>>
    {
        private readonly PruebaAngularContext _context;

        public GetTasksByProjectIdQueryHandler(PruebaAngularContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<PortfolioTask>> Handle(GetTasksByProjectIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Tasks
                .AsNoTracking()
                .Where(t => t.ProjectId == request.ProjectId)
                .ToListAsync(cancellationToken);
        }
    }
}
