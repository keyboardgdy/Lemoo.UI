namespace Lemoo.Core.Abstractions.Module;

/// <summary>
/// 模块热重载事件参数
/// </summary>
public class ModuleReloadedEventArgs : EventArgs
{
    public string ModuleName { get; }
    public IModule? Module { get; }
    public DateTime ReloadedAt { get; }

    public ModuleReloadedEventArgs(string moduleName, IModule? module)
    {
        ModuleName = moduleName;
        Module = module;
        ReloadedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// 模块热重载服务接口
/// </summary>
public interface IModuleHotReloadService
{
    /// <summary>
    /// 模块重载完成事件
    /// </summary>
    event EventHandler<ModuleReloadedEventArgs>? ModuleReloaded;

    /// <summary>
    /// 模块重载失败事件
    /// </summary>
    event EventHandler<ModuleReloadFailedEventArgs>? ModuleReloadFailed;

    /// <summary>
    /// 重新加载指定模块
    /// </summary>
    Task<bool> ReloadModuleAsync(string moduleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 监视模块文件变化
    /// </summary>
    Task WatchModulesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止监视
    /// </summary>
    Task StopWatchingAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// 模块重载失败事件参数
/// </summary>
public class ModuleReloadFailedEventArgs : EventArgs
{
    public string ModuleName { get; }
    public Exception Exception { get; }
    public DateTime FailedAt { get; }

    public ModuleReloadFailedEventArgs(string moduleName, Exception exception)
    {
        ModuleName = moduleName;
        Exception = exception;
        FailedAt = DateTime.UtcNow;
    }
}
