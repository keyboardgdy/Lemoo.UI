using Lemoo.Core.Abstractions.Domain;
using Lemoo.Core.Abstractions.Persistence;
using Lemoo.Core.Abstractions.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Lemoo.Core.Infrastructure.Persistence;

/// <summary>
/// Base repository implementation using EF Core with Specification pattern support
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    protected readonly DbContext DbContext;
    protected readonly DbSet<TEntity> DbSet;
    protected readonly ILogger<Repository<TEntity, TKey>> Logger;
    protected readonly ISpecificationEvaluator SpecificationEvaluator;

    public Repository(DbContext dbContext, ILogger<Repository<TEntity, TKey>> logger)
        : this(dbContext, logger, Lemoo.Core.Abstractions.Specifications.SpecificationEvaluator.Instance)
    {
    }

    public Repository(
        DbContext dbContext,
        ILogger<Repository<TEntity, TKey>> logger,
        ISpecificationEvaluator specificationEvaluator)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        DbSet = dbContext.Set<TEntity>();
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        SpecificationEvaluator = specificationEvaluator ?? throw new ArgumentNullException(nameof(specificationEvaluator));
    }

    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    // Specification pattern support
    public virtual async Task<IEnumerable<TEntity>> GetAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TResult> FirstOrDefaultAsync<TResult>(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
        where TResult : class
    {
        var query = (IQueryable<TResult>)ApplySpecification(specification);
        return await query.FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException($"No result found for specification");
    }

    // Bulk operations
    public virtual async Task AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        if (entities == null) throw new ArgumentNullException(nameof(entities));

        var entityList = entities.ToList();
        await DbSet.AddRangeAsync(entityList, cancellationToken);
        Logger.LogDebug("添加 {Count} 个实体: {EntityType}", entityList.Count, typeof(TEntity).Name);
    }

    public virtual Task UpdateRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        if (entities == null) throw new ArgumentNullException(nameof(entities));

        var entityList = entities.ToList();
        DbSet.UpdateRange(entityList);
        Logger.LogDebug("更新 {Count} 个实体: {EntityType}", entityList.Count, typeof(TEntity).Name);
        return Task.CompletedTask;
    }

    public virtual Task DeleteRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        if (entities == null) throw new ArgumentNullException(nameof(entities));

        var entityList = entities.ToList();
        DbSet.RemoveRange(entityList);
        Logger.LogDebug("删除 {Count} 个实体: {EntityType}", entityList.Count, typeof(TEntity).Name);
        return Task.CompletedTask;
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var entry = await DbSet.AddAsync(entity, cancellationToken);
        Logger.LogDebug("添加实体: {EntityType}, Id: {EntityId}", typeof(TEntity).Name, entity.Id);
        return entry.Entity;
    }

    public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        Logger.LogDebug("更新实体: {EntityType}, Id: {EntityId}", typeof(TEntity).Name, entity.Id);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        Logger.LogDebug("删除实体: {EntityType}, Id: {EntityId}", typeof(TEntity).Name, entity.Id);
        return Task.CompletedTask;
    }

    public virtual async Task<bool> DeleteByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            await DeleteAsync(entity, cancellationToken);
            return true;
        }
        return false;
    }

    public virtual async Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(e => e.Id.Equals(id), cancellationToken);
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await query.CountAsync(cancellationToken);
    }

    /// <summary>
    /// Query by predicate (kept for backward compatibility)
    /// </summary>
    [Obsolete("Consider using Specification pattern instead")]
    public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>>? predicate = null)
    {
        var query = DbSet.AsQueryable();
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        return query;
    }

    /// <summary>
    /// First or default by predicate
    /// </summary>
    public virtual async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Find by predicate
    /// </summary>
    public virtual async Task<IEnumerable<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Apply specification to query
    /// </summary>
    protected virtual IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification)
    {
        return SpecificationEvaluator.GetQuery(DbSet.AsQueryable(), specification);
    }

    // Additional interface methods
    public virtual async Task<IReadOnlyList<TEntity>> GetByIdsAsync(
        IEnumerable<TKey> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        return await DbSet.Where(e => idList.Contains(e.Id))
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<PagedResult<TEntity>> GetPagedAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await DbSet.CountAsync(cancellationToken);
        var items = await DbSet
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<TEntity>(items, totalCount, request.Skip, request.Take);
    }

    public virtual async Task<PagedResult<TEntity>> GetPagedAsync(
        ISpecification<TEntity> specification,
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<TEntity>(items, totalCount, request.Skip, request.Take);
    }
}

