using MediatR;
using System;

namespace PruebaAngular.Application.Commands
{
    /// <summary>
    /// Command para actualizar una tarea existente.
    /// Sigue el patrón CQRS - operación de escritura.
    /// </summary>
    public class UpdateTaskCommand : IRequest<UpdateTaskResult>
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = "Pending";
        public string Priority { get; set; } = "Medium";
        public DateTimeOffset? DueDate { get; set; }
    }

    public class UpdateTaskResult
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTimeOffset? DueDate { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public static UpdateTaskResult Ok(Guid taskId, string title, string? description, 
            string status, string priority, DateTimeOffset? dueDate)
        {
            return new UpdateTaskResult
            {
                TaskId = taskId,
                Title = title,
                Description = description,
                Status = status,
                Priority = priority,
                DueDate = dueDate,
                Success = true
            };
        }

        public static UpdateTaskResult Fail(string error)
        {
            return new UpdateTaskResult { Success = false, ErrorMessage = error };
        }

        public static UpdateTaskResult NotFound(Guid taskId)
        {
            return new UpdateTaskResult 
            { 
                Success = false, 
                ErrorMessage = $"No se encontró la tarea con ID: {taskId}" 
            };
        }
    }
}
