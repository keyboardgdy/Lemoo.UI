using Microsoft.Extensions.DependencyInjection;

namespace Lemoo.Core.Abstractions.Module;

/// <summary>
/// 模块服务容器接口 - 用于实现模块级别的服务隔离
/// </summary>
public interface IModuleServiceContainer : IDisposable
{
    /// <summary>
    /// 模块名称
    /// </summary>
    string ModuleName { get; }

    /// <summary>
    /// 模块的服务集合
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// 构建模块的服务提供者
    /// </summary>
    IServiceProvider BuildServiceProvider();

    /// <summary>
    /// 获取服务提供者（如果已构建）
    /// </summary>
    IServiceProvider? ServiceProvider { get; }

    /// <summary>
    /// 模块状态
    /// </summary>
    ModuleState State { get; set; }

    /// <summary>
    /// 导出的服务（供其他模块使用）
    /// </summary>
    IReadOnlyDictionary<string, object> ExportedServices { get; }

    /// <summary>
    /// 导入的服务（从其他模块导入）
    /// </summary>
    IReadOnlyDictionary<string, object> ImportedServices { get; }

    /// <summary>
    /// 导出服务
    /// </summary>
    void ExportService(string key, object service);

    /// <summary>
    /// 导入服务
    /// </summary>
    void ImportService(string key, object service);

    /// <summary>
    /// 获取导出的服务
    /// </summary>
    T? GetExportedService<T>(string key) where T : class;

    /// <summary>
    /// 获取导入的服务
    /// </summary>
    T? GetImportedService<T>(string key) where T : class;
}

/// <summary>
/// 模块服务容器实现
/// </summary>
public class ModuleServiceContainer : IModuleServiceContainer
{
    private readonly Dictionary<string, object> _exportedServices = new();
    private readonly Dictionary<string, object> _importedServices = new();
    private IServiceProvider? _serviceProvider;
    private bool _disposed;

    public string ModuleName { get; }
    public IServiceCollection Services { get; }
    public IServiceProvider? ServiceProvider => _serviceProvider;
    public ModuleState State { get; set; } = ModuleState.Loaded;
    public IReadOnlyDictionary<string, object> ExportedServices => _exportedServices;
    public IReadOnlyDictionary<string, object> ImportedServices => _importedServices;

    public ModuleServiceContainer(string moduleName)
    {
        ModuleName = moduleName;
        Services = new ServiceCollection();
    }

    public IServiceProvider BuildServiceProvider()
    {
        if (_serviceProvider != null)
        {
            return _serviceProvider;
        }

        _serviceProvider = Services.BuildServiceProvider();
        return _serviceProvider;
    }

    public void ExportService(string key, object service)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Service key cannot be empty", nameof(key));

        if (service == null)
            throw new ArgumentNullException(nameof(service));

        _exportedServices[key] = service;
    }

    public void ImportService(string key, object service)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Service key cannot be empty", nameof(key));

        if (service == null)
            throw new ArgumentNullException(nameof(service));

        _importedServices[key] = service;
    }

    public T? GetExportedService<T>(string key) where T : class
    {
        if (_exportedServices.TryGetValue(key, out var service))
        {
            return service as T;
        }
        return null;
    }

    public T? GetImportedService<T>(string key) where T : class
    {
        if (_importedServices.TryGetValue(key, out var service))
        {
            return service as T;
        }
        return null;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        // 清理导出的服务
        _exportedServices.Clear();

        // 清理导入的服务
        _importedServices.Clear();

        // 释放服务提供者
        if (_serviceProvider is IDisposable disposableServiceProvider)
        {
            disposableServiceProvider.Dispose();
        }

        _disposed = true;
    }
}
