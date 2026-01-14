namespace Lemoo.Core.Abstractions.Domain;

/// <summary>
/// 审计操作类型
/// </summary>
public enum AuditOperationType
{
    /// <summary>
    /// 创建
    /// </summary>
    Create,

    /// <summary>
    /// 更新
    /// </summary>
    Update,

    /// <summary>
    /// 删除
    /// </summary>
    Delete,

    /// <summary>
    /// 查询（敏感数据）
    /// </summary>
    Read,

    /// <summary>
    /// 软删除
    /// </summary>
    SoftDelete,

    /// <summary>
    /// 恢复
    /// </summary>
    Restore
}

/// <summary>
/// 审计日志条目
/// </summary>
public record AuditLogEntry
{
    /// <summary>
    /// 审计日志ID
    /// </summary>
    public Guid AuditId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// 操作类型
    /// </summary>
    public AuditOperationType OperationType { get; init; }

    /// <summary>
    /// 实体类型
    /// </summary>
    public string EntityType { get; init; } = string.Empty;

    /// <summary>
    /// 实体ID
    /// </summary>
    public string EntityId { get; init; } = string.Empty;

    /// <summary>
    /// 变更前的数据（JSON）
    /// </summary>
    public string? PreviousState { get; init; }

    /// <summary>
    /// 变更后的数据（JSON）
    /// </summary>
    public string? NewState { get; init; }

    /// <summary>
    /// 变更的属性列表
    /// </summary>
    public IReadOnlyList<string> ChangedProperties { get; init; } = Array.Empty<string>();

    /// <summary>
    /// 操作人
    /// </summary>
    public string? PerformedBy { get; init; }

    /// <summary>
    /// 操作时间
    /// </summary>
    public DateTime PerformedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 操作来源（IP地址、主机名等）
    /// </summary>
    public string? PerformedFrom { get; init; }

    /// <summary>
    /// 操作原因/备注
    /// </summary>
    public string? Reason { get; init; }

    /// <summary>
    /// 关联的业务ID（如订单号）
    /// </summary>
    public string? BusinessId { get; init; }

    /// <summary>
    /// 关联的操作ID（用于关联多个审计条目）
    /// </summary>
    public Guid? CorrelationId { get; init; }

    /// <summary>
    /// 额外的元数据
    /// </summary>
    public IDictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// 审计日志存储接口
/// </summary>
public interface IAuditLogStore
{
    /// <summary>
    /// 保存审计日志
    /// </summary>
    Task SaveAsync(AuditLogEntry entry, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量保存审计日志
    /// </summary>
    Task SaveBatchAsync(
        IReadOnlyList<AuditLogEntry> entries,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取实体的审计历史
    /// </summary>
    Task<IReadOnlyList<AuditLogEntry>> GetEntityHistoryAsync(
        string entityType,
        string entityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取实体的审计历史（分页）
    /// </summary>
    Task<IReadOnlyList<AuditLogEntry>> GetEntityHistoryAsync(
        string entityType,
        string entityId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户的操作历史
    /// </summary>
    Task<IReadOnlyList<AuditLogEntry>> GetUserHistoryAsync(
        string userId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按操作类型查询审计日志
    /// </summary>
    Task<IReadOnlyList<AuditLogEntry>> GetByOperationTypeAsync(
        AuditOperationType operationType,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按关联ID查询审计日志
    /// </summary>
    Task<IReadOnlyList<AuditLogEntry>> GetByCorrelationIdAsync(
        Guid correlationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 搜索审计日志
    /// </summary>
    Task<IReadOnlyList<AuditLogEntry>> SearchAsync(
        AuditLogSearchCriteria criteria,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除旧审计日志
    /// </summary>
    Task CleanupOldEntriesAsync(
        DateTime before,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取审计日志统计信息
    /// </summary>
    Task<AuditLogStatistics> GetStatisticsAsync(
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 审计日志搜索条件
/// </summary>
public record AuditLogSearchCriteria
{
    public string? EntityType { get; init; }
    public string? EntityId { get; init; }
    public string? PerformedBy { get; init; }
    public AuditOperationType? OperationType { get; init; }
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
    public string? BusinessId { get; init; }
    public Guid? CorrelationId { get; init; }
    public int PageIndex { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// 审计日志统计信息
/// </summary>
public record AuditLogStatistics
{
    public int TotalEntries { get; init; }
    public Dictionary<AuditOperationType, int> OperationsByType { get; init; } = new();
    public Dictionary<string, int> TopEntities { get; init; } = new();
    public Dictionary<string, int> TopUsers { get; init; } = new();
    public DateTime PeriodStart { get; init; }
    public DateTime PeriodEnd { get; init; }
}

/// <summary>
/// 审计日志配置
/// </summary>
public interface IAuditLogConfig
{
    /// <summary>
    /// 是否启用审计
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// 要审计的实体类型
    /// </summary>
    IReadOnlyList<Type> AuditedEntities { get; }

    /// <summary>
    /// 要审计的操作类型
    /// </summary>
    IReadOnlyList<AuditOperationType> AuditedOperations { get; }

    /// <summary>
    /// 是否记录变更前后的完整数据
    /// </summary>
    bool IncludeFullState { get; }

    /// <summary>
    /// 是否忽略敏感属性
    /// </summary>
    bool IgnoreSensitiveProperties { get; }

    /// <summary>
    /// 敏感属性名称列表
    /// </summary>
    IReadOnlyList<string> SensitiveProperties { get; }

    /// <summary>
    /// 审计日志保留天数
    /// </summary>
    int RetentionDays { get; }
}

/// <summary>
/// 审计日志构建器 - 简化审计日志的创建
/// </summary>
public interface IAuditLogBuilder
{
    /// <summary>
    /// 创建审计条目
    /// </summary>
    AuditLogEntry CreateEntry(
        AuditOperationType operation,
        object entity,
        object? previousState = null,
        object? newState = null);

    /// <summary>
    /// 创建删除操作的审计条目
    /// </summary>
    AuditLogEntry CreateDeleteEntry(object entity, object? previousState = null);

    /// <summary>
    /// 创建更新操作的审计条目
    /// </summary>
    AuditLogEntry CreateUpdateEntry(
        object entity,
        object? previousState = null,
        object? newState = null,
        IReadOnlyList<string>? changedProperties = null);

    /// <summary>
    /// 创建创建操作的审计条目
    /// </summary>
    AuditLogEntry CreateCreateEntry(object entity, object? newState = null);

    /// <summary>
    /// 设置操作人
    /// </summary>
    IAuditLogBuilder WithPerformedBy(string? userId);

    /// <summary>
    /// 设置操作来源
    /// </summary>
    IAuditLogBuilder WithPerformedFrom(string? source);

    /// <summary>
    /// 设置操作原因
    /// </summary>
    IAuditLogBuilder WithReason(string? reason);

    /// <summary>
    /// 设置业务ID
    /// </summary>
    IAuditLogBuilder WithBusinessId(string? businessId);

    /// <summary>
    /// 设置关联ID
    /// </summary>
    IAuditLogBuilder WithCorrelationId(Guid? correlationId);

    /// <summary>
    /// 添加元数据
    /// </summary>
    IAuditLogBuilder WithMetadata(string key, string value);

    /// <summary>
    /// 构建审计条目
    /// </summary>
    AuditLogEntry Build();
}
