# Lemoo.Bootstrap 详细分析文档

> 版本：1.0.0  
> 更新日期：2025年1月  
> .NET 版本：10.0

## 目录

- [1. 项目概述](#1-项目概述)
- [2. 架构设计](#2-架构设计)
- [3. 核心组件分析](#3-核心组件分析)
- [4. 启动流程分析](#4-启动流程分析)
- [5. 模块生命周期管理](#5-模块生命周期管理)
- [6. 配置管理](#6-配置管理)
- [7. 扩展方法](#7-扩展方法)
- [8. 错误处理机制](#8-错误处理机制)
- [9. 依赖关系](#9-依赖关系)
- [10. 使用示例](#10-使用示例)
- [11. 最佳实践](#11-最佳实践)
- [12. 性能分析](#12-性能分析)
- [13. 测试建议](#13-测试建议)
- [14. 未来改进](#14-未来改进)

---

## 1. 项目概述

### 1.1 项目定位

**Lemoo.Bootstrap** 是 Lemoo 框架的启动引导层，负责应用程序的初始化、配置验证、环境检测、模块加载和生命周期管理。它是整个应用程序启动的核心组件，为 WPF 桌面应用和 Web API 应用提供统一的启动流程。

### 1.2 核心职责

1. **配置验证**：验证应用程序配置的完整性和正确性
2. **环境检测**：检测运行环境（Development、Staging、Production）
3. **日志初始化**：配置 Serilog 结构化日志系统
4. **模块加载**：发现、加载和注册所有业务模块
5. **服务注册**：注册核心服务和模块服务
6. **生命周期管理**：管理模块的启动和停止生命周期

### 1.3 设计原则

- **单一职责**：每个类和方法职责明确
- **依赖注入**：通过 DI 容器管理依赖关系
- **可扩展性**：通过扩展方法支持不同宿主类型
- **错误处理**：完善的错误处理和日志记录
- **可测试性**：易于单元测试和集成测试

---

## 2. 架构设计

### 2.1 项目结构

```
Lemoo.Bootstrap/
├── IBootstrapper.cs                    # 引导器接口
├── Bootstrapper.cs                     # 引导器实现
├── ServiceCollectionExtensions.cs      # 服务注册扩展
├── HostBuilderExtensions.cs            # Host构建器扩展（WPF/桌面应用）
├── ApplicationBuilderExtensions.cs     # 应用构建器扩展（Web API）
├── StartupOptions.cs                   # 启动配置选项
└── Lemoo.Bootstrap.csproj              # 项目文件
```

### 2.2 类图关系

```
┌─────────────────────┐
│   IBootstrapper     │
└──────────┬──────────┘
           │
           │ implements
           │
┌──────────▼──────────┐
│    Bootstrapper     │
│                     │
│ - _configuration    │
│ - _logger           │
│ - _loadedModules    │
│                     │
│ + BootstrapAsync()  │
│ + RegisterServices()│
│ + ConfigureHost()  │
│ + StartModulesAsync()│
│ + StopModulesAsync()│
└─────────────────────┘

┌─────────────────────────────┐
│  BootstrapResult            │
│                             │
│ + IsSuccess: bool           │
│ + Errors: IReadOnlyList     │
│ + ElapsedTime: TimeSpan     │
│ + Message: string           │
└─────────────────────────────┘

┌─────────────────────────────┐
│  BootstrapError             │
│                             │
│ + Code: string              │
│ + Message: string           │
│ + Details: string           │
└─────────────────────────────┘
```

### 2.3 依赖关系

```
Lemoo.Bootstrap
├── Lemoo.Core.Abstractions
│   ├── IModule
│   ├── IModuleLoader
│   ├── ICacheService
│   ├── IConfigurationService
│   ├── IDeploymentModeService
│   └── IModuleDbContextFactory
├── Lemoo.Core.Application
│   └── Extensions (CQRS管道行为)
├── Lemoo.Core.Infrastructure
│   ├── ModuleLoader
│   ├── MemoryCacheService
│   ├── ConfigurationService
│   ├── DeploymentModeService
│   ├── ModuleDbContextFactory
│   └── SerilogConfiguration
└── Lemoo.Modules.Abstractions
    └── ModuleBase
```

---

## 3. 核心组件分析

### 3.1 IBootstrapper 接口

**文件位置**：`src/Hosts/Lemoo.Bootstrap/IBootstrapper.cs`

**接口定义**：

```csharp
public interface IBootstrapper
{
    /// <summary>
    /// 引导应用程序
    /// </summary>
    Task<BootstrapResult> BootstrapAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 注册服务
    /// </summary>
    void RegisterServices(IServiceCollection services, IConfiguration configuration);
    
    /// <summary>
    /// 配置宿主
    /// </summary>
    void ConfigureHost(IHostBuilder hostBuilder);
}
```

**设计说明**：

- **BootstrapAsync**：异步引导方法，返回引导结果
- **RegisterServices**：注册应用程序服务（同步方法，在服务注册阶段调用）
- **ConfigureHost**：配置 Host Builder（日志、配置等）

### 3.2 Bootstrapper 实现类

**文件位置**：`src/Hosts/Lemoo.Bootstrap/Bootstrapper.cs`

#### 3.2.1 字段和属性

```csharp
private readonly IConfiguration _configuration;
private readonly ILogger<Bootstrapper> _logger;
private IReadOnlyList<IModule>? _loadedModules;
```

- `_configuration`：应用程序配置
- `_logger`：日志记录器
- `_loadedModules`：已加载的模块列表（在 RegisterServices 中加载）

#### 3.2.2 BootstrapAsync 方法

**功能**：执行应用程序引导流程

**执行步骤**：

1. **启动计时**：使用 `Stopwatch` 记录引导耗时
2. **配置验证**：调用 `ValidateConfiguration()` 验证配置
3. **环境检测**：检测运行环境（Development/Staging/Production）
4. **返回结果**：返回 `BootstrapResult` 包含成功/失败状态和错误信息

**代码流程**：

```csharp
public async Task<BootstrapResult> BootstrapAsync(CancellationToken cancellationToken = default)
{
    var stopwatch = Stopwatch.StartNew();
    var errors = new List<BootstrapError>();
    
    try
    {
        // 1. 验证配置
        var configErrors = ValidateConfiguration();
        errors.AddRange(configErrors);
        
        if (errors.Any())
        {
            return new BootstrapResult
            {
                IsSuccess = false,
                Errors = errors,
                ElapsedTime = stopwatch.Elapsed,
                Message = "配置验证失败"
            };
        }
        
        // 2. 检测环境
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
                         ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") 
                         ?? "Production";
        _logger.LogInformation("当前环境: {Environment}", environment);
        
        // 3. 初始化日志（已在ConfigureHost中配置）
        
        _logger.LogInformation("应用程序引导完成，耗时: {ElapsedMilliseconds}ms", 
            stopwatch.ElapsedMilliseconds);
        
        return new BootstrapResult
        {
            IsSuccess = true,
            Errors = Array.Empty<BootstrapError>(),
            ElapsedTime = stopwatch.Elapsed,
            Message = "应用程序引导成功"
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "应用程序引导失败");
        errors.Add(new BootstrapError
        {
            Code = "BOOTSTRAP_ERROR",
            Message = ex.Message,
            Details = ex.ToString()
        });
        
        return new BootstrapResult
        {
            IsSuccess = false,
            Errors = errors,
            ElapsedTime = stopwatch.Elapsed,
            Message = "应用程序引导失败"
        };
    }
}
```

**关键特性**：

- ✅ 完整的错误处理
- ✅ 性能监控（耗时统计）
- ✅ 详细的日志记录
- ✅ 配置验证

#### 3.2.3 RegisterServices 方法

**功能**：注册应用程序服务和模块服务

**执行步骤**：

1. **注册基础设施服务**：ModuleLoader、MemoryCache、Logging
2. **创建临时服务提供者**：用于创建 ModuleLoader（需要 ILogger）
3. **加载模块**：调用 `ModuleLoader.LoadModulesAsync()` 加载所有模块
4. **配置模块服务**：按顺序调用模块的配置方法
   - `PreConfigureServices`：预配置
   - `ConfigureServices`：配置
   - `PostConfigureServices`：后配置

**代码流程**：

```csharp
public void RegisterServices(IServiceCollection services, IConfiguration configuration)
{
    _logger.LogInformation("注册核心服务...");
    
    // 1. 注册基础设施服务
    services.AddSingleton<IModuleLoader, ModuleLoader>();
    services.AddMemoryCache();
    services.AddLogging(builder => builder.AddSerilog());
    
    // 2. 创建临时服务提供者（用于创建ModuleLoader）
    var tempServices = new ServiceCollection();
    tempServices.AddSingleton<IConfiguration>(configuration);
    tempServices.AddLogging();
    var tempProvider = tempServices.BuildServiceProvider();
    
    // 3. 加载模块
    var moduleLoader = new ModuleLoader(
        tempProvider.GetRequiredService<ILogger<ModuleLoader>>(),
        configuration);
    
    _loadedModules = moduleLoader.LoadModulesAsync().GetAwaiter().GetResult();
    
    // 4. 配置模块服务
    foreach (var module in _loadedModules)
    {
        _logger.LogInformation("配置模块服务: {ModuleName}", module.Name);
        module.PreConfigureServices(services, configuration);
        module.ConfigureServices(services, configuration);
    }
    
    // 5. 后配置
    foreach (var module in _loadedModules)
    {
        module.PostConfigureServices(services, configuration);
    }
    
    _logger.LogInformation("已注册 {Count} 个模块", _loadedModules.Count);
}
```

**关键特性**：

- ✅ 模块依赖顺序处理（由 ModuleLoader 保证）
- ✅ 三阶段配置（PreConfigure、Configure、PostConfigure）
- ✅ 同步加载模块（在服务注册阶段必须同步）

**注意事项**：

- ⚠️ 使用 `GetAwaiter().GetResult()` 同步等待异步操作
- ⚠️ 这在服务注册阶段是安全的，因为此时还没有构建 ServiceProvider
- ⚠️ 临时服务提供者仅用于创建 ModuleLoader，不应用于其他用途

#### 3.2.4 ConfigureHost 方法

**功能**：配置 Host Builder（日志和配置）

**执行步骤**：

1. **配置 Serilog**：使用 `SerilogConfiguration.ConfigureSerilog()` 配置日志
2. **配置应用配置**：加载 `appsettings.json` 和环境变量

**代码流程**：

```csharp
public void ConfigureHost(IHostBuilder hostBuilder)
{
    hostBuilder
        .UseSerilog((context, services, configuration) =>
        {
            var appConfiguration = context.Configuration;
            var loggerConfig = SerilogConfiguration.ConfigureSerilog(appConfiguration);
            loggerConfig.ReadFrom.Configuration(appConfiguration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();
            Log.Logger = loggerConfig.CreateLogger();
        })
        .ConfigureAppConfiguration((context, builder) =>
        {
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", 
                       optional: true, reloadOnChange: true)
                   .AddEnvironmentVariables();
        });
}
```

**关键特性**：

- ✅ 结构化日志（Serilog）
- ✅ 多环境配置支持
- ✅ 配置热重载
- ✅ 日志上下文增强

#### 3.2.5 StartModulesAsync 方法

**功能**：启动模块生命周期

**执行步骤**：

1. **获取已加载模块**：从 ModuleLoader 获取模块列表
2. **启动前处理**：按顺序调用 `OnApplicationStartingAsync`
3. **启动后处理**：按顺序调用 `OnApplicationStartedAsync`

**代码流程**：

```csharp
public async Task StartModulesAsync(IServiceProvider serviceProvider, 
    CancellationToken cancellationToken = default)
{
    if (_loadedModules == null)
    {
        _logger.LogWarning("模块尚未加载，无法启动模块生命周期");
        return;
    }
    
    var moduleLoader = serviceProvider.GetRequiredService<IModuleLoader>();
    var modules = moduleLoader.GetLoadedModules();
    
    // 启动前
    foreach (var module in modules)
    {
        try
        {
            _logger.LogDebug("模块 {ModuleName} 启动前处理", module.Name);
            await module.OnApplicationStartingAsync(serviceProvider, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "模块 {ModuleName} 启动前处理失败", module.Name);
            throw;
        }
    }
    
    // 启动后
    foreach (var module in modules)
    {
        try
        {
            _logger.LogDebug("模块 {ModuleName} 启动后处理", module.Name);
            await module.OnApplicationStartedAsync(serviceProvider, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(error, "模块 {ModuleName} 启动后处理失败", module.Name);
            throw;
        }
    }
    
    _logger.LogInformation("所有模块生命周期启动完成");
}
```

**关键特性**：

- ✅ 按依赖顺序执行
- ✅ 完整的错误处理和日志记录
- ✅ 支持取消令牌
- ✅ 异常传播（启动失败时抛出异常）

#### 3.2.6 StopModulesAsync 方法

**功能**：停止模块生命周期

**执行步骤**：

1. **获取已加载模块**：从 ModuleLoader 获取模块列表（逆序）
2. **停止前处理**：逆序调用 `OnApplicationStoppingAsync`
3. **停止后处理**：逆序调用 `OnApplicationStoppedAsync`

**代码流程**：

```csharp
public async Task StopModulesAsync(IServiceProvider serviceProvider, 
    CancellationToken cancellationToken = default)
{
    if (_loadedModules == null)
    {
        return;
    }
    
    var moduleLoader = serviceProvider.GetRequiredService<IModuleLoader>();
    var modules = moduleLoader.GetLoadedModules().Reverse();
    
    // 停止前
    foreach (var module in modules)
    {
        try
        {
            _logger.LogDebug("模块 {ModuleName} 停止前处理", module.Name);
            await module.OnApplicationStoppingAsync(serviceProvider, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "模块 {ModuleName} 停止前处理失败", module.Name);
            // 注意：停止时不会抛出异常，只记录日志
        }
    }
    
    // 停止后
    foreach (var module in modules)
    {
        try
        {
            _logger.LogDebug("模块 {ModuleName} 停止后处理", module.Name);
            await module.OnApplicationStoppedAsync(serviceProvider, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "模块 {ModuleName} 停止后处理失败", module.Name);
        }
    }
    
    _logger.LogInformation("所有模块生命周期停止完成");
}
```

**关键特性**：

- ✅ 逆序执行（与启动顺序相反）
- ✅ 错误容忍（停止时不会抛出异常）
- ✅ 完整的日志记录
- ✅ 支持取消令牌

#### 3.2.7 ValidateConfiguration 方法

**功能**：验证应用程序配置

**验证项**：

1. **Lemoo 配置节**：检查是否存在 `Lemoo` 配置节
2. **模块配置**：检查是否存在 `Lemoo:Modules` 配置节
3. **数据库配置**：检查是否存在 `Lemoo:Database` 配置节

**代码流程**：

```csharp
private IReadOnlyList<BootstrapError> ValidateConfiguration()
{
    var errors = new List<BootstrapError>();
    
    // 验证Lemoo配置节
    var lemooSection = _configuration.GetSection("Lemoo");
    if (!lemooSection.Exists())
    {
        errors.Add(new BootstrapError
        {
            Code = "MISSING_CONFIG",
            Message = "缺少 'Lemoo' 配置节"
        });
        return errors;
    }
    
    // 验证模块配置
    var modulesSection = lemooSection.GetSection("Modules");
    if (!modulesSection.Exists())
    {
        errors.Add(new BootstrapError
        {
            Code = "MISSING_MODULES_CONFIG",
            Message = "缺少 'Lemoo:Modules' 配置节"
        });
    }
    
    // 验证数据库配置
    var databaseSection = lemooSection.GetSection("Database");
    if (!databaseSection.Exists())
    {
        errors.Add(new BootstrapError
        {
            Code = "MISSING_DATABASE_CONFIG",
            Message = "缺少 'Lemoo:Database' 配置节"
        });
    }
    
    return errors;
}
```

**关键特性**：

- ✅ 早期验证（在启动前发现配置问题）
- ✅ 详细的错误信息（错误代码和消息）
- ✅ 可扩展（可以添加更多验证项）

---

### 3.3 BootstrapResult 类

**文件位置**：`src/Hosts/Lemoo.Bootstrap/IBootstrapper.cs`

**类定义**：

```csharp
public class BootstrapResult
{
    public bool IsSuccess { get; set; }
    public IReadOnlyList<BootstrapError> Errors { get; set; } = Array.Empty<BootstrapError>();
    public TimeSpan ElapsedTime { get; set; }
    public string? Message { get; set; }
}
```

**属性说明**：

- `IsSuccess`：引导是否成功
- `Errors`：错误列表（只读集合）
- `ElapsedTime`：引导耗时
- `Message`：结果消息

**使用场景**：

- 检查引导结果
- 记录引导耗时
- 显示错误信息

### 3.4 BootstrapError 类

**文件位置**：`src/Hosts/Lemoo.Bootstrap/IBootstrapper.cs`

**类定义**：

```csharp
public class BootstrapError
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
}
```

**属性说明**：

- `Code`：错误代码（用于程序化处理）
- `Message`：错误消息（用户友好）
- `Details`：详细错误信息（堆栈跟踪等）

**错误代码**：

- `MISSING_CONFIG`：缺少配置节
- `MISSING_MODULES_CONFIG`：缺少模块配置
- `MISSING_DATABASE_CONFIG`：缺少数据库配置
- `BOOTSTRAP_ERROR`：引导过程错误

---

### 3.5 StartupOptions 类

**文件位置**：`src/Hosts/Lemoo.Bootstrap/StartupOptions.cs`

**类定义**：

```csharp
public class StartupOptions
{
    /// <summary>
    /// 是否启用自动数据库迁移
    /// </summary>
    public bool AutoMigrate { get; set; } = false;
    
    /// <summary>
    /// 是否启用数据种子
    /// </summary>
    public bool SeedData { get; set; } = false;
    
    /// <summary>
    /// 是否在启动失败时抛出异常
    /// </summary>
    public bool ThrowOnBootstrapFailure { get; set; } = true;
    
    /// <summary>
    /// 是否启用模块生命周期日志
    /// </summary>
    public bool EnableModuleLifecycleLogging { get; set; } = true;
}
```

**配置选项说明**：

- `AutoMigrate`：自动执行数据库迁移（EF Core Migrations）
- `SeedData`：自动填充种子数据
- `ThrowOnBootstrapFailure`：启动失败时是否抛出异常
- `EnableModuleLifecycleLogging`：是否记录模块生命周期日志

**注意**：当前版本中 `StartupOptions` 已定义但尚未完全集成到启动流程中，可作为未来扩展点。

---

## 4. 启动流程分析

### 4.1 WPF 应用启动流程

```
App.OnStartup()
    │
    ├─> 构建 Configuration
    │
    ├─> 创建 HostBuilder
    │   │
    │   └─> ConfigureLemooApplication()
    │       │
    │       ├─> 创建 Bootstrapper
    │       │
    │       ├─> ConfigureHost()
    │       │   ├─> 配置 Serilog
    │       │   └─> 配置 AppConfiguration
    │       │
    │       └─> RegisterServices()
    │           ├─> 注册核心服务
    │           ├─> 加载模块
    │           └─> 配置模块服务
    │
    ├─> Build Host
    │
    ├─> BootstrapAsync()
    │   ├─> 验证配置
    │   ├─> 检测环境
    │   └─> 返回结果
    │
    ├─> StartModulesAsync()
    │   ├─> OnApplicationStartingAsync (所有模块)
    │   └─> OnApplicationStartedAsync (所有模块)
    │
    └─> 显示主窗口
```

### 4.2 Web API 应用启动流程

```
Program.Main()
    │
    ├─> WebApplication.CreateBuilder()
    │
    ├─> ConfigureLemooApplicationAsync()
    │   │
    │   ├─> 创建 Bootstrapper
    │   │
    │   ├─> ConfigureHost()
    │   │   ├─> 配置 Serilog
    │   │   └─> 配置 AppConfiguration
    │   │
    │   ├─> RegisterServices()
    │   │   ├─> 注册核心服务
    │   │   ├─> 加载模块
    │   │   └─> 配置模块服务
    │   │
    │   ├─> Build Application
    │   │
    │   ├─> 配置中间件管道
    │   │
    │   ├─> BootstrapAsync()
    │   │   ├─> 验证配置
    │   │   ├─> 检测环境
    │   │   └─> 返回结果
    │   │
    │   └─> StartModulesAsync()
    │       ├─> OnApplicationStartingAsync (所有模块)
    │       └─> OnApplicationStartedAsync (所有模块)
    │
    └─> app.Run()
```

### 4.3 启动时序图

```
[WPF Host]          [Bootstrapper]        [ModuleLoader]        [Modules]
     │                     │                      │                  │
     │──CreateBuilder()───>│                      │                  │
     │                     │                      │                  │
     │──ConfigureHost()──>│                      │                  │
     │                     │──ConfigureSerilog()─>│                  │
     │                     │                      │                  │
     │──RegisterServices()>│                      │                  │
     │                     │──LoadModules()──────>│                  │
     │                     │                      │──Discover()─────>│
     │                     │                      │<──Modules────────│
     │                     │<──Modules─────────────│                  │
     │                     │──ConfigureServices()>│                  │
     │                     │                      │                  │──PreConfigure()
     │                     │                      │                  │──Configure()
     │                     │                      │                  │──PostConfigure()
     │                     │<─────────────────────────────────────────│
     │<──Services──────────│                      │                  │
     │                     │                      │                  │
     │──Build()───────────>│                      │                  │
     │                     │                      │                  │
     │──BootstrapAsync()──>│                      │                  │
     │                     │──ValidateConfig()───>│                  │
     │                     │──DetectEnvironment()>│                  │
     │<──Result────────────│                      │                  │
     │                     │                      │                  │
     │──StartModulesAsync()>│                     │                  │
     │                     │──GetModules()───────>│                  │
     │                     │                      │<──Modules────────│
     │                     │──OnStarting()───────>│                  │
     │                     │                      │                  │──OnStarting()
     │                     │                      │                  │──OnStarted()
     │                     │<─────────────────────────────────────────│
     │<──Complete──────────│                      │                  │
     │                     │                      │                  │
     │──ShowWindow()───────>│                      │                  │
```

---

## 5. 模块生命周期管理

### 5.1 生命周期阶段

模块生命周期包含四个阶段：

1. **OnApplicationStartingAsync**：应用启动前
   - 时机：在 `BootstrapAsync()` 之后，应用完全启动之前
   - 用途：初始化资源、验证依赖、预热缓存等

2. **OnApplicationStartedAsync**：应用启动后
   - 时机：在应用完全启动之后
   - 用途：启动后台任务、注册事件处理器等

3. **OnApplicationStoppingAsync**：应用停止前
   - 时机：在应用开始关闭时
   - 用途：停止后台任务、保存状态等

4. **OnApplicationStoppedAsync**：应用停止后
   - 时机：在应用完全关闭后
   - 用途：清理资源、释放连接等

### 5.2 执行顺序

**启动顺序**：按模块依赖顺序执行（拓扑排序）

```
Module A (无依赖)
    └─> Module B (依赖 A)
        └─> Module C (依赖 B)
```

**停止顺序**：逆序执行（与启动顺序相反）

```
Module C
    └─> Module B
        └─> Module A
```

### 5.3 错误处理

**启动阶段**：

- 如果任何模块的 `OnApplicationStartingAsync` 或 `OnApplicationStartedAsync` 抛出异常，启动过程会失败
- 异常会被记录到日志并重新抛出

**停止阶段**：

- 如果任何模块的 `OnApplicationStoppingAsync` 或 `OnApplicationStoppedAsync` 抛出异常，不会阻止其他模块的停止
- 异常会被记录到日志，但不会重新抛出

---

## 6. 配置管理

### 6.1 配置验证

Bootstrapper 在启动时验证以下配置项：

1. **Lemoo 配置节**：必须存在
2. **模块配置**：`Lemoo:Modules` 必须存在
3. **数据库配置**：`Lemoo:Database` 必须存在

### 6.2 配置加载顺序

1. `appsettings.json`（必需）
2. `appsettings.{Environment}.json`（可选）
3. 环境变量（覆盖前面的配置）

### 6.3 配置示例

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

---

## 7. 扩展方法

### 7.1 HostBuilderExtensions

**文件位置**：`src/Hosts/Lemoo.Bootstrap/HostBuilderExtensions.cs`

**方法签名**：

```csharp
public static IHostBuilder ConfigureLemooApplication(
    this IHostBuilder hostBuilder,
    IConfiguration configuration,
    Action<IServiceCollection, IConfiguration>? configureServices = null)
```

**功能**：为 WPF/桌面应用配置 Lemoo 应用程序

**使用示例**：

```csharp
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var hostBuilder = new HostBuilder()
    .ConfigureLemooApplication(configuration, (services, config) =>
    {
        // 自定义服务注册
        services.AddSingleton<IMyService, MyService>();
    });

var host = hostBuilder.Build();
```

### 7.2 ApplicationBuilderExtensions

**文件位置**：`src/Hosts/Lemoo.Bootstrap/ApplicationBuilderExtensions.cs`

**方法签名**：

```csharp
public static async Task<WebApplication> ConfigureLemooApplicationAsync(
    this WebApplicationBuilder builder,
    Action<WebApplication>? configurePipeline = null)
```

**功能**：为 Web API 应用配置 Lemoo 应用程序

**使用示例**：

```csharp
var builder = WebApplication.CreateBuilder(args);

var app = await builder.ConfigureLemooApplicationAsync(configurePipeline: app =>
{
    app.UseRequestId();
    app.UseGlobalExceptionHandler();
    app.MapControllers();
});

app.Run();
```

### 7.3 ServiceCollectionExtensions

**文件位置**：`src/Hosts/Lemoo.Bootstrap/ServiceCollectionExtensions.cs`

**方法签名**：

```csharp
public static IServiceCollection AddLemooCore(
    this IServiceCollection services, 
    IConfiguration configuration)
```

**功能**：注册 Lemoo 核心服务

**注册的服务**：

- `IConfigurationService` → `ConfigurationService`
- `IDeploymentModeService` → `DeploymentModeService`
- `IModuleDbContextFactory` → `ModuleDbContextFactory`
- `ICacheService` → `MemoryCacheService`

---

## 8. 错误处理机制

### 8.1 错误类型

1. **配置错误**：配置验证失败
   - 错误代码：`MISSING_CONFIG`、`MISSING_MODULES_CONFIG`、`MISSING_DATABASE_CONFIG`

2. **引导错误**：引导过程异常
   - 错误代码：`BOOTSTRAP_ERROR`

3. **模块错误**：模块加载或生命周期异常
   - 记录到日志，可能抛出异常（启动时）或忽略（停止时）

### 8.2 错误处理策略

**配置验证失败**：

- 返回 `BootstrapResult`，`IsSuccess = false`
- 包含详细的错误信息
- 不抛出异常（由调用者决定如何处理）

**引导过程异常**：

- 捕获异常并记录日志
- 返回 `BootstrapResult`，`IsSuccess = false`
- 包含异常详情

**模块启动异常**：

- 记录日志
- 抛出异常（阻止应用启动）

**模块停止异常**：

- 记录日志
- 不抛出异常（允许其他模块正常停止）

---

## 9. 依赖关系

### 9.1 项目依赖

```
Lemoo.Bootstrap
├── Lemoo.Core.Abstractions
│   └── 核心接口定义
├── Lemoo.Core.Application
│   └── CQRS 管道行为扩展
├── Lemoo.Core.Infrastructure
│   ├── ModuleLoader
│   ├── 缓存服务
│   ├── 配置服务
│   ├── 部署模式服务
│   ├── 数据库上下文工厂
│   └── Serilog 配置
└── Lemoo.Modules.Abstractions
    └── ModuleBase
```

### 9.2 NuGet 包依赖

```xml
<PackageReference Include="Microsoft.Extensions.Hosting" Version="10.0.1" />
<PackageReference Include="Microsoft.Extensions.Hosting.Abstractions" Version="10.0.1" />
<PackageReference Include="Serilog.Extensions.Hosting" Version="10.0.0" />
<PackageReference Include="Serilog.Settings.Configuration" Version="10.0.0" />
```

---

## 10. 使用示例

### 10.1 WPF 应用示例

```csharp
public partial class App : Application
{
    private IHost? _host;
    
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        try
        {
            // 构建配置
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            
            // 创建Host Builder并配置Lemoo应用程序
            var hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.Sources.Clear();
                    config.AddConfiguration(configuration);
                })
                .ConfigureLemooApplication(configuration, (services, config) =>
                {
                    // 注册UI框架服务
                    RegisterUIFrameworkServices(services);
                    RegisterModuleUIs(services, config);
                });
            
            // 构建Host
            _host = hostBuilder.Build();
            
            // 获取引导器并执行引导
            var bootstrapper = _host.Services.GetRequiredService<Bootstrapper>();
            var bootstrapResult = await bootstrapper.BootstrapAsync();
            
            if (!bootstrapResult.IsSuccess)
            {
                MessageBox.Show(
                    $"应用程序启动失败:\n{string.Join("\n", bootstrapResult.Errors.Select(e => e.Message))}",
                    "启动错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
                return;
            }
            
            // 启动模块生命周期
            await bootstrapper.StartModulesAsync(_host.Services);
            
            // 初始化并显示主窗口
            InitializeMainWindow(_host.Services);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"应用程序启动时发生错误:\n{ex.Message}",
                "启动错误",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
        }
    }
    
    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host != null)
        {
            // 停止模块生命周期
            var bootstrapper = _host.Services.GetService<Bootstrapper>();
            if (bootstrapper != null)
            {
                await bootstrapper.StopModulesAsync(_host.Services);
            }
            
            // 停止Host
            await _host.StopAsync();
            _host.Dispose();
        }
        
        base.OnExit(e);
    }
}
```

### 10.2 Web API 应用示例

```csharp
var builder = WebApplication.CreateBuilder(args);

// 添加API特定服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 配置Lemoo应用程序
var app = await builder.ConfigureLemooApplicationAsync(configurePipeline: app =>
{
    // 配置中间件管道
    app.UseRequestId();
    app.UseGlobalExceptionHandler();
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    
    // 配置Swagger
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
});

app.Run();
```

---

## 11. 最佳实践

### 11.1 配置管理

✅ **推荐**：

- 使用 `appsettings.json` 存储默认配置
- 使用环境变量覆盖敏感配置
- 使用 `appsettings.{Environment}.json` 存储环境特定配置

❌ **不推荐**：

- 硬编码配置值
- 在代码中直接读取配置文件

### 11.2 错误处理

✅ **推荐**：

- 检查 `BootstrapResult.IsSuccess`
- 记录所有错误信息
- 向用户显示友好的错误消息

❌ **不推荐**：

- 忽略引导错误
- 不记录错误日志

### 11.3 模块开发

✅ **推荐**：

- 在 `OnApplicationStartingAsync` 中进行轻量级初始化
- 在 `OnApplicationStartedAsync` 中启动后台任务
- 在 `OnApplicationStoppingAsync` 中优雅地停止任务
- 在 `OnApplicationStoppedAsync` 中清理资源

❌ **不推荐**：

- 在生命周期方法中执行长时间运行的操作
- 忽略取消令牌
- 不处理异常

### 11.4 日志记录

✅ **推荐**：

- 记录关键操作和错误
- 使用结构化日志
- 包含足够的上下文信息

❌ **不推荐**：

- 记录敏感信息
- 过度日志记录（影响性能）

---

## 12. 性能分析

### 12.1 启动性能

**影响因素**：

1. **模块数量**：模块越多，加载时间越长
2. **模块复杂度**：模块配置和初始化复杂度影响启动时间
3. **数据库连接**：数据库连接和迁移可能影响启动时间
4. **日志配置**：日志配置复杂度影响启动时间

**优化建议**：

- 延迟加载非关键模块
- 异步执行模块初始化
- 优化数据库连接和迁移
- 简化日志配置

### 12.2 内存使用

**影响因素**：

1. **模块数量**：每个模块占用一定内存
2. **服务注册**：服务注册数量影响内存使用
3. **缓存配置**：缓存大小影响内存使用

**优化建议**：

- 使用适当的服务生命周期（Singleton、Scoped、Transient）
- 限制缓存大小
- 及时释放不需要的资源

---

## 13. 测试建议

### 13.1 单元测试

**测试目标**：

- `BootstrapAsync` 方法
- `ValidateConfiguration` 方法
- `RegisterServices` 方法
- `StartModulesAsync` 方法
- `StopModulesAsync` 方法

**测试用例**：

1. **配置验证测试**：
   - 有效配置
   - 缺少配置节
   - 无效配置值

2. **引导测试**：
   - 成功引导
   - 配置验证失败
   - 引导过程异常

3. **模块生命周期测试**：
   - 模块启动成功
   - 模块启动失败
   - 模块停止成功
   - 模块停止失败（不应抛出异常）

### 13.2 集成测试

**测试目标**：

- 完整的启动流程
- 模块加载和注册
- 服务依赖解析

**测试场景**：

1. **WPF 应用启动**：模拟完整的 WPF 应用启动流程
2. **Web API 应用启动**：模拟完整的 Web API 应用启动流程
3. **模块依赖**：测试模块依赖解析和加载顺序

---

## 14. 未来改进

### 14.1 计划中的改进

1. **启动性能监控**：
   - 添加详细的启动时间统计
   - 记录每个阶段的耗时
   - 提供性能报告

2. **健康检查**：
   - 在启动时执行健康检查
   - 验证数据库连接
   - 验证外部服务连接

3. **配置验证增强**：
   - 验证连接字符串有效性
   - 验证模块依赖
   - 验证环境变量

4. **StartupOptions 集成**：
   - 完全集成 `StartupOptions` 到启动流程
   - 支持自动数据库迁移
   - 支持数据种子

5. **启动脚本模板**：
   - 提供启动脚本模板
   - 支持不同环境的启动配置

### 14.2 潜在优化

1. **异步模块加载**：
   - 支持异步模块加载（如果可能）
   - 并行加载独立模块

2. **模块热重载**：
   - 支持模块热重载（开发环境）
   - 动态加载和卸载模块

3. **启动诊断**：
   - 提供启动诊断工具
   - 生成启动报告

---

## 附录

### A. 类和方法索引

#### IBootstrapper 接口

- `BootstrapAsync(CancellationToken)`：引导应用程序
- `RegisterServices(IServiceCollection, IConfiguration)`：注册服务
- `ConfigureHost(IHostBuilder)`：配置宿主

#### Bootstrapper 类

- `BootstrapAsync(CancellationToken)`：引导应用程序
- `RegisterServices(IServiceCollection, IConfiguration)`：注册服务
- `ConfigureHost(IHostBuilder)`：配置宿主
- `StartModulesAsync(IServiceProvider, CancellationToken)`：启动模块生命周期
- `StopModulesAsync(IServiceProvider, CancellationToken)`：停止模块生命周期
- `ValidateConfiguration()`：验证配置（私有方法）

#### 扩展方法

- `HostBuilderExtensions.ConfigureLemooApplication(...)`：配置 WPF/桌面应用
- `ApplicationBuilderExtensions.ConfigureLemooApplicationAsync(...)`：配置 Web API 应用
- `ServiceCollectionExtensions.AddLemooCore(...)`：注册核心服务

### B. 配置参考

#### appsettings.json 结构

```json
{
  "Lemoo": {
    "Mode": "Local",
    "Modules": {
      "Enabled": [ "*" ],
      "Path": "./Modules",
      "AutoLoad": true,
      "HotReload": false
    },
    "Database": {
      "Provider": "SqlServer",
      "AutoMigrate": true,
      "ConnectionStrings": {
        "Example": "..."
      }
    }
  }
}
```

### C. 错误代码参考

| 错误代码 | 说明 | 解决方案 |
|---------|------|---------|
| `MISSING_CONFIG` | 缺少 'Lemoo' 配置节 | 添加 `Lemoo` 配置节 |
| `MISSING_MODULES_CONFIG` | 缺少 'Lemoo:Modules' 配置节 | 添加 `Lemoo:Modules` 配置节 |
| `MISSING_DATABASE_CONFIG` | 缺少 'Lemoo:Database' 配置节 | 添加 `Lemoo:Database` 配置节 |
| `BOOTSTRAP_ERROR` | 引导过程错误 | 查看日志获取详细错误信息 |

---

**文档版本**：1.0.0  
**最后更新**：2025年1月  
**维护者**：Lemoo 开发团队
