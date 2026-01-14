using System.Windows.Media;

namespace Lemoo.UI.Helpers;

/// <summary>
/// 主题信息模型
/// 包含主题的显示名称、描述、预览颜色等元数据
/// </summary>
public class ThemeInfo
{
    /// <summary>
    /// 主题类型
    /// </summary>
    public ThemeManager.Theme ThemeType { get; set; }

    /// <summary>
    /// 主题显示名称
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 主题描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 主题详细说明
    /// </summary>
    public string DetailDescription { get; set; } = string.Empty;

    /// <summary>
    /// 预览颜色 - 标题栏/侧边栏背景色
    /// </summary>
    public Color PreviewPrimaryColor { get; set; }

    /// <summary>
    /// 预览颜色 - 主区域背景色
    /// </summary>
    public Color PreviewSecondaryColor { get; set; }

    /// <summary>
    /// 预览颜色 - 强调色
    /// </summary>
    public Color PreviewAccentColor { get; set; }

    /// <summary>
    /// 预览颜色 - 文本色
    /// </summary>
    public Color PreviewTextColor { get; set; }

    /// <summary>
    /// 主题图标（可选，用于未来扩展）
    /// </summary>
    public string? IconKey { get; set; }

    /// <summary>
    /// 是否为默认主题
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 主题标签（如 "推荐"、"新" 等）
    /// </summary>
    public string? Tag { get; set; }

    /// <summary>
    /// 主题类别（用于分组）
    /// </summary>
    public string Category { get; set; } = "默认";

    /// <summary>
    /// 是否被选中（由 ViewModel 管理）
    /// </summary>
    public bool IsSelected { get; set; }
}
