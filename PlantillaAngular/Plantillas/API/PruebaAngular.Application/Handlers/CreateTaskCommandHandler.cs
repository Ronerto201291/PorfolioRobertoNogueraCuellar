using MediatR;
using Microsoft.Extensions.Logging;
using PruebaAngular.Application.Commands;
using PruebaAngular.Domain.AggregateModels.Portfolio;
using PruebaAngular.Infrastructure.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PruebaAngular.Application.Handlers
{
    /// <summary>
    /// Handler para el comando CreateTaskCommand.
    /// Implementa la lógica de negocio para crear una tarea.
    /// Flujo: API ? Command ? Handler ? Infrastructure ? PostgreSQL
    /// </summary>
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, CreateTaskResult>
    {
        private readonly PruebaAngularContext _context;
        private readonly ILogger<CreateTaskCommandHandler> _logger;

        public CreateTaskCommandHandler(
            PruebaAngularContext context,
            ILogger<CreateTaskCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CreateTaskResult> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validación: título obligatorio
                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    _logger.LogWarning("Intento de crear tarea sin título");
                    return CreateTaskResult.Fail("El título de la tarea es obligatorio");
                }

                if (request.Title.Length > 200)
                {
                    return CreateTaskResult.Fail("El título no puede exceder 200 caracteres");
                }

                // Verificar que el proyecto existe
                var project = await _context.Projects.FindAsync(
                    new object[] { request.ProjectId }, 
                    cancellationToken);

                if (project == null)
                {
                    _logger.LogWarning("Proyecto no encontrado: {ProjectId}", request.ProjectId);
                    return CreateTaskResult.ProjectNotFound(request.ProjectId);
                }

                // Validar status
                var validStatuses = new[] { "Pending", "InProgress", "Completed" };
                if (!Array.Exists(validStatuses, s => s == request.Status))
                {
                    return CreateTaskResult.Fail($"Estado inválido. Valores permitidos: {string.Join(", ", validStatuses)}");
                }

                // Validar priority
                var validPriorities = new[] { "Low", "Medium", "High" };
                if (!Array.Exists(validPriorities, p => p == request.Priority))
                {
                    return CreateTaskResult.Fail($"Prioridad inválida. Valores permitidos: {string.Join(", ", validPriorities)}");
                }

                // Crear la tarea usando el factory method del dominio
                var task = PortfolioTask.Create(
                    projectId: request.ProjectId,
                    title: request.Title.Trim(),
                    status: request.Status,
                    priority: request.Priority,
                    description: request.Description?.Trim(),
                    dueDate: request.DueDate
                );

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Tarea creada exitosamente: {TaskId} - {Title} en proyecto {ProjectId}",
                    task.TaskId, task.Title, task.ProjectId);

                return CreateTaskResult.Ok(
                    task.TaskId, task.ProjectId, task.Title, task.Description,
                    task.Status, task.Priority, task.DueDate, task.CreatedAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear tarea: {Message}", ex.Message);
                return CreateTaskResult.Fail($"Error al crear la tarea: {ex.Message}");
            }
        }
    }
}
