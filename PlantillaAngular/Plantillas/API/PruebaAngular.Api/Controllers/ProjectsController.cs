using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PruebaAngular.Api.Model;
using PruebaAngular.Application.Commands;
using System;
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

        /// <summary>
        /// Crea un nuevo proyecto
        /// </summary>
        /// <param name="request">Datos del proyecto a crear</param>
        /// <returns>El proyecto creado con su ID asignado</returns>
        /// <response code="200">Proyecto creado exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiDataResult<CreateProjectResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiDataResult<CreateProjectResult>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiDataResult<CreateProjectResult>>> CreateProject(
            [FromBody] CreateProjectRequest request)
        {
            var command = new CreateProjectCommand
            {
                Name = request.Name,
                Description = request.Description
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(CreateApiResult(result, result.ErrorMessage));
            }

            return Ok(CreateApiResult(result, "Proyecto creado exitosamente"));
        }

        /// <summary>
        /// Actualiza un proyecto existente
        /// </summary>
        /// <param name="id">ID del proyecto a actualizar</param>
        /// <param name="request">Nuevos datos del proyecto</param>
        /// <returns>El proyecto actualizado</returns>
        /// <response code="200">Proyecto actualizado exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="404">Proyecto no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiDataResult<UpdateProjectResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiDataResult<UpdateProjectResult>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiDataResult<UpdateProjectResult>>> UpdateProject(
            [FromRoute] Guid id,
            [FromBody] UpdateProjectRequest request)
        {
            var command = new UpdateProjectCommand
            {
                ProjectId = id,
                Name = request.Name,
                Description = request.Description
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                if (result.ErrorMessage?.Contains("No se encontró") == true)
                {
                    return NotFound(CreateApiResult(result, result.ErrorMessage));
                }
                return BadRequest(CreateApiResult(result, result.ErrorMessage));
            }

            return Ok(CreateApiResult(result, "Proyecto actualizado exitosamente"));
        }

        /// <summary>
        /// Elimina un proyecto y todas sus tareas asociadas
        /// </summary>
        /// <param name="id">ID del proyecto a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="200">Proyecto eliminado exitosamente</response>
        /// <response code="404">Proyecto no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiDataResult<DeleteProjectResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiDataResult<DeleteProjectResult>>> DeleteProject(
            [FromRoute] Guid id)
        {
            var command = new DeleteProjectCommand(id);

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                if (result.ErrorMessage?.Contains("No se encontró") == true)
                {
                    return NotFound(CreateApiResult(result, result.ErrorMessage));
                }
                return BadRequest(CreateApiResult(result, result.ErrorMessage));
            }

            return Ok(CreateApiResult(result, result.Message));
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

    /// <summary>
    /// Request para actualizar un proyecto existente
    /// </summary>
    public class UpdateProjectRequest
    {
        /// <summary>
        /// Nombre del proyecto (obligatorio, máximo 200 caracteres)
        /// </summary>
        /// <example>Mi Portfolio App Actualizado</example>
        [Required(ErrorMessage = "El nombre del proyecto es obligatorio")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del proyecto (opcional, máximo 1000 caracteres)
        /// </summary>
        /// <example>Aplicación actualizada con nuevas funcionalidades</example>
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string? Description { get; set; }
    }
}
