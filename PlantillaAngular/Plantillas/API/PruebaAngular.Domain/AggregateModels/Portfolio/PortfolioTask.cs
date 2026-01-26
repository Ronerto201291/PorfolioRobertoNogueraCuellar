using System;

namespace PruebaAngular.Domain.AggregateModels.Portfolio
{
    /// <summary>
    /// PortfolioTask represents a task within a project
    /// </summary>
    public class PortfolioTask
    {
        public Guid TaskId { get; set; }
        public Guid ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = "Pending";
        public string Priority { get; set; } = "Medium";
        public DateTimeOffset? DueDate { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public virtual Project? Project { get; set; }

        public static PortfolioTask Create(Guid projectId, string title, string status, string priority, string? description = null, DateTimeOffset? dueDate = null)
        {
            return new PortfolioTask
            {
                TaskId = Guid.NewGuid(),
                ProjectId = projectId,
                Title = title,
                Status = status,
                Priority = priority,
                Description = description,
                DueDate = dueDate,
                CreatedAt = DateTimeOffset.UtcNow
            };
        }
    }
}
