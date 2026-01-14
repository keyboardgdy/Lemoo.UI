# Bootstrapper 架构优化总结

## 优化目标

优化两个启动项（Lemoo.Api 和 Lemoo.Host）和 Bootstrapper，实现统一的启动流程和最佳实践。

## 主要改进

### 1. ✅ 创建统一的启动扩展方法

#### ApplicationBuilderExtensions（Web API）
- **文件**: `src/Hosts/Lemoo.Bootstrap/ApplicationBuilderExtensions.cs`
- **功能**: 提供 `ConfigureLemooApplicationAsync` 方法，统一处理 Web API 应用的启动流程
- **优势**: 
  - 消除代码重复
  - 统一的错误处理
  - 简化的启动代码

#### HostBuilderExtensions（WPF/桌面应用）
- **文件**: `src/Hosts/Lemoo.Bootstrap/HostBuilderExtensions.cs`
- **功能**: 提供 `ConfigureLemooApplication` 方法，统一处理桌面应用的启动流程
- **优势**:
  - 支持自定义服务配置
  - 统一的配置加载
  - 灵活的扩展点

### 2. ✅ 增强 Bootstrapper

#### 模块生命周期管理
- **新增方法**:
  - `StartModulesAsync`: 启动模块生命周期（启动前和启动后）
  - `StopModulesAsync`: 停止模块生命周期（停止前和停止后）
- **优势**:
  - 集中的生命周期管理
  - 完整的错误处理和日志记录
  - 支持依赖顺序

### 3. ✅ 优化启动项代码

#### Lemoo.Api (Web API)
**优化前**: 94 行代码，包含重复的模块生命周期管理
**优化后**: 57 行代码，使用统一的扩展方法

**主要改进**:
- 使用 `ConfigureLemooApplicationAsync` 简化启动流程
- 通过回调函数配置中间件管道
- 自动处理引导和模块生命周期

#### Lemoo.Host (WPF)
**优化前**: 包含重复的模块生命周期管理代码（StartModulesAsync 和 StopModulesAsync）
**优化后**: 使用 Bootstrapper 的统一方法

**主要改进**:
- 使用 `ConfigureLemooApplication` 统一配置
- 移除重复的生命周期管理代码
- 通过回调函数支持 UI 特定服务注册

### 4. ✅ 创建配置选项类

#### StartupOptions
- **文件**: `src/Hosts/Lemoo.Bootstrap/StartupOptions.cs`
- **功能**: 统一管理启动配置选项
- **选项**:
  - `AutoMigrate`: 自动数据库迁移
  - `SeedData`: 数据种子
  - `ThrowOnBootstrapFailure`: 启动失败时抛出异常
  - `EnableModuleLifecycleLogging`: 模块生命周期日志

## 架构对比

### 优化前

```
Lemoo.Api/Program.cs
├── 创建引导器
├── 配置Host
├── 注册服务
├── 构建应用
├── 配置管道
├── 引导应用程序
└── 手动管理模块生命周期 ❌

Lemoo.Host/App.xaml.cs
├── 创建引导器
├── 配置Host
├── 注册服务
├── 构建Host
├── 引导应用程序
└── 手动管理模块生命周期 ❌
```

### 优化后

```
Lemoo.Api/Program.cs
└── ConfigureLemooApplicationAsync ✅
    ├── 自动创建引导器
    ├── 自动配置Host
    ├── 自动注册服务
    ├── 自动引导
    └── 自动启动模块生命周期

Lemoo.Host/App.xaml.cs
└── ConfigureLemooApplication ✅
    ├── 自动创建引导器
    ├── 自动配置Host
    ├── 自动注册服务
    └── 通过Bootstrapper管理生命周期
```

## 代码质量改进

### 1. 代码重复消除
- **之前**: 两个启动项都有相同的模块生命周期管理代码
- **现在**: 统一在 Bootstrapper 中管理

### 2. 一致性
- **之前**: 启动流程可能不一致
- **现在**: 统一的启动流程确保行为一致

### 3. 可维护性
- **之前**: 修改启动逻辑需要在多个地方修改
- **现在**: 只需在扩展方法中修改

### 4. 可测试性
- **之前**: 启动逻辑分散，难以测试
- **现在**: 启动逻辑封装，易于测试

## 使用示例

### Web API 启动项

```csharp
var builder = WebApplication.CreateBuilder(args);

// 添加API特定服务
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// 配置Lemoo应用程序（一行代码完成所有启动逻辑）
var app = await builder.ConfigureLemooApplicationAsync(configurePipeline: app =>
{
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

// 配置Lemoo应用程序
var hostBuilder = new HostBuilder()
    .ConfigureLemooApplication(configuration, (services, config) =>
    {
        // 自定义服务注册
        RegisterUIFrameworkServices(services);
        RegisterModuleUIs(services, config);
    });

var host = hostBuilder.Build();

// 引导和启动（使用Bootstrapper统一方法）
var bootstrapper = host.Services.GetRequiredService<Bootstrapper>();
await bootstrapper.BootstrapAsync();
await bootstrapper.StartModulesAsync(host.Services);
```

## 最佳实践实现

1. ✅ **DRY原则**: 消除代码重复
2. ✅ **单一职责**: 每个扩展方法职责明确
3. ✅ **依赖注入**: 通过DI管理Bootstrapper
4. ✅ **错误处理**: 统一的错误处理和日志记录
5. ✅ **可扩展性**: 通过回调函数支持自定义配置
6. ✅ **可测试性**: 启动逻辑封装，易于测试

## 编译验证

✅ **Lemoo.Api**: 编译成功，0 个警告，0 个错误
✅ **Lemoo.Host**: 编译成功，0 个警告，0 个错误
✅ **Lemoo.Bootstrap**: 编译成功，0 个警告，0 个错误

## 后续改进建议

1. **启动性能监控**: 添加启动时间统计
2. **健康检查**: 在启动时执行健康检查
3. **配置验证**: 增强配置验证逻辑
4. **依赖检查**: 启动时检查模块依赖
5. **启动脚本**: 提供启动脚本模板

## 总结

通过本次优化，我们实现了：
- ✅ 统一的启动流程
- ✅ 消除代码重复
- ✅ 提高可维护性
- ✅ 改善代码质量
- ✅ 遵循最佳实践

架构现在更加清晰、一致和易于维护。

