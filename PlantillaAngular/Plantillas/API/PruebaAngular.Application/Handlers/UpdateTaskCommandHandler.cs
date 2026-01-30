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
    /// Handler para el comando UpdateTaskCommand.
    /// Implementa la lógica de negocio para actualizar una tarea.
    /// Flujo: API ? Command ? Handler ? Infrastructure ? PostgreSQL
    /// </summary>
    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, UpdateTaskResult>
    {
        private readonly PruebaAngularContext _context;
        private readonly ILogger<UpdateTaskCommandHandler> _logger;

        public UpdateTaskCommandHandler(
            PruebaAngularContext context,
            ILogger<UpdateTaskCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UpdateTaskResult> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validación: título obligatorio
                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    _logger.LogWarning("Intento de actualizar tarea sin título");
                    return UpdateTaskResult.Fail("El título de la tarea es obligatorio");
                }

                if (request.Title.Length > 200)
                {
                    return UpdateTaskResult.Fail("El título no puede exceder 200 caracteres");
                }

                // Buscar la tarea existente
                var task = await _context.Tasks.FindAsync(
                    new object[] { request.TaskId }, 
                    cancellationToken);

                if (task == null)
                {
                    _logger.LogWarning("Tarea no encontrada: {TaskId}", request.TaskId);
                    return UpdateTaskResult.NotFound(request.TaskId);
                }

                // Validar status
                var validStatuses = new[] { "Pending", "InProgress", "Completed" };
                if (!Array.Exists(validStatuses, s => s == request.Status))
                {
                    return UpdateTaskResult.Fail($"Estado inválido. Valores permitidos: {string.Join(", ", validStatuses)}");
                }

                // Validar priority
                var validPriorities = new[] { "Low", "Medium", "High" };
                if (!Array.Exists(validPriorities, p => p == request.Priority))
                {
                    return UpdateTaskResult.Fail($"Prioridad inválida. Valores permitidos: {string.Join(", ", validPriorities)}");
                }

                // Actualizar propiedades
                task.Title = request.Title.Trim();
                task.Description = request.Description?.Trim();
                task.Status = request.Status;
                task.Priority = request.Priority;
                task.DueDate = request.DueDate;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Tarea actualizada exitosamente: {TaskId} - {Title}",
                    task.TaskId, task.Title);

                return UpdateTaskResult.Ok(
                    task.TaskId, task.Title, task.Description,
                    task.Status, task.Priority, task.DueDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar tarea {TaskId}: {Message}", 
                    request.TaskId, ex.Message);
                return UpdateTaskResult.Fail($"Error al actualizar la tarea: {ex.Message}");
            }
        }
    }
}
