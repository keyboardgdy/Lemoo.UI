using Lemoo.Core.Abstractions.Domain;
using Lemoo.Core.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Lemoo.Core.Infrastructure.Persistence;

/// <summary>
/// 基础仓储实现 - 基于 EF Core
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TKey">主键类型</typeparam>
public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    protected readonly DbContext DbContext;
    protected readonly DbSet<TEntity> DbSet;
    protected readonly ILogger<Repository<TEntity, TKey>> Logger;

    public Repository(DbContext dbContext, ILogger<Repository<TEntity, TKey>> logger)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<TEntity>();
        Logger = logger;
    }

    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
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

    public virtual async Task DeleteByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            await DeleteAsync(entity, cancellationToken);
        }
    }

    public virtual async Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(e => e.Id.Equals(id), cancellationToken);
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(cancellationToken);
    }

    /// <summary>
    /// 根据条件查询
    /// </summary>
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
    /// 根据条件查找第一个
    /// </summary>
    public virtual async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// 根据条件查找所有
    /// </summary>
    public virtual async Task<IEnumerable<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }
}

