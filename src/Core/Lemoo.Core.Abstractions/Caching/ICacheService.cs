namespace Lemoo.Core.Abstractions.Caching;

/// <summary>
/// 缓存服务接口
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// 获取缓存值
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置缓存值
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除缓存
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据模式移除缓存
    /// </summary>
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查缓存是否存在
    /// </summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 刷新缓存过期时间
    /// </summary>
    Task RefreshAsync(string key, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量获取缓存值
    /// </summary>
    Task<IDictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量设置缓存值
    /// </summary>
    Task SetManyAsync<T>(IDictionary<string, T> items, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量移除缓存
    /// </summary>
    Task RemoveManyAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);
}

/// <summary>
/// 分布式缓存服务接口 - 支持跨进程/服务器共享缓存
/// </summary>
public interface IDistributedCacheService : ICacheService
{
    /// <summary>
    /// 获取或创建缓存值（原子操作）
    /// </summary>
    Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取或创建缓存值（带锁，防止缓存击穿）
    /// </summary>
    Task<T?> GetOrCreateAtomicAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        TimeSpan? lockTimeout = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置缓存（仅当不存在时）
    /// </summary>
    Task<bool> SetIfNotExistsAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置缓存（仅当存在时）
    /// </summary>
    Task<bool> SetIfExistsAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取并删除缓存（原子操作）
    /// </summary>
    Task<T?> GetAndRemoveAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 增加计数器值（原子操作）
    /// </summary>
    Task<long> IncrementAsync(string key, long value = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// 减少计数器值（原子操作）
    /// </summary>
    Task<long> DecrementAsync(string key, long value = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取缓存剩余过期时间
    /// </summary>
    Task<TimeSpan?> GetExpirationAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 扫描匹配的键
    /// </summary>
    Task<IReadOnlyList<string>> ScanKeysAsync(string pattern, int pageSize = 100, CancellationToken cancellationToken = default);
}

/// <summary>
/// 缓存选项
/// </summary>
public record CacheOptions
{
    /// <summary>
    /// 过期时间
    /// </summary>
    public TimeSpan? Expiration { get; init; }

    /// <summary>
    /// 滑动过期时间
    /// </summary>
    public TimeSpan? SlidingExpiration { get; init; }

    /// <summary>
    /// 绝对过期时间
    /// </summary>
    public DateTime? AbsoluteExpiration { get; init; }

    /// <summary>
    /// 优先级
    /// </summary>
    public CacheItemPriority Priority { get; init; } = CacheItemPriority.Normal;

    /// <summary>
    /// 标签
    /// </summary>
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();
}

/// <summary>
/// 缓存项优先级
/// </summary>
public enum CacheItemPriority
{
    /// <summary>
    /// 低优先级
    /// </summary>
    Low,

    /// <summary>
    /// 普通优先级
    /// </summary>
    Normal,

    /// <summary>
    /// 高优先级
    /// </summary>
    High,

    /// <summary>
    /// 永不移除
    /// </summary>
    NeverRemove
}

/// <summary>
/// 缓存条目
/// </summary>
/// <typeparam name="T">值类型</typeparam>
public record CacheEntry<T>(T Value, CacheOptions Options);

