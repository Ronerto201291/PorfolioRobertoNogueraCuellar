using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PruebaAngular.Api.Model;
using PruebaAngular.Application.Commands;
using PruebaAngular.Application.DTO.Requests;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PruebaAngular.Api.Controllers
{
    /// <summary>
    /// Controller para gestión de tareas del portfolio.
    /// Expone operaciones CRUD siguiendo el patrón CQRS.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TasksController : BaseApiController
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Crea una nueva tarea en un proyecto
        /// </summary>
        /// <param name="request">Datos de la tarea a crear</param>
        /// <returns>La tarea creada con su ID asignado</returns>
        /// <response code="200">Tarea creada exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="404">Proyecto no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiDataResult<CreateTaskResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiDataResult<CreateTaskResult>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiDataResult<CreateTaskResult>>> CreateTask(
            [FromBody] CreateTaskRequest request)
        {
            var command = new CreateTaskCommand
            {
                ProjectId = request.ProjectId,
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                Priority = request.Priority,
                DueDate = request.DueDate
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                if (result.ErrorMessage?.Contains("No se encontró el proyecto") == true)
                {
                    return NotFound(CreateApiResult(result, result.ErrorMessage));
                }
                return BadRequest(CreateApiResult(result, result.ErrorMessage));
            }

            return Ok(CreateApiResult(result, "Tarea creada exitosamente"));
        }

        /// <summary>
        /// Actualiza una tarea existente
        /// </summary>
        /// <param name="id">ID de la tarea a actualizar</param>
        /// <param name="request">Nuevos datos de la tarea</param>
        /// <returns>La tarea actualizada</returns>
        /// <response code="200">Tarea actualizada exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="404">Tarea no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiDataResult<UpdateTaskResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiDataResult<UpdateTaskResult>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiDataResult<UpdateTaskResult>>> UpdateTask(
            [FromRoute] Guid id,
            [FromBody] UpdateTaskRequest request)
        {
            var command = new UpdateTaskCommand
            {
                TaskId = id,
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                Priority = request.Priority,
                DueDate = request.DueDate
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

            return Ok(CreateApiResult(result, "Tarea actualizada exitosamente"));
        }

        /// <summary>
        /// Elimina una tarea
        /// </summary>
        /// <param name="id">ID de la tarea a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="200">Tarea eliminada exitosamente</response>
        /// <response code="404">Tarea no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiDataResult<DeleteTaskResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiDataResult<DeleteTaskResult>>> DeleteTask(
            [FromRoute] Guid id)
        {
            var command = new DeleteTaskCommand(id);

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


}
