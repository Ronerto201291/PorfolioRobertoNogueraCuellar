using System;
using System.Collections.Generic;
using MediatR;
using PruebaAngular.Domain.AggregateModels.Portfolio;

namespace PruebaAngular.Application.Queries
{
    public class GetTasksByProjectIdQuery : IRequest<IReadOnlyList<PortfolioTask>>
    {
        public Guid ProjectId { get; init; }
    }
}
