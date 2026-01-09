# 模块加载机制重构计划

## 1. 现状分析

### 1.1 当前架构
当前的模块加载机制位于 `Lemoo.Core.Infrastructure.ModuleLoader` 命名空间，主要包含以下组件：
- `ModuleLoader`：实现了 `IModuleLoader` 接口，负责发现、加载和初始化模块
- 发现策略混合在同一个类中，缺乏扩展性

### 1.2 主要问题

#### 1.2.1 程序集发现策略单一
当前实现仅支持两种发现方式：
1. 从文件系统目录（`./Modules`）加载 DLL
2. 从 `AppDomain.CurrentDomain` 已加载程序集发现

问题：
- 硬编码路径 `"./Modules"`
- `AppDomain.CurrentDomain.GetAssemblies()` 可能不包含通过反射加载的程序集
- 缺乏多路径支持

#### 1.2.2 模块命名约定脆弱
```csharp
var moduleName = moduleType.Name.Replace("Module", "");
```
- 依赖特定的命名后缀
- 没有容错机制
- 不支持配置化命名约定

#### 1.2.3 同步加载风险
在 `Bootstrapper.RegisterServices` 方法中：
```csharp
_loadedModules = moduleLoader.LoadModulesAsync().GetAwaiter().GetResult();
```
- 潜在的死锁风险
- 不符合异步最佳实践

#### 1.2.4 配置不灵活
```csharp
var modulePath = _configuration["Lemoo:Modules:Path"] ?? "./Modules";
```
- 只支持单个路径
- 缺乏程序集过滤机制
- 不支持环境特定的配置

#### 1.2.5 模块UI发现问题
在 `Host` 应用中，模块UI程序集可能未被加载到 `AppDomain`，导致 `DiscoverModuleUIs` 方法失败。

## 2. 重构目标

### 2.1 核心目标
1. **策略化设计**：将发现策略与加载逻辑分离
2. **灵活配置**：支持多路径、过滤器、环境特定配置
3. **异步友好**：完全异步的加载流程
4. **扩展性**：支持自定义发现器和过滤器
5. **健壮性**：更好的错误处理和恢复机制

### 2.2 非功能性目标
1. **向后兼容**：现有模块应无需修改
2. **性能优化**：并行发现和验证
3. **日志完善**：详细的加载过程日志

## 3. 设计方案

### 3.1 架构概览
```
Lemoo.Core.Abstractions.ModuleLoader/
├── IAssemblyDiscoveryStrategy (新接口)
├── IModuleDiscoverer (扩展接口)
├── IModuleNameExtractor (新接口)
└── IModuleFilter (新接口)

Lemoo.Core.Infrastructure.ModuleLoader/
├── Strategies/
│   ├── FileSystemDiscoveryStrategy
│   ├── AppDomainDiscoveryStrategy
│   ├── ConfiguredAssembliesDiscoveryStrategy
│   └── CompositeDiscoveryStrategy
├── Extractors/
│   ├── ConventionModuleNameExtractor
│   └── AttributeModuleNameExtractor
├── Filters/
│   ├── WhitelistModuleFilter
│   ├── BlacklistModuleFilter
│   └── EnvironmentModuleFilter
├── ModuleDiscoverer (重构实现)
└── ModuleLoader (精简后的核心加载器)
```

### 3.2 接口设计

#### 3.2.1 IAssemblyDiscoveryStrategy
```csharp
public interface IAssemblyDiscoveryStrategy
{
    Task<IEnumerable<Assembly>> DiscoverAssembliesAsync(
        ModuleDiscoveryContext context,
        CancellationToken cancellationToken);

    string Name { get; }
    int Order { get; }
}
```

#### 3.2.2 IModuleDiscoverer（扩展）
```csharp
public interface IModuleDiscoverer
{
    Task<IEnumerable<IModule>> DiscoverModulesAsync(
        ModuleDiscoveryContext context,
        CancellationToken cancellationToken);

    Task<IEnumerable<IModuleUI>> DiscoverModuleUIsAsync(
        ModuleDiscoveryContext context,
        CancellationToken cancellationToken);
}
```

#### 3.2.3 IModuleNameExtractor
```csharp
public interface IModuleNameExtractor
{
    string ExtractModuleName(Type moduleType);
    bool CanExtract(Type moduleType);
}
```

#### 3.2.4 IModuleFilter
```csharp
public interface IModuleFilter
{
    bool ShouldInclude(IModule module, ModuleDiscoveryContext context);
    string Name { get; }
}
```

### 3.3 关键组件

#### 3.3.1 ModuleDiscoveryContext
包含发现过程所需的所有上下文信息：
- 配置信息
- 环境信息
- 发现选项
- 策略和过滤器配置

#### 3.3.2 CompositeDiscoveryStrategy
组合多个策略，按顺序执行：
```csharp
public class CompositeDiscoveryStrategy : IAssemblyDiscoveryStrategy
{
    private readonly IEnumerable<IAssemblyDiscoveryStrategy> _strategies;

    public async Task<IEnumerable<Assembly>> DiscoverAssembliesAsync(
        ModuleDiscoveryContext context,
        CancellationToken cancellationToken)
    {
        var assemblies = new HashSet<Assembly>();

        foreach (var strategy in _strategies.OrderBy(s => s.Order))
        {
            var discovered = await strategy.DiscoverAssembliesAsync(context, cancellationToken);
            foreach (var assembly in discovered)
            {
                assemblies.Add(assembly);
            }
        }

        return assemblies;
    }
}
```

#### 3.3.3 ModuleDiscoverer（重构）
```csharp
public class ModuleDiscoverer : IModuleDiscoverer
{
    private readonly IAssemblyDiscoveryStrategy _discoveryStrategy;
    private readonly IModuleNameExtractor _nameExtractor;
    private readonly IEnumerable<IModuleFilter> _filters;

    public async Task<IEnumerable<IModule>> DiscoverModulesAsync(
        ModuleDiscoveryContext context,
        CancellationToken cancellationToken)
    {
        // 1. 发现程序集
        var assemblies = await _discoveryStrategy.DiscoverAssembliesAsync(context, cancellationToken);

        // 2. 实例化模块
        var modules = new List<IModule>();
        foreach (var assembly in assemblies)
        {
            var moduleTypes = assembly.GetTypes()
                .Where(t => typeof(IModule).IsAssignableFrom(t)
                         && !t.IsAbstract
                         && !t.IsInterface);

            foreach (var type in moduleTypes)
            {
                try
                {
                    var module = (IModule)Activator.CreateInstance(type)!;

                    // 3. 应用过滤器
                    if (_filters.All(f => f.ShouldInclude(module, context)))
                    {
                        modules.Add(module);
                    }
                }
                catch (Exception ex)
                {
                    context.Logger?.LogWarning(ex, "Failed to instantiate module type {Type}", type.Name);
                }
            }
        }

        return modules;
    }
}
```

### 3.4 配置增强

#### 3.4.1 新配置结构
```json
{
  "Lemoo": {
    "Modules": {
      "Discovery": {
        "Strategies": [
          {
            "Type": "FileSystem",
            "Paths": [
              "./Modules",
              "./CustomModules"
            ],
            "SearchPattern": "*.dll",
            "Recursive": true
          },
          {
            "Type": "AppDomain",
            "AssemblyPatterns": [
              "Lemoo.Modules.*"
            ]
          },
          {
            "Type": "Configured",
            "Assemblies": [
              "Lemoo.Modules.TaskManager",
              "Lemoo.Modules.Example"
            ]
          }
        ],
        "NameExtractor": {
          "Strategy": "Convention",
          "Suffix": "Module",
          "RemoveSuffix": true
        },
        "Filters": [
          {
            "Type": "Whitelist",
            "Modules": [ "*" ]
          },
          {
            "Type": "Environment",
            "Development": [ "*" ],
            "Production": [ "TaskManager", "Example" ]
          }
        ]
      }
    }
  }
}
```

## 4. 实现步骤

### 阶段1：基础接口和策略（2-3天）
1. 在 `Lemoo.Core.Abstractions` 中定义新接口
2. 实现基础发现策略：`FileSystemDiscoveryStrategy`、`AppDomainDiscoveryStrategy`
3. 实现 `CompositeDiscoveryStrategy`
4. 创建 `ModuleDiscoveryContext`

### 阶段2：重构 ModuleDiscoverer（2天）
1. 重构现有 `ModuleDiscoverer` 实现
2. 集成策略模式
3. 添加过滤器支持
4. 实现 `IModuleNameExtractor`

### 阶段3：配置系统和集成（2天）
1. 设计新的配置模型
2. 实现配置绑定
3. 更新 `Bootstrapper` 以使用新系统
4. 更新 `Host` 和 `Api` 项目配置

### 阶段4：异步优化和错误处理（1-2天）
1. 移除所有 `GetAwaiter().GetResult()` 调用
2. 实现完全异步的加载流程
3. 添加详细的错误处理和日志
4. 实现重试机制

### 阶段5：测试和文档（2天）
1. 编写单元测试
2. 更新现有测试
3. 编写配置文档
4. 更新架构文档

## 5. 详细设计

### 5.1 策略实现细节

#### 5.1.1 FileSystemDiscoveryStrategy
```csharp
public class FileSystemDiscoveryStrategy : IAssemblyDiscoveryStrategy
{
    public string Name => "FileSystem";
    public int Order => 100; // 较低优先级

    public async Task<IEnumerable<Assembly>> DiscoverAssembliesAsync(
        ModuleDiscoveryContext context,
        CancellationToken cancellationToken)
    {
        var assemblies = new List<Assembly>();
        var options = context.Options.FileSystemOptions;

        foreach (var path in options.Paths)
        {
            var fullPath = Path.GetFullPath(path);
            if (!Directory.Exists(fullPath))
            {
                context.Logger?.LogWarning("Module path does not exist: {Path}", fullPath);
                continue;
            }

            var dllFiles = Directory.GetFiles(
                fullPath,
                options.SearchPattern,
                options.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            foreach (var dllFile in dllFiles)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dllFile);
                    if (IsModuleAssembly(assembly, context))
                    {
                        assemblies.Add(assembly);
                    }
                }
                catch (Exception ex)
                {
                    context.Logger?.LogWarning(ex, "Failed to load assembly: {File}", dllFile);
                }
            }
        }

        return assemblies;
    }
}
```

#### 5.1.2 AppDomainDiscoveryStrategy
```csharp
public class AppDomainDiscoveryStrategy : IAssemblyDiscoveryStrategy
{
    public string Name => "AppDomain";
    public int Order => 200; // 中等优先级

    public Task<IEnumerable<Assembly>> DiscoverAssembliesAsync(
        ModuleDiscoveryContext context,
        CancellationToken cancellationToken)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => IsModuleAssembly(assembly, context))
            .ToList();

        return Task.FromResult<IEnumerable<Assembly>>(assemblies);
    }
}
```

#### 5.1.3 ReflectionLoadDiscoveryStrategy（新增）
解决模块UI程序集未被加载的问题：
```csharp
public class ReflectionLoadDiscoveryStrategy : IAssemblyDiscoveryStrategy
{
    public string Name => "ReflectionLoad";
    public int Order => 50; // 最高优先级

    public async Task<IEnumerable<Assembly>> DiscoverAssembliesAsync(
        ModuleDiscoveryContext context,
        CancellationToken cancellationToken)
    {
        var assemblies = new List<Assembly>();
        var options = context.Options.ReflectionLoadOptions;

        // 尝试显式加载配置的程序集
        foreach (var assemblyName in options.AssemblyNames)
        {
            try
            {
                var assembly = Assembly.Load(assemblyName);
                if (IsModuleAssembly(assembly, context))
                {
                    assemblies.Add(assembly);
                }
            }
            catch (Exception ex)
            {
                context.Logger?.LogWarning(ex, "Failed to load assembly by name: {Name}", assemblyName);
            }
        }

        return assemblies;
    }
}
```

### 5.2 异步优化

#### 5.2.1 重构 Bootstrapper.RegisterServices
```csharp
public async Task RegisterServicesAsync(
    IServiceCollection services,
    IConfiguration configuration,
    CancellationToken cancellationToken = default)
{
    // 创建模块发现器
    var discoverer = CreateModuleDiscoverer(configuration);

    // 异步发现模块
    var context = new ModuleDiscoveryContext(configuration);
    _loadedModules = (await discoverer.DiscoverModulesAsync(context, cancellationToken)).ToList();

    // 异步配置模块服务
    foreach (var module in _loadedModules)
    {
        module.PreConfigureServices(services, configuration);
        module.ConfigureServices(services, configuration);
    }

    foreach (var module in _loadedModules)
    {
        module.PostConfigureServices(services, configuration);
    }
}
```

#### 5.2.2 并行发现优化
```csharp
public async Task<IEnumerable<Assembly>> DiscoverAssembliesAsync(
    ModuleDiscoveryContext context,
    CancellationToken cancellationToken)
{
    var tasks = _strategies
        .OrderBy(s => s.Order)
        .Select(strategy => strategy.DiscoverAssembliesAsync(context, cancellationToken))
        .ToList();

    var results = await Task.WhenAll(tasks);

    // 合并结果，去重
    var assemblies = new HashSet<Assembly>();
    foreach (var result in results)
    {
        foreach (var assembly in result)
        {
            assemblies.Add(assembly);
        }
    }

    return assemblies;
}
```

### 5.3 错误处理和恢复

#### 5.3.1 模块加载异常处理
```csharp
public class ModuleLoadException : Exception
{
    public string ModuleName { get; }
    public ModuleLoadErrorType ErrorType { get; }

    public ModuleLoadException(
        string moduleName,
        ModuleLoadErrorType errorType,
        string message,
        Exception? innerException = null)
        : base(message, innerException)
    {
        ModuleName = moduleName;
        ErrorType = errorType;
    }
}

public enum ModuleLoadErrorType
{
    AssemblyNotFound,
    TypeResolutionFailed,
    InstantiationFailed,
    DependencyMissing,
    CircularDependency
}
```

#### 5.3.2 优雅降级
```csharp
public async Task<IEnumerable<IModule>> DiscoverModulesAsync(
    ModuleDiscoveryContext context,
    CancellationToken cancellationToken)
{
    var modules = new List<IModule>();

    foreach (var strategy in _strategies)
    {
        try
        {
            var discovered = await strategy.DiscoverAssembliesAsync(context, cancellationToken);
            // 处理发现的程序集...
        }
        catch (Exception ex)
        {
            context.Logger?.LogError(ex, "Strategy {StrategyName} failed, continuing with next strategy", strategy.Name);
            // 继续执行下一个策略
            continue;
        }
    }

    return modules;
}
```

## 6. 向后兼容性

### 6.1 配置兼容
- 保持现有的 `Lemoo:Modules:Path` 配置向后兼容
- 自动将旧配置转换为新格式
- 默认启用 `FileSystem` 和 `AppDomain` 策略

### 6.2 API兼容
- 保持 `IModuleLoader` 接口不变
- 内部实现使用新的策略系统
- 现有的 `GetLoadedModules()`、`GetModule()` 等方法保持相同签名

### 6.3 模块兼容
- 现有的模块无需修改
- 模块命名约定保持向后兼容
- 依赖解析逻辑保持不变

## 7. 风险评估和缓解

### 7.1 主要风险
1. **性能影响**：更复杂的发现逻辑可能影响启动时间
2. **配置复杂性**：新配置系统可能增加学习曲线
3. **向后兼容性**：现有应用可能依赖特定行为

### 7.2 缓解措施
1. **性能优化**：实现并行发现、缓存机制
2. **配置简化**：提供合理的默认配置
3. **兼容性测试**：全面的测试套件
4. **渐进式迁移**：提供迁移指南和工具

## 8. 验收标准

### 8.1 功能要求
1. ✅ 支持多种发现策略配置
2. ✅ 支持多路径模块搜索
3. ✅ 完全异步的加载流程
4. ✅ 详细的加载过程日志
5. ✅ 环境特定的模块过滤
6. ✅ 向后兼容现有配置

### 8.2 非功能要求
1. ✅ 启动时间增加不超过 10%
2. ✅ 内存使用增加不超过 5%
3. ✅ 100% 的单元测试覆盖率
4. ✅ 完整的 API 文档
5. ✅ 配置示例和迁移指南

## 9. 后续优化方向

### 9.1 短期优化（v1.1）
1. 实现模块元数据缓存
2. 添加热重载支持
3. 实现模块依赖图的图形化展示

### 9.2 中期优化（v2.0）
1. 支持远程模块仓库
2. 模块版本管理和冲突解决
3. 模块签名和安全性验证

### 9.3 长期优化（v3.0）
1. 模块市场集成
2. 动态模块安装/卸载
3. 模块沙箱和权限控制

---

**计划版本**：1.0
**预估工作量**：7-9 人日
**优先级**：高
**影响范围**：核心基础设施层