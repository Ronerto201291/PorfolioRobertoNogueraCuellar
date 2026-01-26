using System;
using System.Collections.Generic;

namespace PruebaAngular.Domain.AggregateModels.Portfolio
{
    /// <summary>
    /// Project aggregate root - represents a portfolio project
    /// </summary>
    public class Project
    {
        public Project()
        {
            Tasks = new HashSet<PortfolioTask>();
        }

        public Guid ProjectId { get; set; }

        /// <summary>
        /// Project name - max 200 characters
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Project description - optional, max 1000 characters
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Project creation timestamp
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Navigation property for tasks
        /// </summary>
        public virtual ICollection<PortfolioTask> Tasks { get; set; }

        /// <summary>
        /// Factory method to create a new project
        /// </summary>
        public static Project Create(string name, string? description = null)
        {
            return new Project
            {
                ProjectId = Guid.NewGuid(),
                Name = name,
                Description = description,
                CreatedAt = DateTimeOffset.UtcNow
            };
        }
    }
}
