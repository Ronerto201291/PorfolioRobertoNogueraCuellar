using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PruebaAngular.Domain.Events;

namespace PruebaAngular.Api.Controllers
{
    /// <summary>
    /// Controller para consultar la actividad de eventos en RabbitMQ.
    /// Endpoint de solo lectura para monitoreo.
    /// </summary>
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

        /// <summary>
        /// Obtiene la actividad reciente de eventos (publicados/consumidos).
        /// </summary>
        /// <param name="count">Número de eventos a retornar (máximo 100)</param>
        /// <returns>Lista de actividad de eventos</returns>
        /// <response code="200">Lista de actividad</response>
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

        /// <summary>
        /// Obtiene un resumen de la actividad.
        /// </summary>
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

    public class ActivityResponse
    {
        public int TotalCount { get; set; }
        public List<ActivityItem> Activities { get; set; } = new();
    }

    public class ActivityItem
    {
        public Guid EventId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public DateTimeOffset Timestamp { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Details { get; set; }
    }

    public class ActivitySummary
    {
        public int TotalEvents { get; set; }
        public int PublishedCount { get; set; }
        public int ConsumedCount { get; set; }
        public int FailedCount { get; set; }
        public DateTimeOffset? LastActivityAt { get; set; }
        public Dictionary<string, int> EventTypeBreakdown { get; set; } = new();
    }
}
