namespace Lemoo.Core.Abstractions.Module;

/// <summary>
/// 模块加载事件参数
/// </summary>
public class ModuleLoadingEventArgs : EventArgs
{
    public string ModuleName { get; }
    public string? ModulePath { get; }

    public ModuleLoadingEventArgs(string moduleName, string? modulePath = null)
    {
        ModuleName = moduleName;
        ModulePath = modulePath;
    }
}

/// <summary>
/// 模块已加载事件参数
/// </summary>
public class ModuleLoadedEventArgs : EventArgs
{
    public string ModuleName { get; }
    public IModule Module { get; }
    public TimeSpan LoadDuration { get; }

    public ModuleLoadedEventArgs(string moduleName, IModule module, TimeSpan loadDuration)
    {
        ModuleName = moduleName;
        Module = module;
        LoadDuration = loadDuration;
    }
}

/// <summary>
/// 模块卸载事件参数
/// </summary>
public class ModuleUnloadingEventArgs : EventArgs
{
    public string ModuleName { get; }

    public ModuleUnloadingEventArgs(string moduleName)
    {
        ModuleName = moduleName;
    }
}

/// <summary>
/// 模块加载器接口
/// </summary>
public interface IModuleLoader
{
    /// <summary>
    /// 模块加载前事件
    /// </summary>
    event EventHandler<ModuleLoadingEventArgs>? ModuleLoading;

    /// <summary>
    /// 模块加载后事件
    /// </summary>
    event EventHandler<ModuleLoadedEventArgs>? ModuleLoaded;

    /// <summary>
    /// 模块卸载前事件
    /// </summary>
    event EventHandler<ModuleUnloadingEventArgs>? ModuleUnloading;

    /// <summary>
    /// 加载所有模块
    /// </summary>
    Task<IReadOnlyList<IModule>> LoadModulesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 加载所有模块（带详细结果）
    /// </summary>
    Task<ModuleLoadResult> LoadModulesWithResultAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取已加载的模块
    /// </summary>
    IReadOnlyList<IModule> GetLoadedModules();

    /// <summary>
    /// 根据名称获取模块
    /// </summary>
    IModule? GetModule(string moduleName);

    /// <summary>
    /// 验证模块依赖关系
    /// </summary>
    void ValidateModuleDependencies(IReadOnlyList<IModule> modules);

    /// <summary>
    /// 卸载指定模块
    /// </summary>
    Task<bool> UnloadModuleAsync(string moduleName, TimeSpan timeout = default);

    /// <summary>
    /// 卸载所有模块
    /// </summary>
    Task UnloadAllModulesAsync(TimeSpan timeout = default);

    /// <summary>
    /// 重新加载指定模块
    /// </summary>
    Task<IModule?> ReloadModuleAsync(string moduleName, CancellationToken cancellationToken = default);
}

