using Lemoo.Core.Abstractions.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Lemoo.Core.Infrastructure.Messaging;

/// <summary>
/// RabbitMQ消息总线实现 - 支持分布式消息传递
/// </summary>
public class RabbitMqMessageBus : IMessageBus, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqMessageBus> _logger;
    private readonly IConnection? _connection;
    private readonly IModel? _channel;
    private readonly Dictionary<Type, string> _queueNames = new();
    private readonly Dictionary<string, EventingBasicConsumer> _consumers = new();
    private readonly Dictionary<string, Func<object, Task>> _handlers = new();
    private bool _disposed = false;

    public RabbitMqMessageBus(
        IConfiguration configuration,
        ILogger<RabbitMqMessageBus> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        var connectionString = configuration.GetValue<string>("Lemoo:Messaging:RabbitMQ:ConnectionString");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            _logger.LogWarning("RabbitMQ连接字符串未配置，消息总线将无法工作");
            return;
        }

        try
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(connectionString)
            };
            
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            _logger.LogInformation("RabbitMQ连接已建立");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建RabbitMQ连接失败");
        }
    }

    public async Task PublishAsync<TMessage>(
        TMessage message, 
        CancellationToken cancellationToken = default) 
        where TMessage : class
    {
        if (_channel == null || _connection == null || !_connection.IsOpen)
        {
            _logger.LogWarning("RabbitMQ未连接，消息发布失败");
            return;
        }

        if (message == null)
            throw new ArgumentNullException(nameof(message));

        try
        {
            var messageType = typeof(TMessage);
            var queueName = GetQueueName(messageType);
            
            // 确保队列存在
            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = Guid.NewGuid().ToString();

            _channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: properties,
                body: body);

            _logger.LogDebug("消息已发布: {MessageType}, Queue: {QueueName}", messageType.Name, queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发布消息失败: {MessageType}", typeof(TMessage).Name);
            throw;
        }
    }

    public string Subscribe<TMessage>(Func<TMessage, Task> handler) 
        where TMessage : class
    {
        if (_channel == null || _connection == null)
        {
            _logger.LogWarning("RabbitMQ未连接，消息订阅失败");
            return string.Empty;
        }

        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        try
        {
            var messageType = typeof(TMessage);
            var queueName = GetQueueName(messageType);
            var subscriptionId = Guid.NewGuid().ToString();
            
            // 确保队列存在
            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // 创建消费者
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<TMessage>(json);
                    
                    if (message != null)
                    {
                        await handler(message);
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "处理消息失败: {MessageType}", messageType.Name);
                    _channel.BasicNack(ea.DeliveryTag, false, true); // 重新入队
                }
            };

            // 开始消费
            _channel.BasicConsume(
                queue: queueName,
                autoAck: false,
                consumer: consumer);

            _consumers[subscriptionId] = consumer;
            _handlers[subscriptionId] = async (msg) =>
            {
                if (msg is TMessage typedMessage)
                {
                    await handler(typedMessage);
                }
            };

            _logger.LogInformation("消息订阅成功: {MessageType}, Queue: {QueueName}, SubscriptionId: {SubscriptionId}", 
                messageType.Name, queueName, subscriptionId);

            return subscriptionId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "订阅消息失败: {MessageType}", typeof(TMessage).Name);
            throw;
        }
    }

    public void Unsubscribe(string subscriptionId)
    {
        if (string.IsNullOrWhiteSpace(subscriptionId))
            return;

        if (_consumers.Remove(subscriptionId, out var consumer))
        {
            // 注意：RabbitMQ的消费者一旦启动，不能直接停止
            // 实际应用中应该使用更复杂的消费者管理
            _handlers.Remove(subscriptionId);
            _logger.LogInformation("取消订阅: {SubscriptionId}", subscriptionId);
        }
    }

    public void UnsubscribeAll()
    {
        _consumers.Clear();
        _handlers.Clear();
        _logger.LogInformation("已取消所有订阅");
    }

    private string GetQueueName(Type messageType)
    {
        if (_queueNames.TryGetValue(messageType, out var queueName))
        {
            return queueName;
        }

        queueName = $"lemoo.{messageType.Name.ToLowerInvariant()}";
        _queueNames[messageType] = queueName;
        return queueName;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _channel?.Close();
        _connection?.Close();
        _channel?.Dispose();
        _connection?.Dispose();
        
        _disposed = true;
        _logger.LogInformation("RabbitMQ连接已关闭");
    }
}

