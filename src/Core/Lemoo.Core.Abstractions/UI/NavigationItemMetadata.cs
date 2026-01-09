namespace Lemoo.Core.Abstractions.UI;

/// <summary>
/// 导航项元数据
/// </summary>
public class NavigationItemMetadata
{
    /// <summary>
    /// 页面键（唯一标识）
    /// </summary>
    public string PageKey { get; set; } = string.Empty;
    
    /// <summary>
    /// 显示标题
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// 图标（Segoe MDL2 Assets字符）
    /// </summary>
    public string Icon { get; set; } = string.Empty;
    
    /// <summary>
    /// 所属模块名称
    /// </summary>
    public string Module { get; set; } = string.Empty;
    
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 父导航项键（用于构建层级结构）
    /// </summary>
    public string? ParentPageKey { get; set; }
    
    /// <summary>
    /// 排序顺序
    /// </summary>
    public int Order { get; set; } = 0;
}

