using MediatR;
using Microsoft.Extensions.Logging;
using PruebaAngular.Application.Commands;
using PruebaAngular.Domain.AggregateModels.Portfolio;
using PruebaAngular.Domain.Events;
using PruebaAngular.Infrastructure.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PruebaAngular.Application.Handlers
{
    /// <summary>
    /// Handler para el comando CreateProjectCommand.
    /// Implementa la lógica de negocio para crear un proyecto.
    /// Flujo: API ? Command ? Handler ? Infrastructure ? PostgreSQL ? RabbitMQ
    /// </summary>
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, CreateProjectResult>
    {
        private readonly PruebaAngularContext _context;
        private readonly IEventBus? _eventBus;
        private readonly ILogger<CreateProjectCommandHandler> _logger;

        public CreateProjectCommandHandler(
            PruebaAngularContext context,
            ILogger<CreateProjectCommandHandler> logger,
            IEventBus? eventBus = null)
        {
            _context = context;
            _logger = logger;
            _eventBus = eventBus;
        }

        public async Task<CreateProjectResult> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validación: nombre obligatorio
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    _logger.LogWarning("Intento de crear proyecto sin nombre");
                    return CreateProjectResult.Fail("El nombre del proyecto es obligatorio");
                }

                if (request.Name.Length > 200)
                {
                    _logger.LogWarning("Nombre de proyecto excede el límite de 200 caracteres");
                    return CreateProjectResult.Fail("El nombre del proyecto no puede exceder 200 caracteres");
                }

                var project = Project.Create(
                    name: request.Name.Trim(),
                    description: request.Description?.Trim()
                );

                _context.Projects.Add(project);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Proyecto creado exitosamente: {ProjectId} - {ProjectName}",
                    project.ProjectId,
                    project.Name);

                if (_eventBus != null)
                {
                    await _eventBus.PublishAsync(new ActivityEvent("ProjectCreated")
                    {
                        EntityId = project.ProjectId.ToString(),
                        EntityName = project.Name
                    }, cancellationToken);
                }

                return CreateProjectResult.Ok(
                    project.ProjectId,
                    project.Name,
                    project.Description,
                    project.CreatedAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear proyecto: {Message}", ex.Message);
                return CreateProjectResult.Fail($"Error interno: {ex.Message}");
            }
        }
    }
}
