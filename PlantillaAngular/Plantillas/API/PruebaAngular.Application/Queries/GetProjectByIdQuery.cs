using System;
using MediatR;
using PruebaAngular.Domain.AggregateModels.Portfolio;

namespace PruebaAngular.Application.Queries
{
    public class GetProjectByIdQuery : IRequest<Project?>
    {
        public Guid ProjectId { get; init; }
    }
}
