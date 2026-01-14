# Lemoo.UI 改进实施指南

本文档提供了详细的改进实施方案，包含完整的代码示例和实施步骤。

---

## 一、规格模式 (Specification Pattern) 实现

### 1.1 创建规格基础类

**文件路径：** `src/Core/Lemoo.Core.Domain/Specifications/Specification.cs`

```csharp
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Lemoo.Core.Domain.Specifications;

/// <summary>
/// 规格模式基类 - 用于封装查询逻辑
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public abstract class Specification<T>
{
    /// <summary>
    /// 获取筛选条件表达式
    /// </summary>
    [JsonIgnore]
    public abstract Expression<Func<T, bool>> Criteria { get; }

    /// <summary>
    /// 获取包含的导航属性
    /// </summary>
    public List<Expression<Func<T, object>>> Includes { get; } = new();

    /// <summary>
    /// 获取包含的导航属性（字符串形式）
    /// </summary>
    public List<string> IncludeStrings { get; } = new();

    /// <summary>
    /// 获取排序条件
    /// </summary>
    [JsonIgnore]
    public Expression<Func<T, object>>? OrderBy { get; private set; }

    /// <summary>
    /// 获取降序排序条件
    /// </summary>
    [JsonIgnore]
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    /// <summary>
    /// 获取分页跳过数量
    /// </summary>
    public int Skip { get; private set; }

    /// <summary>
    /// 获取分页获取数量
    /// </summary>
    public int Take { get; private set; }

    /// <summary>
    /// 是否启用分页
    /// </summary>
    public bool IsPagingEnabled { get; private set; }

    /// <summary>
    /// 添加包含的导航属性
    /// </summary>
    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    /// <summary>
    /// 添加包含的导航属性（字符串形式）
    /// </summary>
    protected void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    /// <summary>
    /// 应用升序排序
    /// </summary>
    protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    /// <summary>
    /// 应用降序排序
    /// </summary>
    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }

    /// <summary>
    /// 应用分页
    /// </summary>
    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    /// <summary>
    /// 与运算（组合规格）
    /// </summary>
    public Specification<T> And(Specification<T> specification)
    {
        return new AndSpecification<T>(this, specification);
    }

    /// <summary>
    /// 或运算（组合规格）
    /// </summary>
    public Specification<T> Or(Specification<T> specification)
    {
        return new OrSpecification<T>(this, specification);
    }

    /// <summary>
    /// 非运算
    /// </summary>
    public Specification<T> Not()
    {
        return new NotSpecification<T>(this);
    }
}
```

### 1.2 创建组合规格实现

**文件路径：** `src/Core/Lemoo.Core.Domain/Specifications/CompositeSpecifications.cs`

```csharp
using System.Linq.Expressions;

namespace Lemoo.Core.Domain.Specifications;

/// <summary>
/// AND 规格组合
/// </summary>
public sealed class AndSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    public AndSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override Expression<Func<T, bool>> Criteria
    {
        get
        {
            var leftParam = _left.Criteria.Parameters[0];
            var rightParam = _right.Criteria.Parameters[0];

            // 创建参数表达式替换器
            var parameterReplacer = new ParameterReplacer(rightParam, leftParam);

            var rightBody = parameterReplacer.Visit(_right.Criteria.Body);
            var andExpression = Expression.AndAlso(_left.Criteria.Body, rightBody);

            return Expression.Lambda<Func<T, bool>>(andExpression, leftParam);
        }
    }

    private class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : base.VisitParameter(node);
        }
    }
}

/// <summary>
/// OR 规格组合
/// </summary>
public sealed class OrSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    public OrSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override Expression<Func<T, bool>> Criteria
    {
        get
        {
            var leftParam = _left.Criteria.Parameters[0];
            var rightParam = _right.Criteria.Parameters[0];

            var parameterReplacer = new ParameterReplacer(rightParam, leftParam);
            var rightBody = parameterReplacer.Visit(_right.Criteria.Body);
            var orExpression = Expression.OrElse(_left.Criteria.Body, rightBody);

            return Expression.Lambda<Func<T, bool>>(orExpression, leftParam);
        }
    }

    private class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : base.VisitParameter(node);
        }
    }
}

/// <summary>
/// NOT 规格
/// </summary>
public sealed class NotSpecification<T> : Specification<T>
{
    private readonly Specification<T> _specification;

    public NotSpecification(Specification<T> specification)
    {
        _specification = specification;
    }

    public override Expression<Func<T, bool>> Criteria
    {
        get
        {
            var notExpression = Expression.Not(_specification.Criteria);
            return Expression.Lambda<Func<T, bool>>(notExpression, _specification.Criteria.Parameters[0]);
        }
    }
}
```

### 1.3 创建示例规格

**文件路径：** `src/Modules/Lemoo.Modules.TaskManager/Domain/Specifications/TaskSpecifications.cs`

```csharp
using Lemoo.Core.Domain.Specifications;

namespace Lemoo.Modules.TaskManager.Domain.Specifications;

/// <summary>
/// 活跃任务规格
/// </summary>
public sealed class ActiveTasksSpecification : Specification<Task>
{
    public override Expression<Func<Task, bool>> Criteria => t => !t.IsCompleted;

    public ActiveTasksSpecification()
    {
        // 包含关联的分配对象
        AddInclude(t => t.Assignee);
        AddInclude(t => t.Project);
    }
}

/// <summary>
/// 高优先级任务规格
/// </summary>
public sealed class HighPriorityTasksSpecification : Specification<Task>
{
    public override Expression<Func<Task, bool>> Criteria => t => t.Priority >= Priority.High;

    public HighPriorityTasksSpecification()
    {
        ApplyOrderByDescending(t => t.Priority);
        ApplyPaging(0, 50); // 限制返回前50条
    }
}

/// <summary>
/// 用户的待处理任务规格
/// </summary>
public sealed class UserPendingTasksSpecification : Specification<Task>
{
    public override Expression<Func<Task, bool>> Criteria =>
        t => !t.IsCompleted && t.AssigneeId == _userId && t.DueDate <= _maxDueDate;

    private readonly Guid _userId;
    private readonly DateTime _maxDueDate;

    public UserPendingTasksSpecification(Guid userId, DateTime maxDueDate)
    {
        _userId = userId;
        _maxDueDate = maxDueDate;

        AddInclude(t => t.Project);
        ApplyOrderBy(t => t.DueDate);
    }
}

/// <summary>
/// 搜索任务规格
/// </summary>
public sealed class TaskSearchSpecification : Specification<Task>
{
    public override Expression<Func<Task, bool>> Criteria =>
        t => (string.IsNullOrEmpty(_keyword) || t.Title.Contains(_keyword)) &&
             (!_status.HasValue || t.Status == _status.Value) &&
             (!_priority.HasValue || t.Priority == _priority.Value);

    private readonly string _keyword;
    private readonly TaskStatus? _status;
    private readonly Priority? _priority;

    public TaskSearchSpecification(string? keyword = null, TaskStatus? status = null, Priority? priority = null)
    {
        _keyword = keyword ?? string.Empty;
        _status = status;
        _priority = priority;

        AddInclude(t => t.Assignee);
        AddInclude(t => t.Project);
        ApplyOrderByDescending(t => t.CreatedAt);
    }
}
```

### 1.4 扩展仓储接口和实现

**修改文件：** `src/Core/Lemoo.Core.Abstractions/Persistence/IRepository.cs`

```csharp
using Lemoo.Core.Domain.Specifications;

namespace Lemoo.Core.Abstractions.Persistence;

public interface IRepository<TEntity, TKey> : IReadOnlyRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    // 添加规格模式支持
    Task<IReadOnlyList<TEntity>> ListAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<TEntity?> FirstOrDefaultAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default);

    // 添加批量操作
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}
```

**修改文件：** `src/Core/Lemoo.Core.Infrastructure/Persistence/Repository.cs`

```csharp
using Lemoo.Core.Domain.Specifications;

namespace Lemoo.Core.Infrastructure.Persistence;

public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    // ... 现有代码 ...

    public async Task<IReadOnlyList<TEntity>> ListAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await query.CountAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        DbSet.UpdateRange(entities);
        await Task.CompletedTask;
    }

    public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        DbSet.RemoveRange(entities);
        await Task.CompletedTask;
    }

    private IQueryable<TEntity> ApplySpecification(Specification<TEntity> specification)
    {
        var query = DbSet.AsQueryable();

        // 应用筛选条件
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // 应用包含的导航属性
        query = specification.Includes.Aggregate(query,
            (current, include) => current.Include(include));

        // 应用字符串形式的包含
        query = specification.IncludeStrings.Aggregate(query,
            (current, include) => current.Include(include));

        // 应用排序
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        // 应用分页
        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip).Take(specification.Take);
        }

        return query;
    }
}
```

### 1.5 使用示例

```csharp
// 在 Handler 中使用规格模式
public class GetActiveTasksHandler : IQueryHandler<GetActiveTasksQuery, Result<List<TaskDto>>>
{
    private readonly IRepository<Task, Guid> _repository;

    public async Task<Result<List<TaskDto>>> Handle(GetActiveTasksQuery request, CancellationToken cancellationToken)
    {
        // 使用单一规格
        var spec = new ActiveTasksSpecification();
        var tasks = await _repository.ListAsync(spec, cancellationToken);

        // 组合规格
        var combinedSpec = new ActiveTasksSpecification()
            .And(new HighPriorityTasksSpecification());
        var highPriorityActiveTasks = await _repository.ListAsync(combinedSpec, cancellationToken);

        // 复杂搜索规格
        var searchSpec = new TaskSearchSpecification(
            keyword: request.Keyword,
            status: request.Status,
            priority: request.Priority
        );
        var searchResults = await _repository.ListAsync(searchSpec, cancellationToken);

        // 使用分页
        var pagedSpec = new TaskSearchSpecification();
        pagedSpec.ApplyPaging((request.Page - 1) * request.PageSize, request.PageSize);
        var pagedTasks = await _repository.ListAsync(pagedSpec, cancellationToken);

        return Result.Success(tasks.Select(TaskDto.FromEntity).ToList());
    }
}
```

---

## 二、领域事件处理机制

### 2.1 创建事件处理器接口

**文件路径：** `src/Core/Lemoo.Core.Abstractions/Events/IDomainEventHandler.cs`

```csharp
namespace Lemoo.Core.Abstractions.Events;

/// <summary>
/// 领域事件处理器接口
/// </summary>
/// <typeparam name="TEvent">事件类型</typeparam>
public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    /// <summary>
    /// 处理领域事件
    /// </summary>
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}
```

### 2.2 创建事件调度器接口和实现

**文件路径：** `src/Core/Lemoo.Core.Abstractions/Events/IDomainEventDispatcher.cs`

```csharp
namespace Lemoo.Core.Abstractions.Events;

/// <summary>
/// 领域事件调度器接口
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// 调度并发布单个领域事件
    /// </summary>
    Task DispatchAsync(IDomainEvent @event, CancellationToken cancellationToken = default);

    /// <summary>
    /// 调度并发布多个领域事件
    /// </summary>
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default);
}
```

**文件路径：** `src/Core/Lemoo.Core.Infrastructure/Events/DomainEventDispatcher.cs`

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lemoo.Core.Infrastructure.Events;

/// <summary>
/// 领域事件调度器实现
/// </summary>
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(
        IServiceProvider serviceProvider,
        ILogger<DomainEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
    {
        await DispatchAsync(new[] { @event }, cancellationToken);
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default)
    {
        foreach (var @event in events)
        {
            @event.DispatchedAt = DateTime.UtcNow;

            _logger.LogInformation("Dispatching domain event: {EventType}", @event.GetType().Name);

            // 获取事件处理器类型
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(@event.GetType());

            // 获取所有处理器实现
            var handlers = _serviceProvider.GetServices(handlerType);

            if (!handlers.Any())
            {
                _logger.LogWarning("No handlers found for event: {EventType}", @event.GetType().Name);
                continue;
            }

            // 并发执行所有处理器
            var tasks = handlers.Select(handler =>
            {
                var method = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync));
                if (method == null)
                {
                    _logger.LogWarning("Handler {HandlerType} does not have HandleAsync method", handler.GetType().Name);
                    return Task.CompletedTask;
                }

                return (Task)method.Invoke(handler, new object[] { @event, cancellationToken });
            });

            try
            {
                await Task.WhenAll(tasks);
                _logger.LogInformation("Domain event {EventType} processed successfully", @event.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing domain event: {EventType}", @event.GetType().Name);
                throw;
            }
        }
    }
}
```

### 2.3 修改 UnitOfWork 集成事件调度

**文件路径：** `src/Core/Lemoo.Core.Infrastructure/Persistence/UnitOfWork.cs`

```csharp
using Lemoo.Core.Abstractions.Events;

namespace Lemoo.Core.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(
        DbContext dbContext,
        IDomainEventDispatcher eventDispatcher)
    {
        _dbContext = dbContext;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 在保存前处理领域事件
        await DispatchDomainEventsAsync(cancellationToken);

        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _transaction?.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        // 查找所有有领域事件的聚合根
        var aggregates = _dbContext.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        // 获取所有领域事件
        var domainEvents = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        // 清除聚合根中的事件
        aggregates.ForEach(a => a.ClearDomainEvents());

        // 调度事件
        await _eventDispatcher.DispatchAsync(domainEvents, cancellationToken);
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _dbContext.Dispose();
    }
}
```

### 2.4 注册事件调度器服务

**文件路径：** `src/Core/Lemoo.Core.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ... 现有注册 ...

        // 注册领域事件调度器
        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();

        // 自动注册所有领域事件处理器
        services.Scan(scan => scan
            .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }
}
```

### 2.5 创建示例事件和处理器

**文件路径：** `src/Modules/Lemoo.Modules.TaskManager/Domain/Events/TaskCompletedEvent.cs`

```csharp
namespace Lemoo.Modules.TaskManager.Domain.Events;

/// <summary>
/// 任务已完成领域事件
/// </summary>
public sealed class TaskCompletedEvent : IDomainEvent
{
    public Guid TaskId { get; }
    public string TaskTitle { get; }
    public Guid AssigneeId { get; }
    public DateTime CompletedAt { get; }

    public TaskCompletedEvent(Guid taskId, string taskTitle, Guid assigneeId, DateTime completedAt)
    {
        TaskId = taskId;
        TaskTitle = taskTitle;
        AssigneeId = assigneeId;
        CompletedAt = completedAt;
    }
}
```

**文件路径：** `src/Modules/Lemoo.Modules.TaskManager/Application/Events/TaskCompletedEventHandler.cs`

```csharp
using Lemoo.Core.Abstractions.Events;
using Lemoo.Modules.TaskManager.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Lemoo.Modules.TaskManager.Application.Events;

/// <summary>
/// 任务完成事件处理器 - 发送通知
/// </summary>
public sealed class TaskCompletedNotificationHandler : IDomainEventHandler<TaskCompletedEvent>
{
    private readonly ILogger<TaskCompletedNotificationHandler> _logger;
    private readonly INotificationService _notificationService;

    public TaskCompletedNotificationHandler(
        ILogger<TaskCompletedNotificationHandler> logger,
        INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task HandleAsync(TaskCompletedEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Task {TaskId} completed by user {AssigneeId}",
            @event.TaskId,
            @event.AssigneeId);

        // 发送通知
        await _notificationService.SendNotificationAsync(
            @event.AssigneeId,
            $"任务 '{@event.TaskTitle}' 已完成",
            cancellationToken);
    }
}

/// <summary>
/// 任务完成事件处理器 - 更新统计
/// </summary>
public sealed class TaskCompletedStatisticsHandler : IDomainEventHandler<TaskCompletedEvent>
{
    private readonly ILogger<TaskCompletedStatisticsHandler> _logger;
    private readonly IStatisticsService _statisticsService;

    public TaskCompletedStatisticsHandler(
        ILogger<TaskCompletedStatisticsHandler> logger,
        IStatisticsService statisticsService)
    {
        _logger = logger;
        _statisticsService = statisticsService;
    }

    public async Task HandleAsync(TaskCompletedEvent @event, CancellationToken cancellationToken)
    {
        // 更新用户完成统计
        await _statisticsService.IncrementCompletedTasksAsync(
            @event.AssigneeId,
            cancellationToken);

        _logger.LogInformation(
            "Updated completion statistics for user {UserId}",
            @event.AssigneeId);
    }
}
```

### 2.6 在聚合根中使用领域事件

**文件路径：** `src/Modules/Lemoo.Modules.TaskManager/Domain/Aggregates/Task.cs`

```csharp
public class Task : AggregateRoot<Guid>
{
    // ... 现有属性 ...

    public void Complete(string completedBy)
    {
        if (IsCompleted)
        {
            throw new InvalidOperationException("Task is already completed");
        }

        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
        CompletedBy = completedBy;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = completedBy;

        // 添加领域事件
        AddDomainEvent(new TaskCompletedEvent(
            Id,
            Title,
            AssigneeId,
            CompletedAt.Value));
    }
}
```

---

## 三、软删除支持

### 3.1 修改实体基类

**文件路径：** `src/Core/Lemoo.Core.Domain/Entities/EntityBase.cs`

```csharp
namespace Lemoo.Core.Domain.Entities;

/// <summary>
/// 软删除接口
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }
    string? DeletedBy { get; }
}

/// <summary>
/// 实体基类
/// </summary>
public abstract class EntityBase<TKey> : IEntity<TKey>, ISoftDeletable
{
    public TKey Id { get; protected set; } = default!;
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public string CreatedBy { get; protected set; } = string.Empty;
    public string? UpdatedBy { get; protected set; }

    // 软删除支持
    public bool IsDeleted { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }
    public string? DeletedBy { get; protected set; }

    /// <summary>
    /// 软删除实体
    /// </summary>
    public void SoftDelete(string deletedBy)
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException("Entity is already deleted");
        }

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deletedBy;
    }

    /// <summary>
    /// 恢复实体
    /// </summary>
    public void Restore(string restoredBy)
    {
        if (!IsDeleted)
        {
            throw new InvalidOperationException("Entity is not deleted");
        }

        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = restoredBy;
    }
}
```

### 3.2 配置全局查询过滤器

**文件路径：** `src/Core/Lemoo.Core.Infrastructure/Persistence/ApplicationDbContext.cs`

```csharp
using Lemoo.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lemoo.Core.Infrastructure.Persistence;

public abstract class ApplicationDbContext : DbContext
{
    protected ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 为所有实现了 ISoftDeletable 的实体配置全局查询过滤器
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(FilterSoftDeleted(entityType.ClrType));
            }
        }
    }

    private static LambdaExpression FilterSoftDeleted(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
        var condition = Expression.Equal(property, Expression.Constant(false));
        return Expression.Lambda(condition, parameter);
    }

    /// <summary>
    /// 获取所有实体（包括已软删除的）
    /// </summary>
    public IQueryable<TEntity> GetAllIncludingDeleted<TEntity>()
        where TEntity : class, ISoftDeletable
    {
        return Set<TEntity>().IgnoreQueryFilters();
    }
}
```

### 3.3 修改仓储删除方法

**文件路径：** `src/Core/Lemoo.Core.Infrastructure/Persistence/Repository.cs`

```csharp
public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        // 如果实体支持软删除，使用软删除
        if (entity is ISoftDeletable softDeletable)
        {
            var currentUser = _currentUserService.UserId; // 需要注入当前用户服务
            softDeletable.SoftDelete(currentUser);
        }
        else
        {
            // 否则硬删除
            DbSet.Remove(entity);
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// 永久删除（即使支持软删除）
    /// </summary>
    public async Task HardDeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        await Task.CompletedTask;
    }
}
```

### 3.4 添加恢复操作接口

**文件路径：** `src/Core/Lemoo.Core.Abstractions/Persistence/IRepository.cs`

```csharp
public interface IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    // ... 现有方法 ...

    /// <summary>
    /// 恢复已软删除的实体
    /// </summary>
    Task RestoreAsync(TKey id, CancellationToken cancellationToken = default);
}
```

**实现：**
```csharp
public async Task RestoreAsync(TKey id, CancellationToken cancellationToken = default)
{
    var entity = await DbSet
        .IgnoreQueryFilters()
        .FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);

    if (entity == null)
    {
        throw new NotFoundException($"Entity with id {id} not found");
    }

    if (entity is ISoftDeletable softDeletable)
    {
        var currentUser = _currentUserService.UserId;
        softDeletable.Restore(currentUser);
    }
}
```

---

## 四、批量操作支持

### 4.1 扩展仓储接口

**文件路径：** `src/Core/Lemoo.Core.Abstractions/Persistence/IRepository.cs`

```csharp
public interface IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    // ... 现有方法 ...

    /// <summary>
    /// 批量添加实体
    /// </summary>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量更新实体
    /// </summary>
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除实体
    /// </summary>
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除（按ID）
    /// </summary>
    Task DeleteRangeAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);
}
```

### 4.2 实现批量操作

**文件路径：** `src/Core/Lemoo.Core.Infrastructure/Persistence/Repository.cs`

```csharp
public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        DbSet.UpdateRange(entities);
        await Task.CompletedTask;
    }

    public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        // 如果支持软删除
        if (typeof(ISoftDeletable).IsAssignableFrom(typeof(TEntity)))
        {
            var currentUser = _currentUserService.UserId;
            foreach (var entity in entities)
            {
                if (entity is ISoftDeletable softDeletable)
                {
                    softDeletable.SoftDelete(currentUser);
                }
            }
        }
        else
        {
            DbSet.RemoveRange(entities);
        }

        await Task.CompletedTask;
    }

    public async Task DeleteRangeAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
    {
        var entities = await DbSet
            .Where(e => ids.Contains(e.Id))
            .ToListAsync(cancellationToken);

        await DeleteRangeAsync(entities, cancellationToken);
    }
}
```

### 4.3 使用 EF Core 7+ 的批量操作优化

对于大规模批量操作，可以使用 EF Core 7+ 的 `ExecuteUpdate` 和 `ExecuteDelete`：

**文件路径：** `src/Core/Lemoo.Core.Infrastructure/Persistence/Repository.cs`

```csharp
public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    /// <summary>
    /// 批量更新（使用 ExecuteUpdate，直接在数据库执行）
    /// </summary>
    public async Task<int> BulkUpdateAsync(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<SetPropertyCalls<TEntity>, TEntity>> setPropertyCalls,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(predicate)
            .ExecuteUpdateAsync(setPropertyCalls, cancellationToken);
    }

    /// <summary>
    /// 批量删除（使用 ExecuteDelete，直接在数据库执行）
    /// </summary>
    public async Task<int> BulkDeleteAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(predicate)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
```

**使用示例：**
```csharp
// 批量更新所有过期任务的状态
await _repository.BulkUpdateAsync(
    predicate: t => t.DueDate < DateTime.UtcNow && !t.IsCompleted,
    setPropertyCalls: setter => setter.SetProperty(t => t.Status, TaskStatus.Overdue)
);

// 批量删除所有超过30天的已完成任务
await _repository.BulkDeleteAsync(
    predicate: t => t.IsCompleted && t.CompletedAt < DateTime.UtcNow.AddDays(-30)
);
```

---

## 五、全局异常处理增强

### 5.1 创建问题详情模型

**文件路径：** `src/Core/Lemoo.Application/Common/Models/ProblemDetails.cs`

```csharp
using System.Text.Json.Serialization;

namespace Lemoo.Core.Application.Common.Models;

/// <summary>
/// RFC 7807 问题详情
/// </summary>
public class ProblemDetails
{
    /// <summary>
    /// 问题类型的 URI 引用
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 问题标题
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// HTTP 状态码
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>
    /// 问题详情
    /// </summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    /// <summary>
    /// 问题实例的 URI
    /// </summary>
    [JsonPropertyName("instance")]
    public string? Instance { get; set; }

    /// <summary>
    /// 错误代码
    /// </summary>
    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    /// <summary>
    /// 验证错误
    /// </summary>
    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 追踪 ID
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }
}
```

### 5.2 创建异常处理器

**文件路径：** `src/Core/Lemoo.Application/Exceptions/IExceptionHandler.cs`

```csharp
namespace Lemoo.Core.Application.Exceptions;

/// <summary>
/// 异常处理器接口
/// </summary>
public interface IExceptionHandler
{
    /// <summary>
    /// 处理异常并返回问题详情
    /// </summary>
    ValueTask<ProblemDetails?> HandleAsync(Exception exception, CancellationToken cancellationToken = default);
}
```

### 5.3 实现具体异常处理器

**文件路径：** `src/Core/Lemoo.Application/Exceptions/Handlers/`

```csharp
// ValidationExceptionHandler.cs
public class ValidationExceptionHandler : IExceptionHandler
{
    public ValueTask<ProblemDetails?> HandleAsync(Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ValidationException validationEx)
        {
            var problem = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "验证失败",
                Status = 400,
                ErrorCode = "VALIDATION_ERROR",
                Errors = validationEx.Errors
            };
            return ValueTask.FromResult<ProblemDetails?>(problem);
        }
        return ValueTask.FromResult<ProblemDetails?>(null);
    }
}

// NotFoundExceptionHandler.cs
public class NotFoundExceptionHandler : IExceptionHandler
{
    public ValueTask<ProblemDetails?> HandleAsync(Exception exception, CancellationToken cancellationToken)
    {
        if (exception is NotFoundException notFoundEx)
        {
            var problem = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "资源未找到",
                Status = 404,
                Detail = notFoundEx.Message,
                ErrorCode = "NOT_FOUND"
            };
            return ValueTask.FromResult<ProblemDetails?>(problem);
        }
        return ValueTask.FromResult<ProblemDetails?>(null);
    }
}

// BusinessExceptionHandler.cs
public class BusinessExceptionHandler : IExceptionHandler
{
    public ValueTask<ProblemDetails?> HandleAsync(Exception exception, CancellationToken cancellationToken)
    {
        if (exception is BusinessException businessEx)
        {
            var problem = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                Title = "业务错误",
                Status = 422,
                Detail = businessEx.Message,
                ErrorCode = "BUSINESS_ERROR"
            };
            return ValueTask.FromResult<ProblemDetails?>(problem);
        }
        return ValueTask.FromResult<ProblemDetails?>(null);
    }
}

// GlobalExceptionHandler.cs
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public ValueTask<ProblemDetails?> HandleAsync(Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        var problem = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "服务器错误",
            Status = 500,
            Detail = _environment.IsDevelopment() ? exception.Message : "发生内部服务器错误",
            ErrorCode = "INTERNAL_SERVER_ERROR"
        };

        return ValueTask.FromResult<ProblemDetails?>(problem);
    }
}
```

### 5.4 创建全局异常处理中间件

**文件路径：** `src/Core/Lemoo.Api/Middleware/GlobalExceptionHandlerMiddleware.cs`

```csharp
using System.Net;
using System.Text.Json;
using Lemoo.Core.Application.Exceptions;
using Lemoo.Core.Application.Common.Models;

namespace Lemoo.Api.Middleware;

/// <summary>
/// 全局异常处理中间件
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IEnumerable<IExceptionHandler> _handlers;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        IEnumerable<IExceptionHandler> handlers,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _handlers = handlers;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // 尝试使用特定处理器处理
        ProblemDetails? problem = null;
        foreach (var handler in _handlers)
        {
            problem = await handler.HandleAsync(exception);
            if (problem != null) break;
        }

        // 如果没有特定处理器，使用全局处理器
        if (problem == null)
        {
            var globalHandler = _handlers.OfType<GlobalExceptionHandler>().FirstOrDefault();
            problem = await globalHandler?.HandleAsync(exception);
        }

        // 设置响应
        context.Response.StatusCode = problem?.Status ?? (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/problem+json";

        // 添加上下文信息
        problem!.Instance = context.Request.Path;
        problem.TraceId = context.TraceIdentifier;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };

        var json = JsonSerializer.Serialize(problem, options);
        await context.Response.WriteAsync(json);
    }
}

// 扩展方法
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
```

### 5.5 注册异常处理器

**文件路径：** `src/Core/Lemoo.Api/Program.cs`

```csharp
// 注册异常处理器
services.AddSingleton<IExceptionHandler, ValidationExceptionHandler>();
services.AddSingleton<IExceptionHandler, NotFoundExceptionHandler>();
services.AddSingleton<IExceptionHandler, BusinessExceptionHandler>();
services.AddSingleton<IExceptionHandler, GlobalExceptionHandler>();

// 使用中间件
app.UseGlobalExceptionHandler();
```

---

## 六、性能监控

### 6.1 创建性能监控管道

**文件路径：** `src/Core/Lemoo.Application/Behaviors/PerformanceBehavior.cs`

```csharp
using System.Diagnostics;
using Lemoo.Core.Abstractions.CQRS;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lemoo.Core.Application.Behaviors;

/// <summary>
/// 性能监控管道行为
/// </summary>
public class PerformanceBehavior<TRequest, TResponse> : MediatR.IPipelineBehavior<TRequest, TResponse>
    where TRequest : MediatR.IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly Stopwatch _timer;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
        _timer = new Stopwatch();
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();

        try
        {
            var response = await next();
            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;
            var requestName = typeof(TRequest).Name;

            if (elapsedMilliseconds > 1000) // 超过1秒记录警告
            {
                _logger.LogWarning(
                    "Long Running Request: {Name} ({ElapsedMilliseconds}ms) {@Request}",
                    requestName, elapsedMilliseconds, request);
            }
            else
            {
                _logger.LogInformation(
                    "Request: {Name} ({ElapsedMilliseconds}ms)",
                    requestName, elapsedMilliseconds);
            }

            return response;
        }
        catch (Exception ex)
        {
            _timer.Stop();

            _logger.LogError(
                ex,
                "Request Error: {Name} ({ElapsedMilliseconds}ms) {@Request}",
                typeof(TRequest).Name, _timer.ElapsedMilliseconds, request);

            throw;
        }
    }
}
```

### 6.2 创建性能指标收集器

**文件路径：** `src/Core/Lemoo.Infrastructure/Metrics/PerformanceMetrics.cs`

```csharp
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Lemoo.Core.Infrastructure.Metrics;

/// <summary>
/// 性能指标收集器
/// </summary>
public class PerformanceMetrics
{
    private readonly Meter _meter;
    private readonly Counter<long> _requestCounter;
    private readonly Histogram<double> _requestDuration;
    private readonly Counter<long> _errorCounter;

    public PerformanceMetrics(IMeterFactory meterFactory)
    {
        _meter = meterFactory.Create("Lemoo.Application");

        _requestCounter = _meter.CreateCounter<long>(
            "lemoo.requests.count",
            description: "请求数量");

        _requestDuration = _meter.CreateHistogram<double>(
            "lemoo.requests.duration",
            unit: "ms",
            description: "请求持续时间");

        _errorCounter = _meter.CreateCounter<long>(
            "lemoo.requests.errors",
            description: "请求数量");
    }

    public void RecordRequest(string requestName, double duration, bool success = true)
    {
        var tags = new TagList { new("request", requestName) };

        _requestCounter.Add(1, tags);
        _requestDuration.Record(duration, tags);

        if (!success)
        {
            _errorCounter.Add(1, tags);
        }
    }
}
```

### 6.3 集成 OpenTelemetry

**添加 NuGet 包：**
```bash
dotnet add package OpenTelemetry
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Exporter.Prometheus
dotnet add package OpenTelemetry.Exporter.Console
```

**配置文件：** `src/Core/Lemoo.Api/Program.cs`

```csharp
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService("Lemoo.Api"))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddMeter("Lemoo.Application")
        .AddPrometheusExporter())
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddSource("Lemoo.Application")
        .AddConsoleExporter());

// 添加 Prometheus 端点
app.MapPrometheusScrapingEndpoint();
```

---

## 七、实施清单

### 阶段一：规格模式（第1周）

- [ ] 创建 `Specification<T>` 基类
- [ ] 实现组合规格（AND、OR、NOT）
- [ ] 扩展 `IRepository` 接口
- [ ] 更新 `Repository` 实现
- [ ] 创建示例规格
- [ ] 更新相关查询使用规格模式
- [ ] 编写单元测试

### 阶段二：领域事件（第2周）

- [ ] 创建 `IDomainEventHandler` 接口
- [ ] 实现 `IDomainEventDispatcher`
- [ ] 更新 `UnitOfWork` 集成事件调度
- [ ] 注册事件处理器服务
- [ ] 创建示例事件和处理器
- [ ] 更新聚合根使用领域事件
- [ ] 编写集成测试

### 阶段三：软删除和批量操作（第3周）

- [ ] 更新 `EntityBase` 添加软删除支持
- [ ] 配置全局查询过滤器
- [ ] 更新 `Repository` 删除方法
- [ ] 添加恢复操作
- [ ] 实现批量操作方法
- [ ] 优化批量操作性能
- [ ] 编写测试

### 阶段四：异常处理和监控（第4周）

- [ ] 创建 `ProblemDetails` 模型
- [ ] 实现异常处理器
- [ ] 创建全局异常处理中间件
- [ ] 注册异常处理器
- [ ] 创建性能监控管道
- [ ] 集成 OpenTelemetry
- [ ] 配置 Prometheus 指标

---

## 八、测试策略

### 8.1 单元测试示例

```csharp
public class ActiveTasksSpecificationTests
{
    [Fact]
    public void Criteria_ShouldReturnOnlyActiveTasks()
    {
        // Arrange
        var spec = new ActiveTasksSpecification();

        // Act
        var task1 = new Task { IsCompleted = false };
        var task2 = new Task { IsCompleted = true };

        var predicate = spec.Criteria.Compile();

        // Assert
        Assert.True(predicate(task1));
        Assert.False(predicate(task2));
    }
}
```

### 8.2 集成测试示例

```csharp
public class DomainEventIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    [Fact]
    public async Task CompletingTask_ShouldTriggerNotification()
    {
        // Arrange
        var task = await CreateTestTask();
        var notificationMock = _fixture.GetMock<INotificationService>();

        // Act
        task.Complete("test-user");
        await _unitOfWork.SaveChangesAsync();

        // Assert
        notificationMock.Verify(
            x => x.SendNotificationAsync(
                It.IsAny<Guid>(),
                It.Is<string>(s => s.Contains("已完成")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
```

---

## 九、性能基准测试

使用 BenchmarkDotNet 建立性能基准：

```csharp
[MemoryDiagnoser]
public class RepositoryBenchmarks
{
    private IRepository<Task, Guid> _repository = null!;
    private List<Task> _tasks = null!;

    [GlobalSetup]
    public void Setup()
    {
        // 初始化测试数据
    }

    [Benchmark]
    public async Task<List<Task>> ListAll()
    {
        return await _repository.ListAllAsync();
    }

    [Benchmark]
    public async Task<List<Task>> ListWithSpecification()
    {
        return await _repository.ListAsync(new ActiveTasksSpecification());
    }

    [Benchmark]
    public async Task<Task?> GetById()
    {
        return await _repository.GetByIdAsync(Guid.NewGuid());
    }
}
```

---

本文档提供了详细的改进实施指南，每个改进点都包含完整的代码示例和实施步骤。建议按阶段逐步实施，每个阶段完成后进行充分的测试验证。
