# 模块开发指南

## 模块概述

在Lemoo架构中，模块是业务功能的独立单元，每个模块拥有自己的领域模型、应用逻辑和数据库上下文。

## 模块结构

一个标准的模块应该包含以下结构：

```
Lemoo.Modules.YourModule/
├── Domain/                  # 领域层
│   ├── Entities/           # 实体
│   ├── ValueObjects/       # 值对象
│   ├── DomainEvents/       # 领域事件
│   └── Interfaces/         # 领域接口
├── Application/             # 应用层
│   ├── Commands/           # 命令
│   ├── Queries/            # 查询
│   ├── DTOs/               # 数据传输对象
│   ├── Validators/         # FluentValidation验证器
│   ├── Handlers/           # 命令/查询处理器
│   └── Mappings/           # Mapster映射配置
├── Infrastructure/          # 基础设施层
│   ├── Persistence/        # 持久化
│   │   ├── YourModuleDbContext.cs
│   │   └── Configurations/
│   └── Repositories/       # 仓储实现
└── YourModule.cs           # 模块定义
```

## 创建模块步骤

### 1. 创建项目

```bash
dotnet new classlib -n Lemoo.Modules.YourModule -f net10.0
```

### 2. 添加依赖

```bash
dotnet add package MediatR
dotnet add package FluentValidation
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Mapster
```

### 3. 实现模块类

```csharp
public class YourModule : ModuleBase
{
    public override string Name => "YourModule";
    public override string Version => "1.0.0";
    public override string Description => "您的业务模块描述";
    
    public override IReadOnlyList<string> Dependencies => Array.Empty<string>();
    
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // 注册数据库上下文
        var connectionString = configuration.GetConnectionString("YourModule");
        services.AddDbContext<YourModuleDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
        
        // 注册仓储
        services.AddScoped<IYourEntityRepository, YourEntityRepository>();
        
        // 注册MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(YourModule).Assembly);
        });
        
        // 注册FluentValidation
        services.AddValidatorsFromAssembly(typeof(YourModule).Assembly);
    }
    
    public override void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("YourModule");
        optionsBuilder.UseSqlServer(connectionString);
    }
}
```

### 4. 定义领域模型

```csharp
public class YourEntity : AggregateRoot
{
    public string Name { get; private set; }
    
    private YourEntity() { }
    
    public static YourEntity Create(string name)
    {
        var entity = new YourEntity
        {
            Name = name
        };
        entity.AddDomainEvent(new YourEntityCreatedEvent(entity.Id));
        return entity;
    }
}
```

### 5. 定义命令和查询

```csharp
// 命令
public record CreateYourEntityCommand(string Name) : ICommand<Result<YourEntityDto>>;

// 查询
public record GetYourEntityQuery(Guid Id) : IQuery<Result<YourEntityDto>>;
```

### 6. 实现处理器

```csharp
public class CreateYourEntityCommandHandler : ICommandHandler<CreateYourEntityCommand, Result<YourEntityDto>>
{
    private readonly IYourEntityRepository _repository;
    
    public async Task<Result<YourEntityDto>> Handle(CreateYourEntityCommand request, CancellationToken cancellationToken)
    {
        var entity = YourEntity.Create(request.Name);
        await _repository.AddAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        
        var dto = entity.Adapt<YourEntityDto>();
        return Result<YourEntityDto>.Success(dto);
    }
}
```

### 7. 配置数据库上下文

```csharp
public class YourModuleDbContext : DbContext
{
    public YourModuleDbContext(DbContextOptions<YourModuleDbContext> options) : base(options)
    {
    }
    
    public DbSet<YourEntity> YourEntities { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<YourEntity>(entity =>
        {
            entity.ToTable("YourEntities");
            entity.HasKey(e => e.Id);
            // 配置其他属性
        });
    }
}
```

## 模块配置

在 `appsettings.json` 中配置模块：

```json
{
  "Lemoo": {
    "Modules": {
      "Enabled": [ "YourModule" ],
      "Path": "./Modules"
    },
    "Database": {
      "ConnectionStrings": {
        "YourModule": "Server=localhost;Database=Lemoo_YourModule;..."
      }
    }
  }
}
```

## 模块依赖

如果模块依赖其他模块，在模块类中声明：

```csharp
public override IReadOnlyList<string> Dependencies => new[] { "Example" };
```

系统会自动处理依赖关系，确保依赖模块先加载。

## 模块生命周期

模块支持以下生命周期方法：

- `OnApplicationStartingAsync` - 应用启动前
- `OnApplicationStartedAsync` - 应用启动后
- `OnApplicationStoppingAsync` - 应用停止前
- `OnApplicationStoppedAsync` - 应用停止后

## 最佳实践

1. **单一职责**：每个模块应该只负责一个业务领域
2. **独立性**：模块应该尽可能独立，减少对其他模块的依赖
3. **接口抽象**：通过接口定义模块间的契约
4. **领域事件**：使用领域事件实现模块间通信
5. **测试覆盖**：为模块编写单元测试和集成测试

