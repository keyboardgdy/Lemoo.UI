using System.Reflection;
using Lemoo.Core.Abstractions.Module;
using Lemoo.Core.Common.Module;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lemoo.Core.Infrastructure.ModuleLoader;

/// <summary>
/// 可配置的模块发现策略
/// </summary>
public class ConfigurableModuleDiscoveryStrategy : IModuleDiscoveryStrategy
{
    private readonly ModuleDiscoveryOptions _options;
    private readonly ILogger<ConfigurableModuleDiscoveryStrategy> _logger;

    public ConfigurableModuleDiscoveryStrategy(
        IOptions<ModuleDiscoveryOptions> options,
        ILogger<ConfigurableModuleDiscoveryStrategy> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IEnumerable<Assembly>> DiscoverModulesAsync(CancellationToken cancellationToken = default)
    {
        var assemblies = new List<Assembly>();
        var processedAssemblies = new HashSet<string>();

        // 从配置的路径搜索
        foreach (var path in _options.Paths)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!Directory.Exists(path))
            {
                _logger.LogWarning("模块目录不存在: {ModulePath}", path);
                continue;
            }

            var searchOption = _options.RecursiveSearch
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly;

            var dllFiles = Directory.GetFiles(path, _options.FilePattern, searchOption);

            foreach (var dllFile in dllFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // 应用自定义过滤器
                if (_options.Filter != null && !_options.Filter(dllFile))
                {
                    continue;
                }

                // 检查是否已处理
                if (processedAssemblies.Contains(dllFile))
                {
                    continue;
                }

                try
                {
                    var assembly = Assembly.LoadFrom(dllFile);
                    var assemblyName = assembly.GetName().Name;

                    // 检查排除列表
                    if (_options.ExcludedAssemblies.Contains(assemblyName ?? string.Empty))
                    {
                        _logger.LogDebug("跳过排除的程序集: {AssemblyName}", assemblyName);
                        continue;
                    }

                    assemblies.Add(assembly);
                    processedAssemblies.Add(dllFile);
                    _logger.LogDebug("发现模块程序集: {AssemblyName}", assemblyName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "加载模块程序集失败: {DllFile}", dllFile);
                }
            }
        }

        // 从已加载程序集发现
        if (_options.IncludeLoadedAssemblies)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in loadedAssemblies)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    // 跳过动态程序集和系统程序集
                    if (assembly.IsDynamic)
                    {
                        continue;
                    }

                    var assemblyName = assembly.GetName().Name;

                    // 检查排除列表
                    if (string.IsNullOrEmpty(assemblyName) ||
                        _options.ExcludedAssemblies.Contains(assemblyName))
                    {
                        continue;
                    }

                    // 跳过系统程序集
                    if (assemblyName.StartsWith("System.") ||
                        assemblyName.StartsWith("Microsoft.") ||
                        assemblyName.StartsWith("netstandard"))
                    {
                        continue;
                    }

                    // 检查是否包含模块类型
                    if (assembly.GetTypes().Any(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract))
                    {
                        if (!processedAssemblies.Contains(assembly.Location!))
                        {
                            assemblies.Add(assembly);
                            processedAssemblies.Add(assembly.Location!);
                            _logger.LogDebug("从已加载程序集发现模块: {AssemblyName}", assemblyName);
                        }
                    }
                }
                catch
                {
                    // 忽略无法检查的程序集
                }
            }
        }

        _logger.LogInformation("发现 {Count} 个模块程序集", assemblies.Count);
        return await Task.FromResult(assemblies);
    }
}
