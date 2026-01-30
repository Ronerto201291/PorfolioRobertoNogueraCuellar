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
    /// Handler para el comando DeleteProjectCommand.
    /// Implementa la lógica de negocio para eliminar un proyecto.
    /// Flujo: API ? Command ? Handler ? Infrastructure ? PostgreSQL
    /// </summary>
    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, DeleteProjectResult>
    {
        private readonly PruebaAngularContext _context;
        private readonly ILogger<DeleteProjectCommandHandler> _logger;

        public DeleteProjectCommandHandler(
            PruebaAngularContext context,
            ILogger<DeleteProjectCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DeleteProjectResult> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Buscar el proyecto existente
                var project = await _context.Projects.FindAsync(
                    new object[] { request.ProjectId }, 
                    cancellationToken);

                if (project == null)
                {
                    _logger.LogWarning("Intento de eliminar proyecto inexistente: {ProjectId}", request.ProjectId);
                    return DeleteProjectResult.NotFound(request.ProjectId);
                }

                var projectName = project.Name;

                // Eliminar el proyecto (las tareas se eliminan en cascada por FK)
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Proyecto eliminado exitosamente: {ProjectId} - {ProjectName}",
                    request.ProjectId,
                    projectName);

                return DeleteProjectResult.Ok(request.ProjectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar proyecto {ProjectId}: {Message}", 
                    request.ProjectId, ex.Message);
                return DeleteProjectResult.Fail($"Error al eliminar el proyecto: {ex.Message}");
            }
        }
    }
}
