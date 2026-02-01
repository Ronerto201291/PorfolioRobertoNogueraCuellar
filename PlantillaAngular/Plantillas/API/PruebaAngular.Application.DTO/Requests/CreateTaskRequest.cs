using System;
using System.ComponentModel.DataAnnotations;

namespace PruebaAngular.Application.DTO.Requests
{
    /// <summary>
    /// DTO para la petición de creación de una tarea desde la API.
    /// Debe mapearse al `CreateTaskCommand` en la capa de aplicación.
    /// </summary>
    public class CreateTaskRequest
    {
        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Status { get; set; } = "Pending";

        public string Priority { get; set; } = "Medium";

        public DateTimeOffset? DueDate { get; set; }
    }
}
