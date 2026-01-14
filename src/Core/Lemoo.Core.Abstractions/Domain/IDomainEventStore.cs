namespace Lemoo.Core.Abstractions.Domain;

/// <summary>
/// 领域事件流 - 聚合的所有历史事件
/// </summary>
public interface IDomainEventStream
{
    /// <summary>
    /// 聚合根ID
    /// </summary>
    string AggregateId { get; }

    /// <summary>
    /// 聚合根类型
    /// </summary>
    string AggregateType { get; }

    /// <summary>
    /// 事件流中的事件列表
    /// </summary>
    IReadOnlyList<IDomainEvent> Events { get; }

    /// <summary>
    /// 当前版本号
    /// </summary>
    int Version { get; }
}

/// <summary>
/// 领域事件存储接口 - 支持事件溯源
///
/// 事件溯源是一种持久化模式，它存储聚合的状态变更历史（事件）
/// 而不是只存储当前状态。通过重放事件可以重建聚合的任意历史状态。
/// </summary>
public interface IDomainEventStore
{
    /// <summary>
    /// 追加事件到事件流
    /// </summary>
    /// <param name="aggregateId">聚合根ID</param>
    /// <param name="aggregateType">聚合根类型</param>
    /// <param name="events">要追加的事件</param>
    /// <param name="expectedVersion">预期版本（用于乐观并发控制）</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task AppendAsync(
        string aggregateId,
        string aggregateType,
        IReadOnlyList<IDomainEvent> events,
        int expectedVersion,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取聚合的事件流
    /// </summary>
    /// <param name="aggregateId">聚合根ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task<IDomainEventStream?> ReadStreamAsync(
        string aggregateId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取聚合的事件流（从指定版本开始）
    /// </summary>
    /// <param name="aggregateId">聚合根ID</param>
    /// <param name="fromVersion">起始版本号</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task<IDomainEventStream?> ReadStreamAsync(
        string aggregateId,
        int fromVersion,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取聚合的事件流（指定版本范围）
    /// </summary>
    /// <param name="aggregateId">聚合根ID</param>
    /// <param name="fromVersion">起始版本号</param>
    /// <param name="toVersion">结束版本号</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task<IDomainEventStream?> ReadStreamAsync(
        string aggregateId,
        int fromVersion,
        int toVersion,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取全局事件流（所有事件，按时间顺序）
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    Task<IReadOnlyList<IDomainEvent>> ReadAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取全局事件流（从指定位置开始）
    /// </summary>
    /// <param name="fromPosition">起始位置</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task<IReadOnlyList<IDomainEvent>> ReadAllAsync(
        long fromPosition,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取全局事件流（分页）
    /// </summary>
    /// <param name="pageSize">页大小</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task<IReadOnlyList<IDomainEvent>> ReadAllAsync(
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取特定类型的事件
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task<IReadOnlyList<IDomainEvent>> ReadByTypeAsync(
        Type eventType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取特定类型的事件（时间范围）
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="from">起始时间</param>
    /// <param name="to">结束时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task<IReadOnlyList<IDomainEvent>> ReadByTypeAsync(
        Type eventType,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 订阅事件流变化
    /// </summary>
    /// <param name="subscriptionId">订阅ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task SubscribeAsync(
        string subscriptionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <param name="subscriptionId">订阅ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task UnsubscribeAsync(
        string subscriptionId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 事件快照接口 - 优化事件重放性能
///
/// 当聚合的事件数量过多时，重放所有事件会很慢。
/// 快照保存了聚合在某个版本的状态，可以从快照开始只重放之后的事件。
/// </summary>
public interface IEventSnapshotStore
{
    /// <summary>
    /// 保存快照
    /// </summary>
    /// <param name="aggregateId">聚合根ID</param>
    /// <param name="aggregateType">聚合根类型</param>
    /// <param name="snapshot">快照数据</param>
    /// <param name="version">版本号</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task SaveSnapshotAsync(
        string aggregateId,
        string aggregateType,
        object snapshot,
        int version,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取最新快照
    /// </summary>
    /// <param name="aggregateId">聚合根ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task<(object? snapshot, int version)?> GetLatestSnapshotAsync(
        string aggregateId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取指定版本的快照
    /// </summary>
    /// <param name="aggregateId">聚合根ID</param>
    /// <param name="version">版本号</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task<object?> GetSnapshotAsync(
        string aggregateId,
        int version,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 快照策略接口 - 定义何时创建快照
/// </summary>
public interface ISnapshotStrategy
{
    /// <summary>
    /// 判断是否应该创建快照
    /// </summary>
    /// <param name="aggregateId">聚合根ID</param>
    /// <param name="aggregateType">聚合根类型</param>
    /// <param name="version">当前版本</param>
    /// <param name="eventsSinceLastSnapshot">自上次快照以来的事件数</param>
    bool ShouldCreateSnapshot(
        string aggregateId,
        string aggregateType,
        int version,
        int eventsSinceLastSnapshot);
}

/// <summary>
/// 基于事件数量的快照策略
/// </summary>
public class CountBasedSnapshotStrategy : ISnapshotStrategy
{
    private readonly int _snapshotInterval;

    /// <summary>
    /// 创建基于事件数量的快照策略
    /// </summary>
    /// <param name="snapshotInterval">快照间隔（事件数量）</param>
    public CountBasedSnapshotStrategy(int snapshotInterval = 100)
    {
        _snapshotInterval = snapshotInterval;
    }

    public bool ShouldCreateSnapshot(
        string aggregateId,
        string aggregateType,
        int version,
        int eventsSinceLastSnapshot)
    {
        return eventsSinceLastSnapshot >= _snapshotInterval;
    }
}

/// <summary>
/// 基于时间的快照策略
/// </summary>
public class TimeBasedSnapshotStrategy : ISnapshotStrategy
{
    private readonly TimeSpan _interval;
    private readonly Dictionary<string, DateTime> _lastSnapshotTime = new();

    public TimeBasedSnapshotStrategy(TimeSpan interval)
    {
        _interval = interval;
    }

    public bool ShouldCreateSnapshot(
        string aggregateId,
        string aggregateType,
        int version,
        int eventsSinceLastSnapshot)
    {
        if (!_lastSnapshotTime.TryGetValue(aggregateId, out var lastTime))
        {
            _lastSnapshotTime[aggregateId] = DateTime.UtcNow;
            return true;
        }

        if (DateTime.UtcNow - lastTime >= _interval)
        {
            _lastSnapshotTime[aggregateId] = DateTime.UtcNow;
            return true;
        }

        return false;
    }
}
