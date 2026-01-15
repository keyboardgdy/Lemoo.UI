using Lemoo.Core.Abstractions.Domain;
using Lemoo.Core.Abstractions.Persistence;
using Lemoo.Core.Abstractions.Specifications;
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
    protected readonly ISpecificationEvaluator SpecificationEvaluator;

    public ReadOnlyRepository(DbContext dbContext, ILogger<ReadOnlyRepository<TEntity, TKey>> logger)
        : this(dbContext, logger, Lemoo.Core.Abstractions.Specifications.SpecificationEvaluator.Instance)
    {
    }

    public ReadOnlyRepository(
        DbContext dbContext,
        ILogger<ReadOnlyRepository<TEntity, TKey>> logger,
        ISpecificationEvaluator specificationEvaluator)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<TEntity>();
        Logger = logger;
        SpecificationEvaluator = specificationEvaluator;
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

    public virtual async Task<int> CountAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await query.CountAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetByIdsAsync(
        IEnumerable<TKey> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        return await DbSet.Where(e => idList.Contains(e.Id))
            .AsNoTracking()
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
            .AsNoTracking()
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

    protected virtual IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification)
    {
        return SpecificationEvaluator.GetQuery(DbSet.AsNoTracking(), specification);
    }
}

