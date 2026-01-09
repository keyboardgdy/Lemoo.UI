namespace Lemoo.Core.Abstractions.Module;

/// <summary>
/// 模块加载器接口
/// </summary>
public interface IModuleLoader
{
    /// <summary>
    /// 加载所有模块
    /// </summary>
    Task<IReadOnlyList<IModule>> LoadModulesAsync(CancellationToken cancellationToken = default);
    
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
}

