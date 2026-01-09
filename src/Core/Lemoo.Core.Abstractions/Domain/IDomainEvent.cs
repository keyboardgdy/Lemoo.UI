namespace Lemoo.Core.Abstractions.Domain;

/// <summary>
/// 领域事件接口
/// </summary>
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
    Guid EventId { get; }
}

