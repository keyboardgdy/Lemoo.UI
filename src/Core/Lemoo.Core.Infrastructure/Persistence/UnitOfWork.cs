using Lemoo.Core.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Lemoo.Core.Infrastructure.Persistence;

/// <summary>
/// 工作单元实现 - 基于 DbContext
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;
    private readonly ILogger<UnitOfWork> _logger;
    private IDbContextTransaction? _transaction;
    
    public UnitOfWork(DbContext dbContext, ILogger<UnitOfWork> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            _logger.LogWarning("事务已存在，跳过开始新事务");
            return;
        }
        
        _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        _logger.LogDebug("事务已开始");
    }
    
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            _logger.LogWarning("没有活动的事务，跳过提交");
            return;
        }
        
        try
        {
            await _transaction.CommitAsync(cancellationToken);
            _logger.LogDebug("事务已提交");
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            _logger.LogWarning("没有活动的事务，跳过回滚");
            return;
        }
        
        try
        {
            await _transaction.RollbackAsync(cancellationToken);
            _logger.LogDebug("事务已回滚");
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    
    public void Dispose()
    {
        _transaction?.Dispose();
        _dbContext?.Dispose();
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        if (_dbContext != null)
        {
            await _dbContext.DisposeAsync();
        }
    }

    /// <summary>
    /// Gets all currently tracked entities for domain event dispatching
    /// </summary>
    public IEnumerable<object> GetTrackedEntities()
    {
        return _dbContext.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added ||
                       e.State == EntityState.Modified ||
                       e.State == EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();
    }
}

