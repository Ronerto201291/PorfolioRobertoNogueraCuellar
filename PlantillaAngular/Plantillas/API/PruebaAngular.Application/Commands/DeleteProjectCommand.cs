using MediatR;
using System;

namespace PruebaAngular.Application.Commands
{
    /// <summary>
    /// Command para eliminar un proyecto existente.
    /// Sigue el patrón CQRS - operación de escritura.
    /// </summary>
    public class DeleteProjectCommand : IRequest<DeleteProjectResult>
    {
        /// <summary>
        /// ID del proyecto a eliminar
        /// </summary>
        public Guid ProjectId { get; set; }

        public DeleteProjectCommand(Guid projectId)
        {
            ProjectId = projectId;
        }
    }

    /// <summary>
    /// Resultado de la eliminación de un proyecto
    /// </summary>
    public class DeleteProjectResult
    {
        public Guid ProjectId { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }

        public static DeleteProjectResult Ok(Guid id)
        {
            return new DeleteProjectResult
            {
                ProjectId = id,
                Success = true,
                Message = "Proyecto eliminado correctamente"
            };
        }

        public static DeleteProjectResult Fail(string error)
        {
            return new DeleteProjectResult
            {
                Success = false,
                ErrorMessage = error
            };
        }

        public static DeleteProjectResult NotFound(Guid id)
        {
            return new DeleteProjectResult
            {
                ProjectId = id,
                Success = false,
                ErrorMessage = $"No se encontró el proyecto con ID: {id}"
            };
        }
    }
}
