using System;
using System.Threading.Tasks;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using PruebaAngular.Domain.AggregateModels.Portfolio;
using PruebaAngular.Infrastructure.Data;

namespace PruebaAngular.Api.GraphQL
{
    /// <summary>
    /// GraphQL Mutation resolver for Portfolio data
    /// </summary>
    public class Mutation
    {

        /// <summary>
        /// Create a new project
        /// </summary>
        public async Task<Project> CreateProject(
            [Service] PruebaAngularContext context,
            string name,
            string? description)
        {
            var project = Project.Create(name, description);
            context.Projects.Add(project);
            await context.SaveChangesAsync();
            return project;
        }

        /// <summary>
        /// Update an existing project
        /// </summary>
        public async Task<Project?> UpdateProject(
            [Service] PruebaAngularContext context,
            Guid id,
            string name,
            string? description)
        {
            var project = await context.Projects.FindAsync(id);
            if (project == null) return null;

            project.Name = name;
            project.Description = description;
            await context.SaveChangesAsync();
            return project;
        }

        /// <summary>
        /// Delete a project and all its tasks
        /// </summary>
        public async Task<bool> DeleteProject(
            [Service] PruebaAngularContext context,
            Guid id)
        {
            var project = await context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.ProjectId == id);
            
            if (project == null) return false;

            context.Projects.Remove(project);
            await context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Create a new task
        /// </summary>
        public async Task<PortfolioTask> CreateTask(
            [Service] PruebaAngularContext context,
            Guid projectId,
            string title,
            string status,
            string priority,
            string? description,
            DateTimeOffset? dueDate)
        {
            var task = PortfolioTask.Create(projectId, title, status, priority, description, dueDate);
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            return task;
        }

        /// <summary>
        /// Update an existing task
        /// </summary>
        public async Task<PortfolioTask?> UpdateTask(
            [Service] PruebaAngularContext context,
            Guid id,
            string title,
            string status,
            string priority,
            string? description,
            DateTimeOffset? dueDate)
        {
            var task = await context.Tasks.FirstOrDefaultAsync(t => t.TaskId == id);
            if (task == null) return null;

            task.Title = title;
            task.Status = status;
            task.Priority = priority;
            task.Description = description;
            task.DueDate = dueDate;
            await context.SaveChangesAsync();
            return task;
        }

        /// <summary>
        /// Update task status
        /// </summary>
        public async Task<PortfolioTask?> UpdateTaskStatus(
            [Service] PruebaAngularContext context,
            Guid id,
            string status)
        {
            var task = await context.Tasks.FirstOrDefaultAsync(t => t.TaskId == id);
            if (task == null) return null;

            task.Status = status;
            await context.SaveChangesAsync();
            return task;
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        public async Task<bool> DeleteTask(
            [Service] PruebaAngularContext context,
            Guid id)
        {
            var task = await context.Tasks.FirstOrDefaultAsync(t => t.TaskId == id);
            if (task == null) return false;

            context.Tasks.Remove(task);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
