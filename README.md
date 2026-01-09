# Lemoo - .NET 10 模块化架构框架

## 项目概述

Lemoo 是一个基于 .NET 10 的专业模块化架构框架，采用 DDD（领域驱动设计）、CQRS（命令查询职责分离）和插件式架构设计。

## 技术栈

- **.NET 10 (LTS)** - 长期支持版本
- **C# 13** - 最新语言特性
- **WPF + CommunityToolkit.Mvvm** - 桌面应用UI框架
- **Microsoft.Extensions.Hosting** - 应用程序宿主
- **Microsoft.Extensions.DependencyInjection** - 依赖注入
- **DDD + CQRS** - 架构模式
- **MediatR** - 中介者模式实现
- **EF Core** - 对象关系映射
- **FluentValidation** - 验证框架
- **Mapster** - 对象映射（待集成）
- **Serilog** - 结构化日志
- **Swagger / OpenAPI** - API文档（API模式）

## 架构设计

### 分层架构

```
Lemoo/
├── Lemoo.Bootstrap/              # 启动引导层
├── Lemoo.Host/                    # 宿主层（WPF应用）
├── Lemoo.Core/                    # 核心层
│   ├── Lemoo.Core.Abstractions/   # 核心抽象
│   ├── Lemoo.Core.Domain/         # 领域核心
│   ├── Lemoo.Core.Application/    # 应用核心
│   ├── Lemoo.Core.Infrastructure/ # 基础设施
│   └── Lemoo.Core.Common/         # 通用能力
├── Lemoo.Modules/                 # 模块层
│   ├── Lemoo.Modules.Abstractions/# 模块抽象
│   └── [业务模块]/                # 业务子域
├── Lemoo.UI/                      # UI层
│   ├── Lemoo.UI.WPF/              # WPF应用
│   └── Lemoo.UI.Shared/           # UI共享组件
└── Lemoo.Api/                     # API层（Web API）
```

## 已完成功能

### ✅ 核心抽象层 (Core.Abstractions)
- CQRS接口（ICommand, IQuery, ICommandHandler, IQueryHandler, IPipelineBehavior）
- 领域接口（IEntity, IDomainEvent, IDomainEventHandler）
- 持久化接口（IRepository, IUnitOfWork, IModuleDbContextFactory）
- 模块接口（IModule, IModuleLoader）
- 缓存、日志、配置、部署模式接口

### ✅ 核心通用层 (Core.Common)
- 异常类（BusinessException, ValidationException, NotFoundException等）
- 扩展方法（StringExtensions, EnumerableExtensions, ServiceCollectionExtensions）
- 工具类（Guard, DateTimeHelper）

### ✅ 核心领域层 (Core.Domain)
- EntityBase（实体基类，支持领域事件）
- ValueObjectBase（值对象基类）
- DomainEventBase（领域事件基类）
- AggregateRoot（聚合根基类）

### ✅ 核心应用层 (Core.Application)
- Result（结果模式）
- PagedResult（分页结果）
- ValidationBehavior（验证管道行为）
- LoggingBehavior（日志管道行为，集成性能指标）
- CacheBehavior（缓存管道行为，支持 CacheAttribute）
- TransactionBehavior（事务处理管道行为）
- CacheAttribute（缓存特性）
- PerformanceMetrics（性能指标收集器）
- CommandExtensions / QueryExtensions（命令和查询扩展方法）

### ✅ 基础设施层 (Core.Infrastructure)
- ModuleLoader（模块加载器，支持依赖管理和拓扑排序）
- ModuleDbContextFactory（模块数据库上下文工厂）
- UnitOfWork（工作单元实现，支持事务管理）
- MemoryCacheService（内存缓存服务）
- SerilogConfiguration（Serilog日志配置）
- ConfigurationService（配置服务）
- DeploymentModeService（部署模式服务）

### ✅ 模块抽象层 (Modules.Abstractions)
- ModuleBase（模块基类，提供完整的生命周期管理）

### ✅ 启动引导层 (Bootstrap)
- Bootstrapper（应用程序引导器）
- 配置验证
- 环境检测
- 服务注册扩展

### ✅ 宿主层 (Host)
- WPF应用程序入口
- 模块生命周期管理
- 依赖注入集成

### ✅ 示例模块 (Modules.Example)
- **Domain层**：ExampleEntity实体、领域事件
- **Application层**：命令（CreateExampleCommand, UpdateExampleCommand）、查询（GetExampleQuery, GetAllExamplesQuery）、处理器、DTO、验证器
- **Infrastructure层**：ExampleDbContext、ExampleRepository

### ✅ WPF UI层 (UI.WPF)
- MainViewModel（使用CommunityToolkit.Mvvm）
- MainWindow（示例管理界面）
- 与示例模块集成

### ✅ API层 (Api)
- ExamplesController（RESTful API，使用BaseController）
- BaseController（控制器基类，提供通用功能）
- MetricsController（性能指标API）
- Swagger/OpenAPI集成
- 全局异常处理中间件
- 请求ID追踪中间件
- 健康检查端点（/health）
- Result扩展方法（简化API响应处理）
- 与示例模块集成

### ✅ 测试项目
- Lemoo.Core.Tests - 核心层单元测试
- Lemoo.Modules.Example.Tests - 示例模块测试
- 使用 xUnit、Moq、FluentAssertions

### ✅ 文档
- 架构文档 (docs/architecture.md)
- 快速开始指南 (docs/getting-started.md)
- 模块开发指南 (docs/modules.md)
- 优化建议文档 (docs/optimization-suggestions.md)
- 最新优化总结 (docs/latest-optimizations.md)

## 核心特性

### 1. 插件式模块系统
- 动态模块发现和加载
- 模块依赖管理和验证
- 拓扑排序确保正确的加载顺序
- 循环依赖检测

### 2. 每个模块独立的数据库上下文
- 每个模块拥有自己的DbContext
- 支持多种数据库提供程序（SQL Server, PostgreSQL, SQLite, InMemory）
- 数据库连接字符串独立配置

### 3. 本地与API模式统一
- 通过配置切换部署模式
- 统一的服务接口抽象
- 支持本地直接调用和HTTP API调用

### 4. 高可用性
- 异步处理
- 错误处理和恢复
- 日志记录和监控

### 5. 模块隔离
- 独立程序集
- 独立数据库上下文
- 接口契约隔离

### 6. 性能优化
- 异步/await模式
- 缓存支持
- 数据库连接池
- 查询优化

### 7. 测试友好
- 依赖注入便于Mock
- 接口抽象
- 清晰的职责分离

## 配置示例

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
        "Example": "Server=localhost;Database=Lemoo_Example;..."
      }
    }
  }
}
```

## 项目完成度

✅ **100% 完成** - 所有计划的功能已实现

### 已完成功能清单

- ✅ 完整的核心架构层（5个核心项目）
- ✅ 插件式模块系统
- ✅ 启动引导层和宿主层
- ✅ 完整的示例业务模块
- ✅ WPF用户界面
- ✅ RESTful API支持
- ✅ 单元测试项目
- ✅ 完整文档

### 可选扩展

- [ ] 更多业务模块示例
- [ ] 集成测试
- [ ] 性能测试
- [ ] Docker支持
- [ ] CI/CD配置

## 构建状态

✅ **解决方案编译成功（0个警告，0个错误）**

所有核心架构层、示例模块、WPF UI层和API层已完整实现并通过编译。

## 快速开始

### 前置要求
- .NET 10 SDK
- SQL Server（可选，用于数据库）

### 运行WPF应用
```bash
cd src/Lemoo.Host
dotnet run
```

### 运行API服务
```bash
cd src/Lemoo.Api
dotnet run
```

访问 Swagger UI: `https://localhost:5001/swagger`

### 运行测试
```bash
dotnet test
```

## 文档

- [架构文档](./docs/architecture.md) - 详细的架构说明
- [快速开始指南](./docs/getting-started.md) - 快速上手指南
- [模块开发指南](./docs/modules.md) - 如何开发新模块

## 许可证

待定

## 贡献

欢迎贡献代码和建议！

