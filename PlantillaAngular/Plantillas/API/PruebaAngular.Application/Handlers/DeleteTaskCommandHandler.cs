using MediatR;
using Microsoft.Extensions.Logging;
using PruebaAngular.Application.Commands;
using PruebaAngular.Infrastructure.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PruebaAngular.Application.Handlers
{
    /// <summary>
    /// Handler para el comando DeleteTaskCommand.
    /// Implementa la lógica de negocio para eliminar una tarea.
    /// Flujo: API ? Command ? Handler ? Infrastructure ? PostgreSQL
    /// </summary>
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, DeleteTaskResult>
    {
        private readonly PruebaAngularContext _context;
        private readonly ILogger<DeleteTaskCommandHandler> _logger;

        public DeleteTaskCommandHandler(
            PruebaAngularContext context,
            ILogger<DeleteTaskCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DeleteTaskResult> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Buscar la tarea existente
                var task = await _context.Tasks.FindAsync(
                    new object[] { request.TaskId }, 
                    cancellationToken);

                if (task == null)
                {
                    _logger.LogWarning("Intento de eliminar tarea inexistente: {TaskId}", request.TaskId);
                    return DeleteTaskResult.NotFound(request.TaskId);
                }

                var taskTitle = task.Title;

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Tarea eliminada exitosamente: {TaskId} - {Title}",
                    request.TaskId, taskTitle);

                return DeleteTaskResult.Ok(request.TaskId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar tarea {TaskId}: {Message}", 
                    request.TaskId, ex.Message);
                return DeleteTaskResult.Fail($"Error al eliminar la tarea: {ex.Message}");
            }
        }
    }
}
