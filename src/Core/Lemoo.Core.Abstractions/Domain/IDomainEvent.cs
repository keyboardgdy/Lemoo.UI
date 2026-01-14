namespace Lemoo.Core.Abstractions.Domain;

/// <summary>
/// 领域事件接口
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Event occurrence timestamp
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// Unique event identifier
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Event version for future compatibility
    /// </summary>
    int Version { get; }
}

/// <summary>
/// Base class for domain events with automatic timestamp and ID
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid EventId { get; } = Guid.NewGuid();
    public virtual int Version => 1;
}

