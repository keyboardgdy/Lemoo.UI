using Lemoo.Core.Abstractions.Specifications;

namespace Lemoo.Core.Abstractions.Persistence;

/// <summary>
/// 仓储接口
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TKey">主键类型</typeparam>
public interface IRepository<TEntity, in TKey>
    where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    // Specification pattern support
    Task<IEnumerable<TEntity>> GetAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<TResult> FirstOrDefaultAsync<TResult>(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        where TResult : class;

    // Bulk operations
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    // Batch operations
    Task<IReadOnlyList<TEntity>> GetByIdsAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);

    // Paged operations
    Task<PagedResult<TEntity>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<TEntity>> GetPagedAsync(ISpecification<TEntity> specification, PagedRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// 只读仓储接口
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TKey">主键类型</typeparam>
public interface IReadOnlyRepository<TEntity, in TKey>
    where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    // Specification pattern support
    Task<IEnumerable<TEntity>> GetAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    // Batch operations
    Task<IReadOnlyList<TEntity>> GetByIdsAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);

    // Paged operations
    Task<PagedResult<TEntity>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<TEntity>> GetPagedAsync(ISpecification<TEntity> specification, PagedRequest request, CancellationToken cancellationToken = default);

    // Query is kept for backward compatibility but should be used sparingly
    [Obsolete("Consider using Specification pattern instead of direct IQueryable access")]
    IQueryable<TEntity> Query();
}

