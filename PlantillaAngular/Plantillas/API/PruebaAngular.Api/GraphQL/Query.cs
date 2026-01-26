using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;
using PruebaAngular.Domain.AggregateModels.Portfolio;
using PruebaAngular.Infrastructure.Data;

namespace PruebaAngular.Api.GraphQL
{
    /// <summary>
    /// GraphQL Query resolver for Portfolio data
    /// </summary>
    public class Query
    {
        /// <summary>
        /// Get all projects with their tasks
        /// </summary>
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Project> GetProjects([Service] PruebaAngularContext context)
        {
            return context.Projects.Include(p => p.Tasks);
        }

        /// <summary>
        /// Get a specific project by ID
        /// </summary>
        public async Task<Project?> GetProjectById(
            [Service] PruebaAngularContext context,
            Guid id)
        {
            return await context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.ProjectId == id);
        }

        /// <summary>
        /// Get all tasks
        /// </summary>
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<PortfolioTask> GetTasks([Service] PruebaAngularContext context)
        {
            return context.Tasks.Include(t => t.Project);
        }

        /// <summary>
        /// Get tasks by project ID
        /// </summary>
        public async Task<List<PortfolioTask>> GetTasksByProjectId(
            [Service] PruebaAngularContext context,
            Guid projectId)
        {
            return await context.Tasks
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();
        }

        /// <summary>
        /// Get tasks by status
        /// </summary>
        public async Task<List<PortfolioTask>> GetTasksByStatus(
            [Service] PruebaAngularContext context,
            string status)
        {
            return await context.Tasks
                .Where(t => t.Status == status)
                .Include(t => t.Project)
                .ToListAsync();
        }
    }
}
