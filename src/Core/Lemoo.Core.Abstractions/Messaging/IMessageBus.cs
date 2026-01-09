namespace Lemoo.Core.Abstractions.Messaging;

/// <summary>
/// 消息总线接口 - 提供发布/订阅模式的消息传递
/// </summary>
public interface IMessageBus
{
    /// <summary>
    /// 发布消息
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <param name="message">消息</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task PublishAsync<TMessage>(
        TMessage message, 
        CancellationToken cancellationToken = default) 
        where TMessage : class;
    
    /// <summary>
    /// 订阅消息
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <param name="handler">消息处理器</param>
    /// <returns>订阅ID，可用于取消订阅</returns>
    string Subscribe<TMessage>(Func<TMessage, Task> handler) 
        where TMessage : class;
    
    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <param name="subscriptionId">订阅ID</param>
    void Unsubscribe(string subscriptionId);
    
    /// <summary>
    /// 取消所有订阅
    /// </summary>
    void UnsubscribeAll();
}

/// <summary>
/// 消息基类
/// </summary>
public abstract class Message
{
    public Guid MessageId { get; } = Guid.NewGuid();
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string? CorrelationId { get; set; }
    public string? CausationId { get; set; }
}

/// <summary>
/// 领域事件消息（用于跨模块通信）
/// </summary>
public class DomainEventMessage : Message
{
    public string EventType { get; set; } = string.Empty;
    public object EventData { get; set; } = null!;
    public string ModuleName { get; set; } = string.Empty;
}

