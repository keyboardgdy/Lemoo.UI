using System.Diagnostics;
using System.Reflection;
using Lemoo.Core.Abstractions.Module;
using Lemoo.Core.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lemoo.Core.Infrastructure.ModuleLoader;

/// <summary>
/// 隔离式模块加载器 - 支持程序集隔离和可卸载
/// </summary>
public class IsolatedModuleLoader : IModuleLoader
{
    private readonly ILogger<IsolatedModuleLoader> _logger;
    private readonly IConfiguration _configuration;
    private readonly IEnumerable<IModuleDiscoveryStrategy> _discoveryStrategies;
    private readonly ModuleVersionCompatibilityChecker _versionChecker;
    private readonly ILoggerFactory _loggerFactory;
    private readonly Dictionary<string, ModuleContainer> _moduleContainers = new();

    public event EventHandler<ModuleLoadingEventArgs>? ModuleLoading;
    public event EventHandler<ModuleLoadedEventArgs>? ModuleLoaded;
    public event EventHandler<ModuleUnloadingEventArgs>? ModuleUnloading;

    public IsolatedModuleLoader(
        ILogger<IsolatedModuleLoader> logger,
        IConfiguration configuration,
        IEnumerable<IModuleDiscoveryStrategy> discoveryStrategies,
        ModuleVersionCompatibilityChecker versionChecker,
        ILoggerFactory loggerFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _discoveryStrategies = discoveryStrategies;
        _versionChecker = versionChecker;
        _loggerFactory = loggerFactory;
    }

    public async Task<IReadOnlyList<IModule>> LoadModulesAsync(CancellationToken cancellationToken = default)
    {
        var result = await LoadModulesWithResultAsync(cancellationToken);
        if (!result.Success)
        {
            throw new Exception($"模块加载失败: {result.ErrorMessage}");
        }
        return result.Modules;
    }

    public async Task<ModuleLoadResult> LoadModulesWithResultAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var enabledModulesSection = _configuration.GetSection("Lemoo:Modules:Enabled");
            var enabledModules = enabledModulesSection.Get<string[]>() ?? Array.Empty<string>();

            _logger.LogInformation("开始加载模块（隔离模式）...");

            // 1. 使用策略发现模块
            var moduleAssemblies = await DiscoverModuleAssembliesAsync(cancellationToken);

            // 2. 为每个模块创建独立的加载上下文
            var modules = new List<IModule>();
            foreach (var assemblyInfo in moduleAssemblies)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var assembly = assemblyInfo.Assembly;
                var modulePath = assemblyInfo.Path;

                try
                {
                    var moduleTypes = assembly.GetTypes()
                        .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
                        .ToList();

                    foreach (var moduleType in moduleTypes)
                    {
                        var moduleName = moduleType.Name.Replace("Module", "");

                        if (enabledModules.Length > 0 && !enabledModules.Contains(moduleName) && !enabledModules.Contains("*"))
                        {
                            _logger.LogDebug("模块已禁用: {ModuleName}", moduleName);
                            continue;
                        }

                        try
                        {
                            // 触发加载前事件
                            ModuleLoading?.Invoke(this, new ModuleLoadingEventArgs(moduleName, modulePath));

                            // 创建模块加载上下文
                            var loadContext = CreateModuleLoadContext(moduleName, modulePath ?? assembly.Location);

                            // 在隔离上下文中创建模块实例
                            var module = CreateModuleInContext(loadContext, moduleType, assembly);

                            // 创建模块容器
                            var container = new ModuleContainer
                            {
                                Name = moduleName,
                                Module = module,
                                LoadContext = loadContext,
                                Assembly = assembly,
                                LoadedAt = DateTime.UtcNow
                            };

                            _moduleContainers[moduleName] = container;
                            modules.Add(module);

                            var loadDuration = stopwatch.Elapsed;
                            _logger.LogInformation("成功加载模块: {ModuleName} v{Version} (耗时: {Duration}ms)",
                                module.Name, module.Version, loadDuration.TotalMilliseconds);

                            // 触发加载后事件
                            ModuleLoaded?.Invoke(this, new ModuleLoadedEventArgs(moduleName, module, loadDuration));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "无法实例化模块: {ModuleType}", moduleType.Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "无法检查程序集: {AssemblyName}", assembly.GetName().Name);
                }
            }

            // 3. 验证依赖关系（包括版本兼容性）
            ValidateModuleDependenciesWithVersions(modules);

            // 4. 按依赖关系排序
            var sortedModules = SortModulesByDependencies(modules);

            stopwatch.Stop();
            _logger.LogInformation("成功加载 {Count} 个模块，总耗时: {ElapsedMilliseconds}ms",
                sortedModules.Count, stopwatch.ElapsedMilliseconds);

            return ModuleLoadResult.Successful(sortedModules, stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "模块加载失败，耗时: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
            return ModuleLoadResult.Failed(ex.Message, stopwatch.Elapsed);
        }
    }

    private async Task<List<AssemblyInfo>> DiscoverModuleAssembliesAsync(CancellationToken cancellationToken)
    {
        var assemblies = new List<AssemblyInfo>();
        var processedAssemblyPaths = new HashSet<string>();

        foreach (var strategy in _discoveryStrategies)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                _logger.LogDebug("使用策略发现模块: {StrategyType}", strategy.GetType().Name);
                var discoveredAssemblies = await strategy.DiscoverModulesAsync(cancellationToken);

                foreach (var assembly in discoveredAssemblies)
                {
                    var location = assembly.Location;
                    if (!string.IsNullOrEmpty(location) && !processedAssemblyPaths.Contains(location))
                    {
                        assemblies.Add(new AssemblyInfo { Assembly = assembly, Path = location });
                        processedAssemblyPaths.Add(location);
                    }
                    else if (string.IsNullOrEmpty(location))
                    {
                        assemblies.Add(new AssemblyInfo { Assembly = assembly, Path = null });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "模块发现策略执行失败: {StrategyType}", strategy.GetType().Name);
            }
        }

        _logger.LogDebug("发现 {Count} 个唯一的模块程序集", assemblies.Count);
        return assemblies;
    }

    private ModuleLoadContext CreateModuleLoadContext(string moduleName, string modulePath)
    {
        _logger.LogDebug("为模块 {ModuleName} 创建隔离加载上下文: {ModulePath}", moduleName, modulePath);
        var moduleLogger = _loggerFactory.CreateLogger<ModuleLoadContext>();
        return new ModuleLoadContext(moduleName, modulePath, moduleLogger);
    }

    private IModule CreateModuleInContext(ModuleLoadContext loadContext, Type moduleType, Assembly originalAssembly)
    {
        // 尝试在隔离上下文中加载模块类型
        var isolatedAssembly = loadContext.LoadFromAssemblyPath(originalAssembly.Location);
        var isolatedModuleType = isolatedAssembly.GetType(moduleType.FullName ?? moduleType.Name);

        if (isolatedModuleType == null)
        {
            throw new InvalidOperationException($"无法在隔离上下文中找到模块类型: {moduleType.FullName}");
        }

        return (IModule)Activator.CreateInstance(isolatedModuleType)!;
    }

    public void ValidateModuleDependencies(IReadOnlyList<IModule> modules)
    {
        ValidateModuleDependenciesWithVersions(modules);
    }

    private void ValidateModuleDependenciesWithVersions(IReadOnlyList<IModule> modules)
    {
        // 1. 基本依赖检查
        var moduleNames = modules.Select(m => m.Name).ToHashSet();

        foreach (var module in modules)
        {
            foreach (var dependency in module.Dependencies)
            {
                if (!moduleNames.Contains(dependency))
                {
                    throw new ModuleDependencyException(
                        module.Name,
                        $"模块 '{module.Name}' 依赖的模块 '{dependency}' 未找到");
                }
            }
        }

        // 2. 版本兼容性检查
        var compatibilityResult = _versionChecker.ValidateCompatibility(modules);

        if (!compatibilityResult.IsCompatible)
        {
            var errorMessages = compatibilityResult.Errors.Select(e => e.Message).ToList();
            throw new ModuleDependencyException(
                "ModuleSystem",
                $"模块版本兼容性检查失败:\n{string.Join("\n", errorMessages)}");
        }

        // 3. 记录警告
        foreach (var warning in compatibilityResult.Warnings)
        {
            _logger.LogWarning("{Message}", warning.Message);
        }
    }

    private IReadOnlyList<IModule> SortModulesByDependencies(IReadOnlyList<IModule> modules)
    {
        var sorted = new List<IModule>();
        var visited = new HashSet<string>();
        var visiting = new HashSet<string>();

        void Visit(IModule module)
        {
            if (visiting.Contains(module.Name))
                throw new CircularDependencyException(module.Name, $"检测到循环依赖: {module.Name}");

            if (visited.Contains(module.Name))
                return;

            visiting.Add(module.Name);

            // 处理旧版本的依赖
            foreach (var depName in module.Dependencies)
            {
                var dep = modules.FirstOrDefault(m => m.Name == depName);
                if (dep != null)
                    Visit(dep);
            }

            // 处理新版本的依赖
            foreach (var dep in module.DependencyModules)
            {
                var depModule = modules.FirstOrDefault(m => m.Name == dep.ModuleName);
                if (depModule != null)
                    Visit(depModule);
            }

            visiting.Remove(module.Name);
            visited.Add(module.Name);
            sorted.Add(module);
        }

        foreach (var module in modules)
        {
            if (!visited.Contains(module.Name))
                Visit(module);
        }

        return sorted;
    }

    public IReadOnlyList<IModule> GetLoadedModules()
    {
        return _moduleContainers.Values.Select(c => c.Module).ToList().AsReadOnly();
    }

    public IModule? GetModule(string moduleName)
    {
        return _moduleContainers.TryGetValue(moduleName, out var container) ? container.Module : null;
    }

    public Task<bool> UnloadModuleAsync(string moduleName, TimeSpan timeout = default)
    {
        if (!_moduleContainers.TryGetValue(moduleName, out var container))
        {
            _logger.LogWarning("无法卸载不存在的模块: {ModuleName}", moduleName);
            return Task.FromResult(false);
        }

        // 触发卸载前事件
        ModuleUnloading?.Invoke(this, new ModuleUnloadingEventArgs(moduleName));

        try
        {
            // 卸载模块加载上下文
            var unloadTask = container.LoadContext.UnloadAsync(timeout);
            var success = unloadTask.GetAwaiter().GetResult();

            if (success)
            {
                _moduleContainers.Remove(moduleName);
                _logger.LogInformation("已卸载模块: {ModuleName}", moduleName);
            }

            return Task.FromResult(success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "卸载模块失败: {ModuleName}", moduleName);
            return Task.FromResult(false);
        }
    }

    public Task UnloadAllModulesAsync(TimeSpan timeout = default)
    {
        var moduleNames = _moduleContainers.Keys.ToList();
        var tasks = moduleNames.Select(name => UnloadModuleAsync(name, timeout));
        return Task.WhenAll(tasks);
    }

    public async Task<IModule?> ReloadModuleAsync(string moduleName, CancellationToken cancellationToken = default)
    {
        // 先卸载
        var unloadSuccess = await UnloadModuleAsync(moduleName);
        if (!unloadSuccess)
        {
            _logger.LogWarning("重新加载模块失败，无法卸载: {ModuleName}", moduleName);
            return null;
        }

        // 等待 GC 回收
        await Task.Delay(100, cancellationToken);
        GC.Collect();
        GC.WaitForPendingFinalizers();

        // 重新加载所有模块
        var result = await LoadModulesWithResultAsync(cancellationToken);
        if (!result.Success)
        {
            _logger.LogError("重新加载模块失败: {ErrorMessage}", result.ErrorMessage);
            return null;
        }

        return GetModule(moduleName);
    }

    /// <summary>
    /// 模块容器 - 存储模块及其加载上下文
    /// </summary>
    private class ModuleContainer
    {
        public string Name { get; set; } = string.Empty;
        public IModule Module { get; set; } = null!;
        public ModuleLoadContext LoadContext { get; set; } = null!;
        public Assembly Assembly { get; set; } = null!;
        public DateTime LoadedAt { get; set; }
    }

    /// <summary>
    /// 程序集信息
    /// </summary>
    private class AssemblyInfo
    {
        public Assembly Assembly { get; set; } = null!;
        public string? Path { get; set; }
    }
}
