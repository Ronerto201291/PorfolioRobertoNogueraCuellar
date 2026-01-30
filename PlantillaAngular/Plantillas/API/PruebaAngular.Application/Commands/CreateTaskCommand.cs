using MediatR;
using System;

namespace PruebaAngular.Application.Commands
{
    /// <summary>
    /// Command para crear una nueva tarea en un proyecto.
    /// Sigue el patrón CQRS - operación de escritura.
    /// </summary>
    public class CreateTaskCommand : IRequest<CreateTaskResult>
    {
        public Guid ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = "Pending";
        public string Priority { get; set; } = "Medium";
        public DateTimeOffset? DueDate { get; set; }
    }

    public class CreateTaskResult
    {
        public Guid TaskId { get; set; }
        public Guid ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTimeOffset? DueDate { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public static CreateTaskResult Ok(Guid taskId, Guid projectId, string title, string? description, 
            string status, string priority, DateTimeOffset? dueDate, DateTimeOffset createdAt)
        {
            return new CreateTaskResult
            {
                TaskId = taskId,
                ProjectId = projectId,
                Title = title,
                Description = description,
                Status = status,
                Priority = priority,
                DueDate = dueDate,
                CreatedAt = createdAt,
                Success = true
            };
        }

        public static CreateTaskResult Fail(string error)
        {
            return new CreateTaskResult { Success = false, ErrorMessage = error };
        }

        public static CreateTaskResult ProjectNotFound(Guid projectId)
        {
            return new CreateTaskResult 
            { 
                Success = false, 
                ErrorMessage = $"No se encontró el proyecto con ID: {projectId}" 
            };
        }
    }
}
