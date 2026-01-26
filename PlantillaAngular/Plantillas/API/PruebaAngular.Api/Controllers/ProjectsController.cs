using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PruebaAngular.Api.Model;
using PruebaAngular.Application.Commands;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PruebaAngular.Api.Controllers
{
    /// <summary>
    /// Controller para gestión de proyectos del portfolio.
    /// Expone operaciones CRUD siguiendo el patrón CQRS.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProjectsController : BaseApiController
    {
        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <param name="request">Datos del proyecto a crear</param>
        /// <returns>El proyecto creado con su ID asignado</returns>
        /// <response code="200">Proyecto creado exitosamente</response>
        /// <response code="400">Datos de entrada inválidos (ej: nombre vacío)</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiDataResult<CreateProjectResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiDataResult<CreateProjectResult>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiDataResult<CreateProjectResult>>> CreateProject(
            [FromBody] CreateProjectRequest request)
        {
            // Crear el command desde el request
            var command = new CreateProjectCommand
            {
                Name = request.Name,
                Description = request.Description
            };

            // Enviar al handler via MediatR
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(CreateApiResult(result, result.ErrorMessage));
            }

            return Ok(CreateApiResult(result, "Proyecto creado exitosamente"));
        }
    }

    /// <summary>
    /// Request para crear un nuevo proyecto
    /// </summary>
    public class CreateProjectRequest
    {
        /// <summary>
        /// Nombre del proyecto (obligatorio, máximo 200 caracteres)
        /// </summary>
        /// <example>Mi Portfolio App</example>
        [Required(ErrorMessage = "El nombre del proyecto es obligatorio")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del proyecto (opcional, máximo 1000 caracteres)
        /// </summary>
        /// <example>Una aplicación full-stack con Angular y .NET</example>
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string? Description { get; set; }
    }
}
