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
    public class ProjectsController : BaseApiController
    {
        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

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

}
