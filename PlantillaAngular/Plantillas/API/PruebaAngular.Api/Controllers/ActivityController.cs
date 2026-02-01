using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PruebaAngular.Application.DTO.Response;
using PruebaAngular.Domain.Events;

namespace PruebaAngular.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ActivityController : ControllerBase
    {
        private readonly IEventBus _eventBus;

        public ActivityController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ActivityResponse), StatusCodes.Status200OK)]
        public IActionResult GetActivity([FromQuery] int count = 50)
        {
            count = Math.Clamp(count, 1, 100);
            
            var activities = _eventBus.GetRecentActivity(count);

            var response = new ActivityResponse
            {
                TotalCount = activities.Count,
                Activities = activities.Select(a => new ActivityItem
                {
                    EventId = a.EventId,
                    EventType = a.EventType,
                    Timestamp = a.Timestamp,
                    Status = a.Status.ToString(),
                    Details = a.Details
                }).ToList()
            };

            return Ok(response);
        }

        [HttpGet("summary")]
        [ProducesResponseType(typeof(ActivitySummary), StatusCodes.Status200OK)]
        public IActionResult GetSummary()
        {
            var activities = _eventBus.GetRecentActivity(100);

            var summary = new ActivitySummary
            {
                TotalEvents = activities.Count,
                PublishedCount = activities.Count(a => a.Status == EventActivityStatus.Published),
                ConsumedCount = activities.Count(a => a.Status == EventActivityStatus.Consumed),
                FailedCount = activities.Count(a => a.Status == EventActivityStatus.Failed),
                LastActivityAt = activities.FirstOrDefault()?.Timestamp,
                EventTypeBreakdown = activities
                    .GroupBy(a => a.EventType)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return Ok(summary);
        }
    }

}
