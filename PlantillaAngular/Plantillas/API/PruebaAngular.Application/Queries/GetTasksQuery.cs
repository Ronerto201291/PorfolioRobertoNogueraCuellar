using System.Collections.Generic;
using MediatR;
using PruebaAngular.Domain.AggregateModels.Portfolio;

namespace PruebaAngular.Application.Queries
{
    public class GetTasksQuery : IRequest<IReadOnlyList<PortfolioTask>>
    {
    }
}
