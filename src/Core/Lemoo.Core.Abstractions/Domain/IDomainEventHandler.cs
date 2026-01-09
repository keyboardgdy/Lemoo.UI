namespace Lemoo.Core.Abstractions.Domain;

/// <summary>
/// 领域事件处理器接口
/// </summary>
/// <typeparam name="TDomainEvent">领域事件类型</typeparam>
public interface IDomainEventHandler<in TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken);
}

/// <summary>
/// 通知处理器接口（MediatR）
/// </summary>
/// <typeparam name="TNotification">通知类型</typeparam>
public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}

/// <summary>
/// 通知接口（MediatR）
/// </summary>
public interface INotification
{
}

