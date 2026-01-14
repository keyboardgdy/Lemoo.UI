using System.Diagnostics;
using System.Reflection;
using Lemoo.Core.Abstractions.Module;
using Lemoo.Core.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lemoo.Core.Infrastructure.ModuleLoader;

/// <summary>
/// 模块加载器实现
/// </summary>
public class ModuleLoader : IModuleLoader
{
    private readonly ILogger<ModuleLoader> _logger;
    private readonly IConfiguration _configuration;
    private readonly List<IModule> _loadedModules = new();
    private readonly Dictionary<string, ModuleLoadContext> _moduleLoadContexts = new();
    private readonly ModuleVersionCompatibilityChecker _versionChecker;
    private bool _disposed;

    public ModuleLoader(ILogger<ModuleLoader> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _versionChecker = new ModuleVersionCompatibilityChecker(logger);
    }
    
    public Task<IReadOnlyList<IModule>> LoadModulesAsync(CancellationToken cancellationToken = default)
    {
        var enabledModulesSection = _configuration.GetSection("Lemoo:Modules:Enabled");
        var enabledModules = enabledModulesSection.Get<string[]>() ?? Array.Empty<string>();
        var modulePath = _configuration["Lemoo:Modules:Path"] ?? "./Modules";

        _logger.LogInformation("开始加载模块，路径: {ModulePath}", modulePath);

        var moduleAssemblies = DiscoverModuleAssemblies(modulePath);
        var modules = InstantiateModules(moduleAssemblies, enabledModules);

        // 版本兼容性检查
        var compatibilityResult = _versionChecker.ValidateCompatibility(modules);
        if (!compatibilityResult.IsCompatible)
        {
            var errorMessages = string.Join(Environment.NewLine,
                compatibilityResult.Errors.Select(e => $"  - {e.Message}"));
            throw new ModuleDependencyException("ModuleCompatibility",
                $"模块版本兼容性检查失败:{Environment.NewLine}{errorMessages}");
        }

        // 记录兼容性警告
        foreach (var warning in compatibilityResult.Warnings)
        {
            _logger.LogWarning("模块兼容性警告: {Message}", warning.Message);
        }

        ValidateModuleDependencies(modules);
        var sortedModules = SortModulesByDependencies(modules);

        _loadedModules.Clear();
        _loadedModules.AddRange(sortedModules);

        _logger.LogInformation("成功加载 {Count} 个模块", _loadedModules.Count);

        return Task.FromResult<IReadOnlyList<IModule>>(_loadedModules.AsReadOnly());
    }

    public Task<ModuleLoadResult> LoadModulesWithResultAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var modules = LoadModulesAsync(cancellationToken).GetAwaiter().GetResult();
            stopwatch.Stop();
            return Task.FromResult(ModuleLoadResult.Successful(modules, stopwatch.Elapsed));
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return Task.FromResult(ModuleLoadResult.Failed(ex.Message, stopwatch.Elapsed));
        }
    }
    
    private IReadOnlyList<Assembly> DiscoverModuleAssemblies(string modulePath)
    {
        var assemblies = new HashSet<Assembly>();
        var loadedAssemblyNames = new HashSet<string>();
        
        // 1. 首先从文件系统加载模块（如果路径存在）
        if (Directory.Exists(modulePath))
        {
            var dllFiles = Directory.GetFiles(modulePath, "*.dll", SearchOption.AllDirectories);
            
            foreach (var dllFile in dllFiles)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dllFile);
                    var assemblyName = assembly.GetName().Name;
                    if (assemblyName != null && IsModuleAssembly(assemblyName))
                    {
                        assemblies.Add(assembly);
                        loadedAssemblyNames.Add(assemblyName);
                        _logger.LogDebug("从文件系统加载程序集: {AssemblyName}", assemblyName);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "无法加载程序集: {DllFile}", dllFile);
                }
            }
        }
        else
        {
            _logger.LogInformation("模块路径不存在: {ModulePath}，尝试从已加载的程序集中发现模块", modulePath);
        }
        
        // 2. 从已加载的程序集中发现模块（支持项目引用方式）
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var assemblyName = assembly.GetName().Name;
            if (assemblyName != null 
                && IsModuleAssembly(assemblyName) 
                && !loadedAssemblyNames.Contains(assemblyName))
            {
                assemblies.Add(assembly);
                _logger.LogDebug("从已加载程序集中发现: {AssemblyName}", assemblyName);
            }
        }
        
        return assemblies.ToList();
    }
    
    private bool IsModuleAssembly(string assemblyName)
    {
        return assemblyName.StartsWith("Lemoo.Modules.", StringComparison.OrdinalIgnoreCase)
            && !assemblyName.EndsWith(".Abstractions", StringComparison.OrdinalIgnoreCase)
            && !assemblyName.EndsWith(".UI", StringComparison.OrdinalIgnoreCase);
    }
    
    private IReadOnlyList<IModule> InstantiateModules(
        IReadOnlyList<Assembly> assemblies,
        string[] enabledModules)
    {
        var modules = new List<IModule>();
        
        foreach (var assembly in assemblies)
        {
            var moduleTypes = assembly.GetTypes()
                .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);
                
            foreach (var moduleType in moduleTypes)
            {
                var moduleName = moduleType.Name.Replace("Module", "");
                
                if (enabledModules.Length > 0 && !enabledModules.Contains(moduleName) && !enabledModules.Contains("*"))
                    continue;
                    
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

    /// <summary>
    /// 卸载模块
    /// </summary>
    public async Task<bool> UnloadModuleAsync(string moduleName, TimeSpan timeout = default)
    {
        if (_disposed)
            return false;

        _logger.LogInformation("开始卸载模块: {ModuleName}", moduleName);

        try
        {
            // 1. 从已加载列表中移除
            var module = _loadedModules.FirstOrDefault(m => m.Name == moduleName);
            if (module != null)
            {
                _loadedModules.Remove(module);
            }

            // 2. 卸载模块加载上下文
            if (_moduleLoadContexts.TryGetValue(moduleName, out var loadContext))
            {
                var unloaded = await loadContext.UnloadAsync(timeout);
                if (unloaded)
                {
                    _moduleLoadContexts.Remove(moduleName);
                    _logger.LogInformation("模块卸载成功: {ModuleName}", moduleName);
                    return true;
                }
                else
                {
                    _logger.LogWarning("模块卸载超时: {ModuleName}", moduleName);
                    return false;
                }
            }

            _logger.LogDebug("模块没有加载上下文，跳过卸载: {ModuleName}", moduleName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "模块卸载失败: {ModuleName}", moduleName);
            return false;
        }
    }

    /// <summary>
    /// 卸载所有模块
    /// </summary>
    public async Task UnloadAllModulesAsync(TimeSpan timeout = default)
    {
        _logger.LogInformation("开始卸载所有模块...");

        var moduleNames = _moduleLoadContexts.Keys.ToList();
        foreach (var moduleName in moduleNames)
        {
            await UnloadModuleAsync(moduleName, timeout);
        }

        _loadedModules.Clear();
        _logger.LogInformation("所有模块已卸载");
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        // 同步卸载所有模块（使用较短的超时）
        UnloadAllModulesAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();

        _disposed = true;
    }
}

