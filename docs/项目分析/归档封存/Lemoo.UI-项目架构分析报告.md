# Lemoo.UI 项目架构分析报告

## 项目概述

**项目名称：** Lemoo.UI
**框架版本：** .NET 10 (LTS)
**语言版本：** C# 13
**架构模式：** DDD (领域驱动设计) + CQRS + 模块化架构
**分析日期：** 2026-01-14

---

## 一、整体架构设计

### 1.1 架构分层

Lemoo.UI 采用严格的分层架构，遵循依赖倒置和洋葱架构原则：

```
┌─────────────────────────────────────────────────────────┐
│                   Presentation Layer                     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │   Lemoo.Api  │  │ Lemoo.Host   │  │  Lemoo.UI    │  │
│  │  (Web API)   │  │   (WPF)      │  │ (Components) │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│                    Modules Layer                        │
│  ┌──────────────────────────────────────────────────┐  │
│  │         Lemoo.Modules.Abstractions               │  │
│  │         Lemoo.Modules.TaskManager (示例)          │  │
│  └──────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│                    Application Layer                    │
│              Lemoo.Core.Application                     │
│  • CQRS Handlers    • Pipeline Behaviors                │
│  • Result Pattern   • Validators                       │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│                      Domain Layer                       │
│                Lemoo.Core.Domain                        │
│  • Entities        • Aggregates      • Value Objects   │
│  • Domain Events   • Domain Services                   │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│                 Infrastructure Layer                    │
│             Lemoo.Core.Infrastructure                   │
│  • EF Core         • Caching        • Logging          │
│  • File Storage    • Configuration  • Services         │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│                  Abstractions Layer                     │
│              Lemoo.Core.Abstractions                    │
│  • Core Interfaces • Domain Contracts  • CQRS I/F       │
└─────────────────────────────────────────────────────────┘
```

### 1.2 模块化设计

项目采用高度模块化的设计，通过 `IModule` 接口实现模块的可插拔架构：

**模块生命周期：**
```
发现 → 验证 → 注册 → 配置 → 启动 → 运行 → 停止
```

**模块基类 (`ModuleBase`) 提供的扩展点：**
- `PreConfigureServices` - 服务预配置
- `ConfigureServices` - 服务注册
- `PostConfigureServices` - 服务后配置
- `ConfigureDbContext` - 数据库配置
- `OnApplicationStarting/Started/Stopping/Stopped` - 生命周期钩子

---

## 二、核心架构组件分析

### 2.1 领域驱动设计 (DDD) 实现

#### 2.1.1 实体基类

```csharp
// 位置: src/Core/Lemoo.Core.Domain/Entities/EntityBase.cs
public abstract class EntityBase<TKey> : IEntity<TKey>
{
    public TKey Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public string CreatedBy { get; protected set; }
    public string? UpdatedBy { get; protected set; }
}
```

**设计评价：**
- ✅ 提供了标准的审计字段
- ✅ 使用 `protected set` 确保封装性
- ⚠️ 建议：添加软删除支持 (`IsDeleted`)

#### 2.1.2 聚合根基类

```csharp
// 位置: src/Core/Lemoo.Core.Domain/Aggregates/AggregateRoot.cs
public abstract class AggregateRoot<TKey> : EntityBase<TKey>
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent) { ... }
    protected void ClearDomainEvents() { ... }
}
```

**设计评价：**
- ✅ 完整实现领域事件模式
- ✅ 只读暴露确保事件安全
- ✅ 提供清除事件的机制

#### 2.1.3 值对象基类

```csharp
// 位置: src/Core/Lemoo.Core.Domain/ValueObjects/ValueObjectBase.cs
public abstract class ValueObjectBase : IEquatable<ValueObjectBase>
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj) { ... }
    public override int GetHashCode() { ... }
}
```

**设计评价：**
- ✅ 实现了值对象的语义相等性
- ✅ 使用组件式比较确保正确性

### 2.2 CQRS 实现分析

#### 2.2.1 命令查询分离

```csharp
// 命令接口
public interface ICommand : IRequest<Result> { }
public interface ICommand<TResponse> : IRequest<TResponse> { }

// 查询接口
public interface IQuery<TResponse> : IRequest<TResponse> { }
```

**设计评价：**
- ✅ 清晰的读写分离
- ✅ 统一使用 `Result<T>` 模式
- ✅ 与 MediatR 无缝集成

#### 2.2.2 管道行为

项目实现了四个核心管道行为：

**1. 验证管道 (`ValidationBehavior`)**
```csharp
// 位置: src/Core/Lemoo.Core.Application/Behaviors/ValidationBehavior.cs
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next)
    {
        // 使用 FluentValidation 自动验证请求
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // 聚合验证错误并抛出异常
    }
}
```

**2. 事务管道 (`TransactionBehavior`)**
- 自动管理数据库事务
- 支持事务回滚

**3. 日志管道 (`LoggingBehavior`)**
- 记录请求和响应
- 性能监控

**4. 缓存管道 (`CacheBehavior`)**
- 响应缓存
- 可配置过期策略

**设计评价：**
- ✅ 关注点分离良好
- ✅ 管道可组合
- ⚠️ 建议：添加限流管道、重试管道

### 2.3 结果模式 (Result Pattern)

```csharp
// 位置: src/Core/Lemoo.Core.Application/Common/Result.cs
public class Result
{
    public bool IsSuccess { get; protected set; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; protected set; }
    public IReadOnlyList<string> Errors { get; protected set; }

    // 函数式操作
    public Result OnSuccess(Action action) { ... }
    public Result OnFailure(Action<string?> action) { ... }
    public TResult Match<TResult>(Func<TResult> onSuccess, Func<string?, TResult> onFailure) { ... }
}

public class Result<T> : Result
{
    public T? Data { get; private set; }

    // 函数式组合
    public Result<TResult> Map<TResult>(Func<T, TResult> mapper) { ... }
    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder) { ... }
}
```

**设计评价：**
- ✅ 类型安全的错误处理
- ✅ 支持函数式编程风格
- ✅ 链式操作支持
- ✅ 模式匹配能力

**函数式编程特性：**
- `Map` - 转换成功值
- `Bind` - 链式 Result 操作
- `Match` - 模式匹配
- `OnSuccess/OnFailure` - 副作用处理
- `ValueOr` - 提供默认值

---

## 三、基础设施层分析

### 3.1 数据访问

#### 3.1.1 仓储模式

```csharp
// 位置: src/Core/Lemoo.Core.Infrastructure/Persistence/Repository.cs
public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    protected readonly DbContext DbContext;
    protected readonly DbSet<TEntity> DbSet;

    public virtual async Task<TEntity> AddAsync(TEntity entity) { ... }
    public virtual async Task UpdateAsync(TEntity entity) { ... }
    public virtual async Task DeleteAsync(TEntity entity) { ... }
    public virtual async Task<TEntity?> GetByIdAsync(TKey id) { ... }
}
```

**设计评价：**
- ✅ 标准仓储模式实现
- ✅ 支持异步操作
- ⚠️ 建议：添加批量操作支持
- ⚠️ 建议：添加规格模式 (Specification Pattern)

#### 3.1.2 工作单元模式

```csharp
// 位置: src/Core/Lemoo.Core.Infrastructure/Persistence/UnitOfWork.cs
public class UnitOfWork : IUnitOfWork
{
    public async Task BeginTransactionAsync() { ... }
    public async Task CommitTransactionAsync() { ... }
    public async Task RollbackTransactionAsync() { ... }
    public async Task<int> SaveChangesAsync() { ... }
}
```

**设计评价：**
- ✅ 完整的事务管理
- ✅ 支持分布式事务（可扩展）

### 3.2 缓存服务

项目实现了两种缓存策略：

**1. 内存缓存 (`MemoryCacheService`)**
- 基于 `IMemoryCache`
- 适合单机部署
- 支持滑动/绝对过期

**2. Redis 缓存 (`RedisCacheService`)**
- 基于 StackExchange.Redis
- 适合分布式部署
- 支持缓存同步

**设计评价：**
- ✅ 策略模式应用
- ✅ 配置驱动的缓存选择
- ✅ 统一的缓存接口

### 3.3 配置系统

```json
// appsettings.json 结构
{
  "Lemoo": {
    "Mode": "Api",
    "Modules": {
      "Enabled": ["*"],
      "Path": "./Modules",
      "AutoLoad": true,
      "HotReload": false
    },
    "Database": {
      "Provider": "SqlServer",
      "ConnectionStrings": { ... }
    },
    "Caching": {
      "Provider": "Memory",
      "DefaultExpiration": "00:05:00"
    }
  }
}
```

**设计评价：**
- ✅ 集中式配置管理
- ✅ 环境感知
- ✅ 支持配置热更新
- ⚠️ 建议：添加配置验证

---

## 四、UI层分析

### 4.1 WPF 实现

**位置：** `src/UI/Lemoo.UI.WPF`

**关键特性：**
- MVVM 模式 (CommunityToolkit.Mvvm)
- 主题系统 (亮色/暗色)
- 页面导航系统
- 依赖注入集成

```csharp
// App.xaml.cs 核心初始化
private void OnStartup(object sender, StartupEventArgs e)
{
    // 初始化主题系统
    ThemeManager.Initialize();

    // 配置依赖注入
    var services = new ServiceCollection();

    // 注册页面和 ViewModel
    RegisterPages(services);

    _serviceProvider = services.BuildServiceProvider();
}
```

**设计评价：**
- ✅ 现代 MVVM 实现
- ✅ 主题切换支持
- ✅ 导航系统完善
- ⚠️ 建议：添加 View 定位服务

### 4.2 Web API 实现

**位置：** `src/Hosts/Lemoo.Api`

**中间件管道：**
```
RequestId → ExceptionHandler → HTTPS → CORS → Authorization → Controllers
```

**设计评价：**
- ✅ RESTful API 设计
- ✅ Swagger 集成
- ✅ 全局异常处理
- ✅ 请求追踪

---

## 五、SOLID 原则评估

### 5.1 单一职责原则 (SRP)

| 组件 | 评分 | 说明 |
|------|------|------|
| `EntityBase` | ✅ 优秀 | 仅负责实体的基本属性 |
| `Repository` | ✅ 优秀 | 仅负责数据访问 |
| `ValidationBehavior` | ✅ 优秀 | 仅负责验证 |
| `ModuleBase` | ⚠️ 良好 | 职责稍多，但合理 |

### 5.2 开闭原则 (OCP)

| 扩展点 | 评分 | 说明 |
|--------|------|------|
| 模块系统 | ✅ 优秀 | 通过继承扩展 |
| 管道行为 | ✅ 优秀 | 可插拔设计 |
| 缓存策略 | ✅ 优秀 | 策略模式 |
| 异常处理 | ⚠️ 可改进 | 建议添加更多处理器 |

### 5.3 里氏替换原则 (LSP)

- ✅ 所有仓储实现可互换
- ✅ 所有缓存实现可互换
- ✅ 模块继承关系正确

### 5.4 接口隔离原则 (ISP)

- ✅ `IRepository` vs `IReadOnlyRepository`
- ✅ `ICommand` vs `IQuery`
- ⚠️ 建议：拆分大型接口

### 5.5 依赖倒置原则 (DIP)

- ✅ 核心层不依赖基础设施
- ✅ 依赖注入贯穿始终
- ✅ 抽象定义在核心层

---

## 六、设计模式应用评估

### 6.1 已应用的设计模式

| 模式 | 应用位置 | 评分 |
|------|----------|------|
| **依赖注入** | 全局 | ✅ 优秀 |
| **中介者模式** | MediatR/CQRS | ✅ 优秀 |
| **仓储模式** | 数据访问 | ✅ 优秀 |
| **工作单元** | 事务管理 | ✅ 优秀 |
| **策略模式** | 缓存、日志 | ✅ 优秀 |
| **管道模式** | 中间件 | ✅ 优秀 |
| **工厂模式** | 模块加载器 | ✅ 优秀 |
| **观察者模式** | 领域事件 | ✅ 优秀 |
| **装饰器模式** | 管道行为 | ✅ 优秀 |
| **规格模式** | ❌ 未应用 | ⚠️ 建议添加 |

### 6.2 架构模式

| 模式 | 应用情况 | 评价 |
|------|----------|------|
| **分层架构** | ✅ 完整实现 | 优秀的层次分离 |
| **洋葱架构** | ✅ 遵循原则 | 依赖方向正确 |
| **清洁架构** | ✅ 业务逻辑独立 | 核心不依赖外部 |
| **CQRS** | ✅ 完整实现 | 读写分离清晰 |
| **DDD** | ✅ 核心模式实现 | 聚合、值对象完善 |
| **模块化** | ✅ 可插拔设计 | 生命周期管理完善 |

---

## 七、改进建议

### 7.1 高优先级改进

#### 1. 添加规格模式 (Specification Pattern)

**当前问题：**
- 仓储查询缺乏组合能力
- 复杂查询逻辑散落在各处

**建议实现：**
```csharp
// 定义规格接口
public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    List<string> Includes { get; }
}

// 示例规格
public class ActiveTasksSpecification : ISpecification<Task>
{
    public Expression<Func<Task, bool>> Criteria => t => !t.IsCompleted;
    public List<string> Includes => new() { "Assignee", "Project" };
}

// 使用
var tasks = await _repository.ListAsync(new ActiveTasksSpecification());
```

**预期收益：**
- 查询逻辑可复用
- 复杂条件可组合
- 符合开闭原则

#### 2. 添加领域事件处理机制

**当前问题：**
- 领域事件已定义，但缺乏处理框架

**建议实现：**
```csharp
// 添加事件处理器接口
public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}

// 添加事件调度器
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events);
}

// 在 UnitOfWork 中集成
public async Task CommitTransactionAsync()
{
    await _dbContext.SaveChangesAsync();

    var aggregates = _dbContext.ChangeTracker.Entries<AggregateRoot>()
        .Select(e => e.Entity);

    foreach (var aggregate in aggregates)
    {
        await _eventDispatcher.DispatchAsync(aggregate.DomainEvents);
        aggregate.ClearDomainEvents();
    }
}
```

**预期收益：**
- 完整的领域事件处理
- 解耦聚合和副作用
- 支持事件溯源（未来扩展）

#### 3. 添加软删除支持

**建议实现：**
```csharp
public abstract class EntityBase<TKey>
{
    public bool IsDeleted { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }
    public string? DeletedBy { get; protected set; }

    public void SoftDelete(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}

// 全局查询过滤器
public abstract class ApplicationDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(FilterSoftDeleted);
            }
        }
    }
}
```

#### 4. 添加批量操作支持

**建议实现：**
```csharp
public interface IRepository<TEntity, TKey>
{
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities);
}
```

### 7.2 中优先级改进

#### 5. 添加全局异常处理增强

**建议实现：**
```csharp
// 添加更多异常处理器
public class ValidationExceptionHandler : IExceptionHandler
{
    public async Task<ErrorResult?> HandleAsync(Exception exception)
    {
        if (exception is ValidationException validationEx)
        {
            return new ErrorResult
            {
                StatusCode = 400,
                Title = "验证失败",
                Errors = validationEx.Errors
            };
        }
        return null;
    }
}

// 添加问题详情 (RFC 7807) 支持
public class ProblemDetailsResult
{
    public string Type { get; set; }  // URI to problem type
    public string Title { get; set; }
    public int Status { get; set; }
    public string Detail { get; set; }
    public string Instance { get; set; }
    public Dictionary<string, string[]> Errors { get; set; }
}
```

#### 6. 添加限流和熔断

**建议实现：**
```csharp
// 使用 Polly 添加限流管道
public class RateLimitBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly AsyncRateLimitPolicy _policy;

    public async Task<TResponse> Handle(TRequest request, ...)
    {
        return await _policy.ExecuteAsync(() => next());
    }
}

// 添加熔断器
public class CircuitBreakerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly AsyncCircuitBreakerPolicy _policy;
    // ...
}
```

#### 7. 添加性能监控

**建议实现：**
```csharp
// 添加性能计数器
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, ...)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();
            _logger.LogPerformance<TRequest>(stopwatch.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError<TRequest>(ex, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
```

### 7.3 低优先级改进

#### 8. 添加审计日志

**建议实现：**
```csharp
public interface IAuditService
{
    Task LogActionAsync(string action, string entity, string entityId, object? changes);
}

// 自动审计实体变更
public class AuditableInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(...)
    {
        foreach (var entry in dbContext.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
            {
                // 记录变更
            }
        }
    }
}
```

#### 9. 添加数据迁移脚本支持

**建议实现：**
- 添加自定义迁移脚本支持
- 支持种子数据
- 环境特定的迁移

#### 10. 添加集成测试框架

**建议实现：**
```csharp
// 集成测试基类
public abstract class IntegrationTestBase : IClassFixture<ApiFactory>
{
    protected readonly HttpClient Client;
    protected readonly ApiFactory Factory;

    protected IntegrationTestBase(ApiFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }
}

// API 工厂
public class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 使用测试数据库
            services.AddDbContext<TestDbContext>();
        });
    }
}
```

### 7.4 代码质量改进

#### 11. 添加 XML 文档注释

**当前状态：** 部分类有注释，但不完整

**建议：**
- 为所有公共 API 添加 XML 文档
- 使用文档生成工具（DocFX）
- 添加代码示例

#### 12. 添加静态分析

**建议工具：**
- **Roslyn Analyzers** - 编译时检查
- **StyleCop** - 代码风格
- **SonarAnalyzer** - 代码质量
- **Security Code Scan** - 安全检查

#### 13. 添加性能基准测试

**建议实现：**
```csharp
// 使用 BenchmarkDotNet
[MemoryDiagnoser]
public class RepositoryBenchmarks
{
    private IRepository<Task, Guid> _repository = null!;

    [Benchmark]
    public async Task<Task?> GetById()
    {
        return await _repository.GetByIdAsync(Guid.NewGuid());
    }

    [Benchmark]
    public async Task<List<Task>> ListAll()
    {
        return await _repository.ListAllAsync();
    }
}
```

---

## 八、架构优势总结

### 8.1 核心优势

1. **清晰的架构分层**
   - 依赖方向明确
   - 职责边界清晰
   - 易于理解和维护

2. **强大的模块化系统**
   - 可插拔架构
   - 生命周期管理
   - 依赖解析

3. **完善的 CQRS 实现**
   - 读写分离
   - 管道行为
   - 结果模式

4. **领域驱动设计**
   - 聚合根
   - 值对象
   - 领域事件

5. **现代化技术栈**
   - .NET 10 + C# 13
   - 异步编程
   - 依赖注入

### 8.2 可维护性评估

| 指标 | 评分 | 说明 |
|------|------|------|
| **可读性** | ⭐⭐⭐⭐⭐ | 代码清晰，命名规范 |
| **可测试性** | ⭐⭐⭐⭐ | 依赖注入便于测试 |
| **可扩展性** | ⭐⭐⭐⭐⭐ | 模块化设计灵活 |
| **可维护性** | ⭐⭐⭐⭐ | 结构清晰但需要更多文档 |
| **性能** | ⭐⭐⭐⭐ | 基础良好，有优化空间 |

---

## 九、技术债务清单

### 9.1 需要关注的技术债务

1. **文档不足**
   - 缺少架构决策记录 (ADR)
   - API 文档不完整
   - 缺少开发者指南

2. **测试覆盖**
   - 单元测试覆盖率未知
   - 缺少集成测试
   - 缺少端到端测试

3. **配置验证**
   - 缺少启动时配置验证
   - 缺少配置模式文档

4. **错误处理**
   - 异常消息可能暴露敏感信息
   - 缺少错误码标准

5. **性能优化**
   - 未使用编译查询
   - 未实现查询缓存
   - N+1 查询风险

---

## 十、下一步行动计划

### 阶段一：基础设施完善 (1-2 周)

1. ✅ 添加规格模式实现
2. ✅ 完善领域事件处理
3. ✅ 添加软删除支持
4. ✅ 添加批量操作

### 阶段二：质量提升 (2-3 周)

1. 添加集成测试框架
2. 添加单元测试覆盖
3. 添加静态分析工具
4. 完善文档注释

### 阶段三：性能优化 (1-2 周)

1. 添加性能监控
2. 实现查询缓存
3. 优化 EF Core 查询
4. 添加编译查询

### 阶段四：安全加固 (1 周)

1. 审查异常消息
2. 添加安全头
3. 实现速率限制
4. 添加审计日志

---

## 结论

Lemoo.UI 是一个**架构设计优秀**的 .NET 企业级应用框架。它成功应用了 DDD、CQRS、模块化等现代架构模式，具有良好的可扩展性和可维护性。

**主要亮点：**
- ✅ 清晰的分层架构
- ✅ 强大的模块化系统
- ✅ 完善的 CQRS 实现
- ✅ 现代化的技术栈

**需要改进的方面：**
- ⚠️ 测试覆盖不足
- ⚠️ 文档需要完善
- ⚠️ 缺少规格模式
- ⚠️ 性能监控待加强

**总体评分：8.5/10**

这是一个值得学习和参考的架构设计，通过实施建议的改进措施，可以进一步提升至生产级企业应用标准。
