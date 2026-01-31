using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PruebaAngular.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        // NOTE: persistence is handled via EF Core through MediatR handler.
        // The in-memory store was removed in favor of DB-backed subscribers.

        public class SubscribeRequest
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request, [FromServices] IMediator mediator)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await mediator.Send(new PruebaAngular.Application.Commands.SubscribeNotificationCommand { Email = request.Email });

            if (!result.Success)
            {
                if (result.Message?.Contains("already") == true)
                    return Conflict(new { message = result.Message });

                return BadRequest(new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetSubscribers), new { }, new { email = request.Email });
        }

        [HttpGet("subscribers")]
        public async Task<IActionResult> GetSubscribers([FromServices] PruebaAngularContext db)
        {
            var list = await db.Set<PruebaAngular.Domain.AggregateModels.NotificationSubscriber>()
                .Select(s => s.Email)
                .ToArrayAsync();
            return Ok(list);
        }
    }
}
