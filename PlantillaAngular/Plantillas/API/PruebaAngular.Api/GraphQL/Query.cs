using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate;
using MediatR;
using PruebaAngular.Application.Queries;
using PruebaAngular.Domain.AggregateModels.Portfolio;

namespace PruebaAngular.Api.GraphQL
{
    public class Query
    {
        public async Task<IReadOnlyList<Project>> GetProjects([Service] IMediator mediator)
        {
            return await mediator.Send(new GetProjectsQuery());
        }

        public async Task<Project?> GetProjectById([Service] IMediator mediator, Guid id)
        {
            return await mediator.Send(new GetProjectByIdQuery { ProjectId = id });
        }

        public async Task<IReadOnlyList<PortfolioTask>> GetTasks([Service] IMediator mediator)
        {
            return await mediator.Send(new GetTasksQuery());
        }

        public async Task<IReadOnlyList<PortfolioTask>> GetTasksByProjectId([Service] IMediator mediator, Guid projectId)
        {
            return await mediator.Send(new GetTasksByProjectIdQuery { ProjectId = projectId });
        }

        public async Task<IReadOnlyList<PortfolioTask>> GetTasksByStatus([Service] IMediator mediator, string status)
        {
            return await mediator.Send(new GetTasksByStatusQuery { Status = status });
        }
    }
}
