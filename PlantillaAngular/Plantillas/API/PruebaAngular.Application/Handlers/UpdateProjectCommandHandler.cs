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
    /// Handler para el comando UpdateProjectCommand.
    /// Implementa la lógica de negocio para actualizar un proyecto.
    /// Flujo: API ? Command ? Handler ? Infrastructure ? PostgreSQL
    /// </summary>
    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, UpdateProjectResult>
    {
        private readonly PruebaAngularContext _context;
        private readonly ILogger<UpdateProjectCommandHandler> _logger;

        public UpdateProjectCommandHandler(
            PruebaAngularContext context,
            ILogger<UpdateProjectCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UpdateProjectResult> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validación: nombre obligatorio
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    _logger.LogWarning("Intento de actualizar proyecto sin nombre");
                    return UpdateProjectResult.Fail("El nombre del proyecto es obligatorio");
                }

                if (request.Name.Length > 200)
                {
                    _logger.LogWarning("Nombre de proyecto excede el límite de 200 caracteres");
                    return UpdateProjectResult.Fail("El nombre del proyecto no puede exceder 200 caracteres");
                }

                // Buscar el proyecto existente
                var project = await _context.Projects.FindAsync(
                    new object[] { request.ProjectId }, 
                    cancellationToken);

                if (project == null)
                {
                    _logger.LogWarning("Proyecto no encontrado: {ProjectId}", request.ProjectId);
                    return UpdateProjectResult.NotFound(request.ProjectId);
                }

                // Actualizar propiedades
                project.Name = request.Name.Trim();
                project.Description = request.Description?.Trim();

                // Persistir cambios
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Proyecto actualizado exitosamente: {ProjectId} - {ProjectName}",
                    project.ProjectId,
                    project.Name);

                return UpdateProjectResult.Ok(project.ProjectId, project.Name, project.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar proyecto {ProjectId}: {Message}", 
                    request.ProjectId, ex.Message);
                return UpdateProjectResult.Fail($"Error al actualizar el proyecto: {ex.Message}");
            }
        }
    }
}
