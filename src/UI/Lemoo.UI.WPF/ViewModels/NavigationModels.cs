using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Lemoo.UI.WPF.ViewModels;

/// <summary>
/// 顶部菜单的数据模型（供 MainTitleBar 使用）。
/// 通过反射读取 Header / Icon / PageKey / Children 属性。
/// </summary>
public class MenuItemModel
{
    public string Header { get; set; } = string.Empty;

    /// <summary>
    /// 字体图标（Segoe MDL2 Assets 字符），用于上下文菜单和子菜单。
    /// 可为空。
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 对应的页面键（可为空，为空则仅作为菜单分组/父级）。
    /// </summary>
    public string? PageKey { get; set; }

    public ObservableCollection<MenuItemModel> Children { get; set; } = new();
}

/// <summary>
/// 侧边栏导航项数据模型（供 Sidebar 使用）。
/// Sidebar 通过反射访问 Title / Icon / PageKey / HasChildren / IsExpanded / Children / IsEnabled 等属性。
/// </summary>
public partial class NavigationItem : ObservableObject
{
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 字体图标（Segoe MDL2 Assets 字符）。
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// 对应页面键（可为空，父级分组节点可不设置）。
    /// </summary>
    public string? PageKey { get; set; }

    /// <summary>
    /// 是否可点击/可见。
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 是否有子节点（用于显示展开图标）。
    /// </summary>
    public bool HasChildren => Children.Count > 0;

    /// <summary>
    /// 展开状态，由侧边栏通过反射修改。
    /// </summary>
    [ObservableProperty]
    private bool isExpanded;

    public ObservableCollection<NavigationItem> Children { get; set; } = new();
}


