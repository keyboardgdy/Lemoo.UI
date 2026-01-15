namespace Lemoo.UI.Models;

/// <summary>
/// 控件信息
/// </summary>
public class ControlInfo
{
    /// <summary>
    /// 控件名称（类名）
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 显示名称（中文）
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 分类
    /// </summary>
    public ControlCategory Category { get; set; }

    /// <summary>
    /// 类型（Styled/Custom）
    /// </summary>
    public ControlType Type { get; set; }

    /// <summary>
    /// 图标路径或几何数据
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// XAML命名空间前缀
    /// </summary>
    public string XamlNamespace { get; set; } = string.Empty;

    /// <summary>
    /// XAML命名空间URI
    /// </summary>
    public string XamlNamespaceUri { get; set; } = string.Empty;

    /// <summary>
    /// 示例代码
    /// </summary>
    public string SampleCode { get; set; } = string.Empty;

    /// <summary>
    /// 子样式（用于有多个变体的控件，如Button的Primary/Outline等）
    /// </summary>
    public List<ControlStyleVariant>? StyleVariants { get; set; }
}

/// <summary>
/// 控件样式变体
/// </summary>
public class ControlStyleVariant
{
    /// <summary>
    /// 样式名称
    /// </summary>
    public string StyleName { get; set; } = string.Empty;

    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 样式键
    /// </summary>
    public string StyleKey { get; set; } = string.Empty;
}
