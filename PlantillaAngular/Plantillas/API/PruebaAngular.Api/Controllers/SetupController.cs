using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PruebaAngular.Application.Commands;
using PruebaAngular.Application.Queries;

namespace PruebaAngular.Api.Controllers
{
    /// <summary>
    /// Controlador para configuración e inicialización de la base de datos.
    /// Sigue el patrón CQRS - delega la lógica a Commands y Queries vía MediatR.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SetupController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SetupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Inicializa la base de datos - crea tablas y datos semilla
        /// </summary>
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

        /// <summary>
        /// Verifica el estado de conexión de la base de datos
        /// </summary>
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
