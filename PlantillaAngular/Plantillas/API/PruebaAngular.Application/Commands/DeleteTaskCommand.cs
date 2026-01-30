using MediatR;
using System;

namespace PruebaAngular.Application.Commands
{
    /// <summary>
    /// Command para eliminar una tarea existente.
    /// Sigue el patrón CQRS - operación de escritura.
    /// </summary>
    public class DeleteTaskCommand : IRequest<DeleteTaskResult>
    {
        public Guid TaskId { get; set; }

        public DeleteTaskCommand(Guid taskId)
        {
            TaskId = taskId;
        }
    }

    public class DeleteTaskResult
    {
        public Guid TaskId { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }

        public static DeleteTaskResult Ok(Guid taskId)
        {
            return new DeleteTaskResult
            {
                TaskId = taskId,
                Success = true,
                Message = "Tarea eliminada correctamente"
            };
        }

        public static DeleteTaskResult Fail(string error)
        {
            return new DeleteTaskResult { Success = false, ErrorMessage = error };
        }

        public static DeleteTaskResult NotFound(Guid taskId)
        {
            return new DeleteTaskResult
            {
                TaskId = taskId,
                Success = false,
                ErrorMessage = $"No se encontró la tarea con ID: {taskId}"
            };
        }
    }
}
