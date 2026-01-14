namespace Lemoo.Core.Abstractions.Messaging;

/// <summary>
/// 消息重试策略
/// </summary>
public interface IMessageRetryPolicy
{
    /// <summary>
    /// 最大重试次数
    /// </summary>
    int MaxRetries { get; }

    /// <summary>
    /// 获取下次重试的延迟时间
    /// </summary>
    /// <param name="failedAttempt">失败的尝试次数（从0开始）</param>
    TimeSpan GetNextDelay(int failedAttempt);

    /// <summary>
    /// 判断异常是否可重试
    /// </summary>
    bool CanRetry(Exception exception);

    /// <summary>
    /// 创建默认的重试策略
    /// </summary>
    static IMessageRetryPolicy Default => new ExponentialBackoffRetryPolicy(maxRetries: 3, initialDelay: TimeSpan.FromSeconds(1));
}

/// <summary>
/// 指数退避重试策略
/// </summary>
public class ExponentialBackoffRetryPolicy : IMessageRetryPolicy
{
    private readonly TimeSpan _initialDelay;
    private readonly TimeSpan _maxDelay;
    private readonly double _backoffMultiplier;

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetries { get; }

    /// <summary>
    /// 创建指数退避重试策略
    /// </summary>
    /// <param name="maxRetries">最大重试次数</param>
    /// <param name="initialDelay">初始延迟</param>
    /// <param name="maxDelay">最大延迟</param>
    /// <param name="backoffMultiplier">退避乘数（默认2，即每次延迟翻倍）</param>
    public ExponentialBackoffRetryPolicy(
        int maxRetries = 3,
        TimeSpan? initialDelay = null,
        TimeSpan? maxDelay = null,
        double backoffMultiplier = 2.0)
    {
        if (maxRetries < 0)
            throw new ArgumentException("Max retries must be non-negative.", nameof(maxRetries));
        if (backoffMultiplier < 1.0)
            throw new ArgumentException("Backoff multiplier must be at least 1.0.", nameof(backoffMultiplier));

        MaxRetries = maxRetries;
        _initialDelay = initialDelay ?? TimeSpan.FromSeconds(1);
        _maxDelay = maxDelay ?? TimeSpan.FromMinutes(1);
        _backoffMultiplier = backoffMultiplier;
    }

    /// <summary>
    /// 获取下次重试的延迟时间
    /// </summary>
    public TimeSpan GetNextDelay(int failedAttempt)
    {
        var delay = TimeSpan.FromSeconds(
            Math.Min(
                _initialDelay.TotalSeconds * Math.Pow(_backoffMultiplier, failedAttempt),
                _maxDelay.TotalSeconds));

        return delay;
    }

    /// <summary>
    /// 判断异常是否可重试
    /// </summary>
    public virtual bool CanRetry(Exception exception)
    {
        // 默认可重试的异常类型
        return exception is TimeoutException
            or OperationCanceledException
            or IOException
            or HttpRequestException;
    }
}

/// <summary>
/// 固定延迟重试策略
/// </summary>
public class FixedDelayRetryPolicy : IMessageRetryPolicy
{
    private readonly TimeSpan _delay;

    public int MaxRetries { get; }

    public FixedDelayRetryPolicy(
        int maxRetries = 3,
        TimeSpan? delay = null)
    {
        MaxRetries = maxRetries;
        _delay = delay ?? TimeSpan.FromSeconds(5);
    }

    public TimeSpan GetNextDelay(int failedAttempt)
    {
        return _delay;
    }

    public virtual bool CanRetry(Exception exception)
    {
        return exception is TimeoutException
            or OperationCanceledException
            or IOException
            or HttpRequestException;
    }
}

/// <summary>
/// 线性退避重试策略
/// </summary>
public class LinearBackoffRetryPolicy : IMessageRetryPolicy
{
    private readonly TimeSpan _initialDelay;
    private readonly TimeSpan _increment;

    public int MaxRetries { get; }

    public LinearBackoffRetryPolicy(
        int maxRetries = 3,
        TimeSpan? initialDelay = null,
        TimeSpan? increment = null)
    {
        MaxRetries = maxRetries;
        _initialDelay = initialDelay ?? TimeSpan.FromSeconds(1);
        _increment = increment ?? TimeSpan.FromSeconds(1);
    }

    public TimeSpan GetNextDelay(int failedAttempt)
    {
        var delay = TimeSpan.FromSeconds(
            _initialDelay.TotalSeconds + (_increment.TotalSeconds * failedAttempt));
        return delay;
    }

    public virtual bool CanRetry(Exception exception)
    {
        return exception is TimeoutException
            or OperationCanceledException
            or IOException
            or HttpRequestException;
    }
}

/// <summary>
/// 死信消息
/// </summary>
public record DeadLetterMessage
{
    /// <summary>
    /// 原始消息
    /// </summary>
    public object Message { get; init; } = null!;

    /// <summary>
    /// 消息类型
    /// </summary>
    public Type MessageType { get; init; } = null!;

    /// <summary>
    /// 最后的异常
    /// </summary>
    public Exception LastException { get; init; } = null!;

    /// <summary>
    /// 重试次数
    /// </summary>
    public int RetryCount { get; init; }

    /// <summary>
    /// 首次失败时间
    /// </summary>
    public DateTime FirstFailedAt { get; init; }

    /// <summary>
    /// 最后失败时间
    /// </summary>
    public DateTime LastFailedAt { get; init; }

    /// <summary>
    /// 原始主题/队列
    /// </summary>
    public string OriginalTopic { get; init; } = null!;

    /// <summary>
    /// 元数据
    /// </summary>
    public IDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();
}

/// <summary>
/// 死信队列接口
/// </summary>
public interface IDeadLetterQueue
{
    /// <summary>
    /// 将消息添加到死信队列
    /// </summary>
    Task EnqueueAsync(DeadLetterMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// 从死信队列获取消息
    /// </summary>
    Task<DeadLetterMessage?> DequeueAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取死信队列中的所有消息
    /// </summary>
    Task<IReadOnlyList<DeadLetterMessage>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 按类型获取死信消息
    /// </summary>
    Task<IReadOnlyList<DeadLetterMessage>> GetByTypeAsync(
        Type messageType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按原始主题获取死信消息
    /// </summary>
    Task<IReadOnlyList<DeadLetterMessage>> GetByOriginalTopicAsync(
        string originalTopic,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 重试死信消息
    /// </summary>
    Task RetryAsync(DeadLetterMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量重试死信消息
    /// </summary>
    Task RetryBatchAsync(
        IEnumerable<DeadLetterMessage> messages,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除死信消息
    /// </summary>
    Task DeleteAsync(DeadLetterMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清空死信队列
    /// </summary>
    Task ClearAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取死信队列大小
    /// </summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// 增强的消息总线接口（支持重试和死信队列）
/// </summary>
public interface IResilientMessageBus : IMessageBus
{
    /// <summary>
    /// 发布消息（带重试策略）
    /// </summary>
    Task PublishAsync(
        object message,
        IMessageRetryPolicy? retryPolicy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 订阅消息（带重试策略）
    /// </summary>
    Task SubscribeAsync<T>(
        Func<T, Task> handler,
        IMessageRetryPolicy? retryPolicy = null,
        CancellationToken cancellationToken = default)
        where T : class;

    /// <summary>
    /// 获取死信队列
    /// </summary>
    IDeadLetterQueue GetDeadLetterQueue(string topic);
}
