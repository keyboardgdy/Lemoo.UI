namespace Lemoo.Core.Abstractions.Module;

/// <summary>
/// 模块加载结果
/// </summary>
public class ModuleLoadResult
{
    /// <summary>
    /// 是否加载成功
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// 加载的模块列表
    /// </summary>
    public IReadOnlyList<IModule> Modules { get; init; } = Array.Empty<IModule>();

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// 加载耗时
    /// </summary>
    public TimeSpan LoadDuration { get; init; }

    /// <summary>
    /// 创建成功结果
    /// </summary>
    public static ModuleLoadResult Successful(IReadOnlyList<IModule> modules, TimeSpan duration)
    {
        return new ModuleLoadResult
        {
            Success = true,
            Modules = modules,
            LoadDuration = duration
        };
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    public static ModuleLoadResult Failed(string errorMessage, TimeSpan duration)
    {
        return new ModuleLoadResult
        {
            Success = false,
            ErrorMessage = errorMessage,
            Modules = Array.Empty<IModule>(),
            LoadDuration = duration
        };
    }
}
