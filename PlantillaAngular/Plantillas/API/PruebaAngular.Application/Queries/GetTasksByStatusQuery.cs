using System.Collections.Generic;
using MediatR;
using PruebaAngular.Domain.AggregateModels.Portfolio;

namespace PruebaAngular.Application.Queries
{
    public class GetTasksByStatusQuery : IRequest<IReadOnlyList<PortfolioTask>>
    {
        public string Status { get; init; } = string.Empty;
    }
}
