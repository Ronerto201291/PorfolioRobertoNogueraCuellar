using MediatR;
using System;

namespace PruebaAngular.Application.Commands
{
    /// <summary>
    /// Command para actualizar un proyecto existente.
    /// Sigue el patrón CQRS - operación de escritura.
    /// </summary>
    public class UpdateProjectCommand : IRequest<UpdateProjectResult>
    {
        /// <summary>
        /// ID del proyecto a actualizar
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Nuevo nombre del proyecto (obligatorio)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Nueva descripción del proyecto (opcional)
        /// </summary>
        public string? Description { get; set; }
    }

    /// <summary>
    /// Resultado de la actualización de un proyecto
    /// </summary>
    public class UpdateProjectResult
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public static UpdateProjectResult Ok(Guid id, string name, string? description)
        {
            return new UpdateProjectResult
            {
                ProjectId = id,
                Name = name,
                Description = description,
                Success = true
            };
        }

        public static UpdateProjectResult Fail(string error)
        {
            return new UpdateProjectResult
            {
                Success = false,
                ErrorMessage = error
            };
        }

        public static UpdateProjectResult NotFound(Guid id)
        {
            return new UpdateProjectResult
            {
                Success = false,
                ErrorMessage = $"No se encontró el proyecto con ID: {id}"
            };
        }
    }
}
