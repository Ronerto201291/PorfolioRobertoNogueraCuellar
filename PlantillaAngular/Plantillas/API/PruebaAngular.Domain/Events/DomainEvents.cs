using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PruebaAngular.Domain.Events
{
    /// <summary>
    /// Interfaz base para todos los eventos de dominio.
    /// </summary>
    public interface IDomainEvent
    {
        Guid EventId { get; }
        DateTimeOffset OccurredAt { get; }
        string EventType { get; }
    }

    /// <summary>
    /// Clase base abstracta para eventos de dominio.
    /// </summary>
    public abstract class DomainEvent : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
        public abstract string EventType { get; }
    }

    /// <summary>
    /// Evento emitido cuando se crea un proyecto.
    /// </summary>
    public class ProjectCreatedEvent : DomainEvent
    {
        public override string EventType => nameof(ProjectCreatedEvent);
        public Guid ProjectId { get; init; }
        public string ProjectName { get; init; } = string.Empty;
    }

    /// <summary>
    /// Evento emitido cuando se actualiza un proyecto.
    /// </summary>
    public class ProjectUpdatedEvent : DomainEvent
    {
        public override string EventType => nameof(ProjectUpdatedEvent);
        public Guid ProjectId { get; init; }
        public string ProjectName { get; init; } = string.Empty;
    }

    /// <summary>
    /// Evento emitido cuando se elimina un proyecto.
    /// </summary>
    public class ProjectDeletedEvent : DomainEvent
    {
        public override string EventType => nameof(ProjectDeletedEvent);
        public Guid ProjectId { get; init; }
    }

    /// <summary>
    /// Evento emitido cuando se crea una tarea.
    /// </summary>
    public class TaskCreatedEvent : DomainEvent
    {
        public override string EventType => nameof(TaskCreatedEvent);
        public Guid TaskId { get; init; }
        public Guid ProjectId { get; init; }
        public string TaskTitle { get; init; } = string.Empty;
    }

    /// <summary>
    /// Evento emitido cuando se actualiza una tarea.
    /// </summary>
    public class TaskUpdatedEvent : DomainEvent
    {
        public override string EventType => nameof(TaskUpdatedEvent);
        public Guid TaskId { get; init; }
        public string TaskTitle { get; init; } = string.Empty;
        public string NewStatus { get; init; } = string.Empty;
    }

    /// <summary>
    /// Evento emitido cuando se elimina una tarea.
    /// </summary>
    public class TaskDeletedEvent : DomainEvent
    {
        public override string EventType => nameof(TaskDeletedEvent);
        public Guid TaskId { get; init; }
    }

    /// <summary>
    /// Representa una actividad de evento (publicado o consumido).
    /// </summary>
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

    /// <summary>
    /// Interfaz para el bus de eventos.
    /// </summary>
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
            where TEvent : IDomainEvent;

        IReadOnlyList<EventActivity> GetRecentActivity(int count = 50);
    }
}
