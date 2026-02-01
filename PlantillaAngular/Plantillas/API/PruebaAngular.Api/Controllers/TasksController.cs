using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PruebaAngular.Api.Model;
using PruebaAngular.Application.Commands;
using PruebaAngular.Application.DTO.Requests;
using System;
using System.Threading.Tasks;

namespace PruebaAngular.Api.Controllers
{
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
