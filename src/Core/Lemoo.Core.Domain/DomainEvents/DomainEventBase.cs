using Lemoo.Core.Abstractions.Domain;

namespace Lemoo.Core.Domain.DomainEvents;

/// <summary>
/// 领域事件基类
/// </summary>
public abstract class DomainEventBase : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public virtual int Version => 1;
}

