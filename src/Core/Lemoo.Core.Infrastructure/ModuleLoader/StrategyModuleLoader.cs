using System.Diagnostics;
using System.Reflection;
using Lemoo.Core.Abstractions.Module;
using Lemoo.Core.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lemoo.Core.Infrastructure.ModuleLoader;

/// <summary>
/// 策略式模块加载器 - 支持多种模块发现策略
/// </summary>
public class StrategyModuleLoader : IModuleLoader
{
    private readonly ILogger<StrategyModuleLoader> _logger;
    private readonly IConfiguration _configuration;
    private readonly IEnumerable<IModuleDiscoveryStrategy> _discoveryStrategies;
    private readonly List<IModule> _loadedModules = new();

    public StrategyModuleLoader(
        ILogger<StrategyModuleLoader> logger,
        IConfiguration configuration,
        IEnumerable<IModuleDiscoveryStrategy> discoveryStrategies)
    {
        _logger = logger;
        _configuration = configuration;
        _discoveryStrategies = discoveryStrategies;
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

            _logger.LogInformation("开始加载模块...");

            // 1. 使用策略发现模块
            var moduleAssemblies = await DiscoverModuleAssembliesAsync(cancellationToken);

            // 2. 实例化模块
            var modules = InstantiateModules(moduleAssemblies, enabledModules);

            // 3. 验证依赖关系
            ValidateModuleDependencies(modules);

            // 4. 按依赖关系排序
            var sortedModules = SortModulesByDependencies(modules);

            _loadedModules.Clear();
            _loadedModules.AddRange(sortedModules);

            stopwatch.Stop();
            _logger.LogInformation("成功加载 {Count} 个模块，耗时: {ElapsedMilliseconds}ms",
                _loadedModules.Count, stopwatch.ElapsedMilliseconds);

            return ModuleLoadResult.Successful(_loadedModules.AsReadOnly(), stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "模块加载失败，耗时: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
            return ModuleLoadResult.Failed(ex.Message, stopwatch.Elapsed);
        }
    }

    private async Task<List<Assembly>> DiscoverModuleAssembliesAsync(CancellationToken cancellationToken)
    {
        var assemblies = new HashSet<Assembly>();
        var processedAssemblyPaths = new HashSet<string>();

        // 使用所有配置的发现策略
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
                        assemblies.Add(assembly);
                        processedAssemblyPaths.Add(location);
                    }
                    else if (string.IsNullOrEmpty(location))
                    {
                        // 动态程序集没有位置，直接添加
                        assemblies.Add(assembly);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "模块发现策略执行失败: {StrategyType}", strategy.GetType().Name);
            }
        }

        _logger.LogDebug("发现 {Count} 个唯一的模块程序集", assemblies.Count);
        return assemblies.ToList();
    }

    private IReadOnlyList<IModule> InstantiateModules(
        IReadOnlyList<Assembly> assemblies,
        string[] enabledModules)
    {
        var modules = new List<IModule>();

        foreach (var assembly in assemblies)
        {
            try
            {
                var moduleTypes = assembly.GetTypes()
                    .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

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
                        var module = (IModule)Activator.CreateInstance(moduleType)!;
                        modules.Add(module);
                        _logger.LogInformation("发现模块: {ModuleName} v{Version}", module.Name, module.Version);
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

        return modules;
    }

    public void ValidateModuleDependencies(IReadOnlyList<IModule> modules)
    {
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

            foreach (var depName in module.Dependencies)
            {
                var dep = modules.FirstOrDefault(m => m.Name == depName);
                if (dep != null)
                    Visit(dep);
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

    public IReadOnlyList<IModule> GetLoadedModules() => _loadedModules.AsReadOnly();

    public IModule? GetModule(string moduleName) =>
        _loadedModules.FirstOrDefault(m => m.Name == moduleName);
}
