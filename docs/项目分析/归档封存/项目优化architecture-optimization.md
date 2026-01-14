# 架构优化文档

## 优化概述

本次优化主要针对两个启动项（Lemoo.Api 和 Lemoo.Host）和 Bootstrapper 进行了重构，实现了统一的启动流程和最佳实践。

## 优化内容

### 1. 统一的启动扩展方法

#### 1.1 ApplicationBuilderExtensions（Web API）

创建了 `ApplicationBuilderExtensions.ConfigureLemooApplicationAsync` 方法，统一处理：
- 引导器创建和配置
- 服务注册（核心服务、模块服务、CQRS管道行为）
- 应用程序引导
- 模块生命周期启动

**使用示例：**
```csharp
var app = await builder.ConfigureLemooApplicationAsync(configurePipeline: app =>
{
    app.UseRequestId();
    app.UseGlobalExceptionHandler();
    // ... 其他中间件配置
});
```

#### 1.2 HostBuilderExtensions（WPF/桌面应用）

创建了 `HostBuilderExtensions.ConfigureLemooApplication` 方法，统一处理：
- Host配置
- 服务注册
- 支持自定义服务配置回调

**使用示例：**
```csharp
var hostBuilder = new HostBuilder()
    .ConfigureLemooApplication(configuration, (services, config) =>
    {
        // 自定义服务注册
        RegisterUIFrameworkServices(services);
        RegisterModuleUIs(services, config);
    });
```

### 2. Bootstrapper 增强

#### 2.1 模块生命周期管理

在 `Bootstrapper` 中添加了模块生命周期管理方法：
- `StartModulesAsync`: 启动模块生命周期（启动前和启动后）
- `StopModulesAsync`: 停止模块生命周期（停止前和停止后）

#### 2.2 统一的错误处理

所有模块生命周期方法都包含完整的错误处理和日志记录。

### 3. 代码消除重复

#### 3.1 之前的问题

- API 和 WPF 启动项都有重复的模块生命周期管理代码
- 引导器创建和配置逻辑分散
- 服务注册顺序不一致

#### 3.2 优化后

- 统一的扩展方法封装了所有启动逻辑
- 模块生命周期管理集中在 Bootstrapper 中
- 服务注册顺序统一且可配置

### 4. 配置管理优化

#### 4.1 StartupOptions 类

创建了 `StartupOptions` 类用于统一管理启动配置：
- `AutoMigrate`: 是否启用自动数据库迁移
- `SeedData`: 是否启用数据种子
- `ThrowOnBootstrapFailure`: 是否在启动失败时抛出异常
- `EnableModuleLifecycleLogging`: 是否启用模块生命周期日志

## 架构优势

### 1. 代码复用

- 两个启动项共享相同的启动逻辑
- 减少代码重复，提高可维护性

### 2. 一致性

- 统一的启动流程确保行为一致
- 错误处理方式统一

### 3. 可扩展性

- 通过回调函数支持自定义配置
- 易于添加新的启动项类型

### 4. 可测试性

- 启动逻辑封装在扩展方法中，易于测试
- Bootstrapper 可以独立测试

## 使用指南

### Web API 启动项

```csharp
var builder = WebApplication.CreateBuilder(args);

// 添加API特定服务
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// 配置Lemoo应用程序
var app = await builder.ConfigureLemooApplicationAsync(configurePipeline: app =>
{
    // 配置中间件
    app.UseRequestId();
    app.UseGlobalExceptionHandler();
    app.MapControllers();
});

app.Run();
```

### WPF 启动项

```csharp
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var hostBuilder = new HostBuilder()
    .ConfigureLemooApplication(configuration, (services, config) =>
    {
        // 注册UI特定服务
        RegisterUIFrameworkServices(services);
        RegisterModuleUIs(services, config);
    });

var host = hostBuilder.Build();

// 引导和启动模块
var bootstrapper = host.Services.GetRequiredService<Bootstrapper>();
await bootstrapper.BootstrapAsync();
await bootstrapper.StartModulesAsync(host.Services);
```

## 最佳实践

1. **统一使用扩展方法**: 所有启动项都应使用统一的扩展方法
2. **错误处理**: 在启动失败时提供清晰的错误信息
3. **日志记录**: 记录所有关键启动步骤
4. **配置验证**: 在启动前验证配置
5. **模块生命周期**: 确保正确启动和停止模块

## 未来改进

1. 添加启动性能监控
2. 支持启动时的健康检查
3. 添加启动配置验证
4. 支持启动时的依赖检查

