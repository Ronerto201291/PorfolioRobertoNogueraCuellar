using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PruebaAngular.Domain.Events
{
    public interface IDomainEvent
    {
        Guid EventId { get; }
        DateTimeOffset OccurredAt { get; }
        string EventType { get; }
    }

    public abstract class DomainEvent : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
        public abstract string EventType { get; }
    }

    public class ProjectCreatedEvent : DomainEvent
    {
        public override string EventType => nameof(ProjectCreatedEvent);
        public Guid ProjectId { get; init; }
        public string ProjectName { get; init; } = string.Empty;
    }

    public class ProjectUpdatedEvent : DomainEvent
    {
        public override string EventType => nameof(ProjectUpdatedEvent);
        public Guid ProjectId { get; init; }
        public string ProjectName { get; init; } = string.Empty;
    }

    public class ProjectDeletedEvent : DomainEvent
    {
        public override string EventType => nameof(ProjectDeletedEvent);
        public Guid ProjectId { get; init; }
    }

    public class TaskCreatedEvent : DomainEvent
    {
        public override string EventType => nameof(TaskCreatedEvent);
        public Guid TaskId { get; init; }
        public Guid ProjectId { get; init; }
        public string TaskTitle { get; init; } = string.Empty;
    }

    public class TaskUpdatedEvent : DomainEvent
    {
        public override string EventType => nameof(TaskUpdatedEvent);
        public Guid TaskId { get; init; }
        public string TaskTitle { get; init; } = string.Empty;
        public string NewStatus { get; init; } = string.Empty;
    }

    public class TaskDeletedEvent : DomainEvent
    {
        public override string EventType => nameof(TaskDeletedEvent);
        public Guid TaskId { get; init; }
    }

    public class ActivityEvent : DomainEvent
    {
        private readonly string _eventType;

        public ActivityEvent(string eventType)
        {
            _eventType = eventType;
        }

        public override string EventType => _eventType;

        public string EntityId { get; init; } = string.Empty;
        public string EntityName { get; init; } = string.Empty;

        public DateTimeOffset Timestamp => OccurredAt;
    }

    public class EventActivity
    {
        public Guid EventId { get; init; }
        public string EventType { get; init; } = string.Empty;
        public DateTimeOffset Timestamp { get; init; }
        public EventActivityStatus Status { get; init; }
        public string? Details { get; init; }
    }

    public enum EventActivityStatus
    {
        Published,
        Consumed,
        Failed
    }

    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
            where TEvent : IDomainEvent;

        IReadOnlyList<EventActivity> GetRecentActivity(int count = 50);
    }
}
