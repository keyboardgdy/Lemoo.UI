namespace Lemoo.Core.Abstractions.Domain;

/// <summary>
/// 领域事件处理器接口
/// </summary>
/// <typeparam name="TEvent">领域事件类型</typeparam>
public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    /// <summary>
    /// 处理领域事件
    /// </summary>
    /// <param name="domainEvent">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}

/// <summary>
/// Marker interface for domain event dispatcher
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatch a single domain event
    /// </summary>
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatch multiple domain events
    /// </summary>
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}

/// <summary>
/// Generic domain event dispatcher for specific event types
/// </summary>
public interface IDomainEventDispatcher<T> where T : IDomainEvent
{
    Task DispatchAsync(T domainEvent, CancellationToken cancellationToken = default);
}
