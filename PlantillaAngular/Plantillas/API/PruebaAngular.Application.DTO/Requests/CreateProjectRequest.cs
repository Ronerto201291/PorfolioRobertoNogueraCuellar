using System.ComponentModel.DataAnnotations;

namespace PruebaAngular.Application.DTO.Requests
{
    public class CreateProjectRequest
    {
        [Required(ErrorMessage = "El nombre del proyecto es obligatorio")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string? Description { get; set; }
    }
}
