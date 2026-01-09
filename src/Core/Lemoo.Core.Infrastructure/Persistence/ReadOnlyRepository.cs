using Lemoo.Core.Abstractions.Domain;
using Lemoo.Core.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Lemoo.Core.Infrastructure.Persistence;

/// <summary>
/// 只读仓储实现 - 基于 EF Core
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TKey">主键类型</typeparam>
public class ReadOnlyRepository<TEntity, TKey> : IReadOnlyRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    protected readonly DbContext DbContext;
    protected readonly DbSet<TEntity> DbSet;
    protected readonly ILogger<ReadOnlyRepository<TEntity, TKey>> Logger;

    public ReadOnlyRepository(DbContext dbContext, ILogger<ReadOnlyRepository<TEntity, TKey>> logger)
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
        return await DbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(e => e.Id.Equals(id), cancellationToken);
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(cancellationToken);
    }

    public virtual IQueryable<TEntity> Query()
    {
        return DbSet.AsNoTracking();
    }

    /// <summary>
    /// 根据条件查询
    /// </summary>
    public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate)
    {
        return DbSet.AsNoTracking().Where(predicate);
    }

    /// <summary>
    /// 根据条件查找第一个
    /// </summary>
    public virtual async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// 根据条件查找所有
    /// </summary>
    public virtual async Task<IEnumerable<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }
}

