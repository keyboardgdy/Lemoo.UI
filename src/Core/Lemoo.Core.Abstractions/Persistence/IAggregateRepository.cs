using Lemoo.Core.Abstractions.Domain;
using Lemoo.Core.Abstractions.Specifications;

namespace Lemoo.Core.Abstractions.Persistence;

/// <summary>
/// 聚合仓储接口 - 专门用于管理聚合根
///
/// 聚合仓储与普通仓储的区别：
/// 1. 自动处理领域事件的派发
/// 2. 确保聚合根的完整性
/// 3. 支持乐观并发控制
/// 4. 提供版本控制
/// </summary>
/// <typeparam name="TAggregate">聚合根类型</typeparam>
/// <typeparam name="TKey">主键类型</typeparam>
public interface IAggregateRepository<TAggregate, TKey>
    where TAggregate : class, IAggregateRoot<TKey>
    where TKey : notnull
{
    /// <summary>
    /// 根据ID获取聚合根
    /// </summary>
    Task<TAggregate?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据规范获取聚合根
    /// </summary>
    Task<IEnumerable<TAggregate>> GetAsync(ISpecification<TAggregate> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据规范获取单个聚合根
    /// </summary>
    Task<TAggregate?> FirstOrDefaultAsync(ISpecification<TAggregate> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加聚合根（会自动派发领域事件）
    /// </summary>
    Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新聚合根（会自动派发领域事件）
    /// </summary>
    Task UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除聚合根
    /// </summary>
    Task DeleteAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量添加聚合根
    /// </summary>
    Task AddRangeAsync(IEnumerable<TAggregate> aggregates, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查聚合根是否存在
    /// </summary>
    Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取聚合数量
    /// </summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取聚合数量（带规范）
    /// </summary>
    Task<int> CountAsync(ISpecification<TAggregate> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// 加载聚合（从数据库加载并恢复状态）
    /// </summary>
    Task<TAggregate?> LoadAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 保存聚合（持久化更改并派发领域事件）
    /// </summary>
    Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量保存聚合
    /// </summary>
    Task SaveRangeAsync(IEnumerable<TAggregate> aggregates, CancellationToken cancellationToken = default);
}

/// <summary>
/// 聚合仓储接口（使用 Guid 作为主键）
/// </summary>
/// <typeparam name="TAggregate">聚合根类型</typeparam>
public interface IAggregateRepository<TAggregate> : IAggregateRepository<TAggregate, Guid>
    where TAggregate : class, IAggregateRoot<Guid>
{
}
