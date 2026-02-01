using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PruebaAngular.Application.Queries;
using PruebaAngular.Domain.AggregateModels.Portfolio;
using PruebaAngular.Infrastructure.Data;

namespace PruebaAngular.Application.Handlers
{
    public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, Project?>
    {
        private readonly PruebaAngularContext _context;

        public GetProjectByIdQueryHandler(PruebaAngularContext context)
        {
            _context = context;
        }

        public async Task<Project?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Projects
                .AsNoTracking()
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.ProjectId == request.ProjectId, cancellationToken);
        }
    }
}
