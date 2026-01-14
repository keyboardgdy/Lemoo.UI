using System.Reflection;
using Lemoo.Core.Abstractions.Module;
using Lemoo.Core.Common.Module;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lemoo.Core.Infrastructure.ModuleLoader;

/// <summary>
/// 文件系统模块发现策略
/// </summary>
public class FileSystemModuleDiscoveryStrategy : IModuleDiscoveryStrategy
{
    private readonly string _modulePath;
    private readonly string _filePattern;
    private readonly ILogger<FileSystemModuleDiscoveryStrategy> _logger;

    public FileSystemModuleDiscoveryStrategy(
        string modulePath,
        ILogger<FileSystemModuleDiscoveryStrategy> logger)
    {
        _modulePath = modulePath;
        _filePattern = "*.Module.dll";
        _logger = logger;
    }

    public FileSystemModuleDiscoveryStrategy(
        string modulePath,
        string filePattern,
        ILogger<FileSystemModuleDiscoveryStrategy> logger)
    {
        _modulePath = modulePath;
        _filePattern = filePattern;
        _logger = logger;
    }

    public async Task<IEnumerable<Assembly>> DiscoverModulesAsync(CancellationToken cancellationToken = default)
    {
        var assemblies = new List<Assembly>();

        if (!Directory.Exists(_modulePath))
        {
            _logger.LogWarning("模块目录不存在: {ModulePath}", _modulePath);
            return await Task.FromResult(assemblies);
        }

        var dllFiles = Directory.GetFiles(_modulePath, _filePattern, SearchOption.TopDirectoryOnly);

        foreach (var dllFile in dllFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var assembly = Assembly.LoadFrom(dllFile);
                assemblies.Add(assembly);
                _logger.LogDebug("发现模块程序集: {AssemblyName}", assembly.GetName().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "加载模块程序集失败: {DllFile}", dllFile);
            }
        }

        _logger.LogInformation("从文件系统发现 {Count} 个模块程序集", assemblies.Count);
        return await Task.FromResult(assemblies);
    }
}
