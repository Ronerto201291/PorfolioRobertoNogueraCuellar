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
    public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, IReadOnlyList<Project>>
    {
        private readonly PruebaAngularContext _context;

        public GetProjectsQueryHandler(PruebaAngularContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Project>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Projects
                .AsNoTracking()
                .Include(p => p.Tasks)
                .ToListAsync(cancellationToken);
        }
    }
}
