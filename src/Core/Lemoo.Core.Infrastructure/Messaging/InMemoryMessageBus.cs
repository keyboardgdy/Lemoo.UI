using Lemoo.Core.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Lemoo.Core.Infrastructure.Messaging;

/// <summary>
/// 内存消息总线实现 - 基于内存的发布/订阅模式
/// </summary>
public class InMemoryMessageBus : IMessageBus
{
    private readonly ConcurrentDictionary<Type, List<MessageHandler>> _handlers = new();
    private readonly ConcurrentDictionary<string, MessageHandler> _subscriptions = new();
    private readonly ILogger<InMemoryMessageBus> _logger;
    private int _subscriptionCounter = 0;
    private readonly object _lockObject = new();

    public InMemoryMessageBus(ILogger<InMemoryMessageBus> logger)
    {
        _logger = logger;
    }

    public async Task PublishAsync<TMessage>(
        TMessage message, 
        CancellationToken cancellationToken = default) 
        where TMessage : class
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        var messageType = typeof(TMessage);
        
        if (!_handlers.TryGetValue(messageType, out var handlers) || handlers.Count == 0)
        {
            _logger.LogDebug("没有订阅者处理消息类型: {MessageType}", messageType.Name);
            return;
        }

        _logger.LogDebug("发布消息: {MessageType}, 订阅者数量: {Count}", messageType.Name, handlers.Count);

        // 并行执行所有处理器
        var tasks = handlers.Select(handler => ExecuteHandlerAsync(handler, message, cancellationToken));
        await Task.WhenAll(tasks);
    }

    public string Subscribe<TMessage>(Func<TMessage, Task> handler) 
        where TMessage : class
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        var messageType = typeof(TMessage);
        var subscriptionId = GenerateSubscriptionId();
        
        var messageHandler = new MessageHandler
        {
            SubscriptionId = subscriptionId,
            MessageType = messageType,
            Handler = async (msg, ct) =>
            {
                if (msg is TMessage typedMessage)
                {
                    await handler(typedMessage);
                }
            }
        };

        lock (_lockObject)
        {
            if (!_handlers.TryGetValue(messageType, out var handlers))
            {
                handlers = new List<MessageHandler>();
                _handlers[messageType] = handlers;
            }
            
            handlers.Add(messageHandler);
            _subscriptions[subscriptionId] = messageHandler;
        }

        _logger.LogInformation("订阅消息类型: {MessageType}, 订阅ID: {SubscriptionId}", 
            messageType.Name, subscriptionId);

        return subscriptionId;
    }

    public void Unsubscribe(string subscriptionId)
    {
        if (string.IsNullOrWhiteSpace(subscriptionId))
            return;

        if (!_subscriptions.TryRemove(subscriptionId, out var handler))
        {
            _logger.LogWarning("未找到订阅: {SubscriptionId}", subscriptionId);
            return;
        }

        lock (_lockObject)
        {
            if (_handlers.TryGetValue(handler.MessageType, out var handlers))
            {
                handlers.Remove(handler);
                if (handlers.Count == 0)
                {
                    _handlers.TryRemove(handler.MessageType, out _);
                }
            }
        }

        _logger.LogInformation("取消订阅: {SubscriptionId}", subscriptionId);
    }

    public void UnsubscribeAll()
    {
        lock (_lockObject)
        {
            _handlers.Clear();
            _subscriptions.Clear();
        }

        _logger.LogInformation("已取消所有订阅");
    }

    private async Task ExecuteHandlerAsync(
        MessageHandler handler, 
        object message, 
        CancellationToken cancellationToken)
    {
        try
        {
            await handler.Handler(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理消息时发生错误: {MessageType}, 订阅ID: {SubscriptionId}", 
                handler.MessageType.Name, handler.SubscriptionId);
            // 不抛出异常，允许其他处理器继续执行
        }
    }

    private string GenerateSubscriptionId()
    {
        return $"sub_{Interlocked.Increment(ref _subscriptionCounter)}_{Guid.NewGuid():N}";
    }

    private class MessageHandler
    {
        public string SubscriptionId { get; set; } = string.Empty;
        public Type MessageType { get; set; } = null!;
        public Func<object, CancellationToken, Task> Handler { get; set; } = null!;
    }
}

