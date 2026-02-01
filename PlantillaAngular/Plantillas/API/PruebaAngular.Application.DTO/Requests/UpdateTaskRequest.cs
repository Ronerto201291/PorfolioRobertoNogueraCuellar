using System;
using System.ComponentModel.DataAnnotations;

namespace PruebaAngular.Application.DTO.Requests
{
    /// <summary>
    /// DTO para la petición de actualización de una tarea desde la API.
    /// Debe mapearse al `UpdateTaskCommand` en la capa de aplicación.
    /// </summary>
    public class UpdateTaskRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Status { get; set; } = "Pending";

        public string Priority { get; set; } = "Medium";

        public DateTimeOffset? DueDate { get; set; }
    }
}
