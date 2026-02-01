using System;
using System.Collections.Generic;

namespace PruebaAngular.Application.DTO.Response
{
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
