using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PruebaAngular.Application.Commands;
using PruebaAngular.Application.Queries;

namespace PruebaAngular.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SetupController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SetupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("init")]
        public async Task<IActionResult> InitializeDatabase()
        {
            var result = await _mediator.Send(new InitializeDatabaseCommand());

            if (!result.Success)
            {
                return StatusCode(500, new { error = result.ErrorMessage });
            }

            return Ok(new 
            { 
                message = result.Message, 
                tablesCreated = result.TablesCreated, 
                dataSeeded = result.DataSeeded 
            });
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var result = await _mediator.Send(new GetDatabaseStatusQuery());

            return Ok(new 
            { 
                connected = result.Connected,
                connectionString = result.ConnectionInfo,
                error = result.ErrorMessage
            });
        }
    }
}
