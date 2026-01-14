# Lemoo 架构文档

## 架构概述

Lemoo 是一个基于 .NET 10 的专业模块化架构框架，采用领域驱动设计（DDD）、命令查询职责分离（CQRS）和插件式架构设计。

## 架构分层

### 1. 启动引导层 (Bootstrap)

负责应用程序的初始化、配置验证、环境检测和启动前检查。

**主要组件：**
- `IBootstrapper` - 启动引导器接口
- `Bootstrapper` - 启动引导器实现
- `ServiceCollectionExtensions` - 服务注册扩展

**职责：**
- 配置验证
- 环境检测
- 日志初始化
- 模块加载和注册
- 服务配置

### 2. 宿主层 (Host)

WPF应用程序的入口点，负责应用程序生命周期管理。

**主要组件：**
- `App.xaml.cs` - 应用程序入口
- `MainWindow.xaml.cs` - 主窗口

**职责：**
- 应用程序启动和关闭
- 模块生命周期管理
- 依赖注入容器管理

### 3. 核心层 (Core)

#### 3.1 核心抽象层 (Core.Abstractions)

定义所有核心接口和契约。

**主要接口：**
- CQRS接口：`ICommand`, `IQuery`, `ICommandHandler`, `IQueryHandler`, `IPipelineBehavior`
- 领域接口：`IEntity`, `IDomainEvent`, `IDomainEventHandler`
- 持久化接口：`IRepository`, `IUnitOfWork`, `IModuleDbContextFactory`
- 模块接口：`IModule`, `IModuleLoader`
- 服务接口：`ICacheService`, `ILoggerService`, `IConfigurationService`

#### 3.2 核心通用层 (Core.Common)

提供通用功能和工具类。

**主要组件：**
- 异常类：`BusinessException`, `ValidationException`, `NotFoundException`等
- 扩展方法：`StringExtensions`, `EnumerableExtensions`, `ServiceCollectionExtensions`
- 工具类：`Guard`, `DateTimeHelper`

#### 3.3 核心领域层 (Core.Domain)

提供领域模型的基础实现。

**主要组件：**
- `EntityBase<TKey>` - 实体基类
- `ValueObjectBase` - 值对象基类
- `DomainEventBase` - 领域事件基类
- `AggregateRoot<TKey>` - 聚合根基类

#### 3.4 核心应用层 (Core.Application)

提供应用层的通用能力。

**主要组件：**
- `Result<T>` - 结果模式
- `PagedResult<T>` - 分页结果
- `ValidationBehavior` - 验证管道行为
- `LoggingBehavior` - 日志管道行为

#### 3.5 基础设施层 (Core.Infrastructure)

提供基础设施实现。

**主要组件：**
- `ModuleLoader` - 模块加载器
- `ModuleDbContextFactory` - 数据库上下文工厂
- `MemoryCacheService` - 内存缓存服务
- `SerilogConfiguration` - 日志配置
- `ConfigurationService` - 配置服务
- `DeploymentModeService` - 部署模式服务

### 4. 模块层 (Modules)

#### 4.1 模块抽象层 (Modules.Abstractions)

定义模块的基础抽象。

**主要组件：**
- `ModuleBase` - 模块基类

#### 4.2 业务模块 (Modules.Example)

示例业务模块，展示如何使用架构。

**结构：**
```
Modules.Example/
├── Domain/              # 领域层
│   ├── Entities/        # 实体
│   └── DomainEvents/    # 领域事件
├── Application/         # 应用层
│   ├── Commands/        # 命令
│   ├── Queries/         # 查询
│   ├── DTOs/            # 数据传输对象
│   ├── Validators/      # 验证器
│   └── Handlers/        # 处理器
└── Infrastructure/      # 基础设施层
    ├── Persistence/     # 持久化
    └── Repositories/    # 仓储
```

### 5. UI层 (UI)

#### 5.1 WPF UI层 (UI.WPF)

WPF用户界面实现。

**主要组件：**
- `MainViewModel` - 主视图模型（MVVM模式）
- `MainWindow` - 主窗口

### 6. API层 (Api)

Web API实现，支持RESTful API。

**主要组件：**
- `ExamplesController` - 示例控制器
- Swagger/OpenAPI集成

## 核心设计模式

### 1. 插件式模块系统

- **动态加载**：运行时发现和加载模块
- **依赖管理**：自动解析模块依赖关系
- **拓扑排序**：确保模块按正确顺序加载
- **循环依赖检测**：防止循环依赖

### 2. DDD（领域驱动设计）

- **实体**：具有唯一标识的对象
- **值对象**：通过值相等性比较的对象
- **聚合根**：管理聚合内实体的根实体
- **领域事件**：表示领域内发生的重要事件

### 3. CQRS（命令查询职责分离）

- **命令**：修改状态的操作
- **查询**：读取数据的操作
- **处理器**：处理命令和查询
- **管道行为**：横切关注点（验证、日志、事务等）

### 4. 结果模式

统一的操作结果表示，支持成功和失败两种状态。

## 模块开发指南

### 创建新模块

1. 创建模块项目
2. 继承 `ModuleBase`
3. 实现必要的抽象方法
4. 定义模块的领域模型
5. 实现命令和查询
6. 配置数据库上下文

### 模块结构示例

```csharp
public class MyModule : ModuleBase
{
    public override string Name => "MyModule";
    public override string Version => "1.0.0";
    public override string Description => "我的业务模块";
    
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // 注册服务
    }
    
    public override void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder, IConfiguration configuration)
    {
        // 配置数据库
    }
}
```

## 配置说明

### appsettings.json

```json
{
  "Lemoo": {
    "Mode": "Local",
    "Modules": {
      "Enabled": [ "*" ],
      "Path": "./Modules"
    },
    "Database": {
      "Provider": "SqlServer",
      "ConnectionStrings": {
        "Example": "..."
      }
    }
  }
}
```

## 最佳实践

1. **模块隔离**：每个模块应该完全独立，不直接依赖其他模块
2. **接口抽象**：通过接口定义契约，而不是具体实现
3. **依赖注入**：使用依赖注入管理对象生命周期
4. **异步操作**：所有I/O操作应该使用异步方法
5. **错误处理**：使用结果模式统一处理错误
6. **日志记录**：记录关键操作和错误信息
7. **单元测试**：为业务逻辑编写单元测试

