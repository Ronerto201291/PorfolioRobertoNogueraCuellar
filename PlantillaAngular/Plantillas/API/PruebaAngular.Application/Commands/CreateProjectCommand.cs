using MediatR;
using System;

namespace PruebaAngular.Application.Commands
{
    /// <summary>
    /// Command para crear un nuevo proyecto en el portfolio.
    /// Sigue el patrón CQRS - operación de escritura.
    /// </summary>
    public class CreateProjectCommand : IRequest<CreateProjectResult>
    {
        /// <summary>
        /// Nombre del proyecto (obligatorio)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del proyecto (opcional)
        /// </summary>
        public string? Description { get; set; }
    }

    /// <summary>
    /// Resultado de la creación de un proyecto
    /// </summary>
    public class CreateProjectResult
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public static CreateProjectResult Ok(Guid id, string name, string? description, DateTimeOffset createdAt)
        {
            return new CreateProjectResult
            {
                ProjectId = id,
                Name = name,
                Description = description,
                CreatedAt = createdAt,
                Success = true
            };
        }

        public static CreateProjectResult Fail(string error)
        {
            return new CreateProjectResult
            {
                Success = false,
                ErrorMessage = error
            };
        }
    }
}
