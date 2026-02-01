using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PruebaAngular.Application.Commands;
using PruebaAngular.Application.DTO.Requests;
using PruebaAngular.Application.Queries;

namespace PruebaAngular.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeNotificationRequest request, [FromServices] IMediator mediator)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await mediator.Send(new SubscribeNotificationCommand { Email = request.Email });

            if (!result.Success)
            {
                if (result.Message?.Contains("already") == true)
                {
                    return Conflict(new { message = result.Message });
                }

                return BadRequest(new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetSubscribers), new { }, new { email = request.Email });
        }

        [HttpGet("subscribers")]
        public async Task<IActionResult> GetSubscribers([FromServices] IMediator mediator)
        {
            var list = await mediator.Send(new GetNotificationSubscribersQuery());
            return Ok(list);
        }
    }
}
