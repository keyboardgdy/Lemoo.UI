namespace Lemoo.Core.Common.Module;

/// <summary>
/// 模块发现配置选项
/// </summary>
public class ModuleDiscoveryOptions
{
    /// <summary>
    /// 模块搜索路径列表
    /// </summary>
    public List<string> Paths { get; set; } = new();

    /// <summary>
    /// 文件模式（用于筛选 DLL 文件）
    /// </summary>
    public string FilePattern { get; set; } = "*.Module.dll";

    /// <summary>
    /// 是否包含已加载的程序集
    /// </summary>
    public bool IncludeLoadedAssemblies { get; set; } = true;

    /// <summary>
    /// 自定义过滤器函数
    /// </summary>
    public Func<string, bool>? Filter { get; set; }

    /// <summary>
    /// 是否递归搜索子目录
    /// </summary>
    public bool RecursiveSearch { get; set; } = false;

    /// <summary>
    /// 排除的程序集名称列表
    /// </summary>
    public List<string> ExcludedAssemblies { get; set; } = new();
}
