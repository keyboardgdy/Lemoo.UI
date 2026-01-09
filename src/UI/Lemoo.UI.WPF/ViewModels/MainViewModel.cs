using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Lemoo.UI.WPF.ViewModels;

/// <summary>
/// 主视图模型
/// </summary>
public partial class MainViewModel : ObservableObject
{
    /// <summary>
    /// 窗口标题（供 MainWindow 绑定使用）
    /// </summary>
    [ObservableProperty]
    private string title = "Lemoo - UI 示例";
    
    [ObservableProperty]
    private string statusMessage = string.Empty;
    
    [ObservableProperty]
    private bool isLoading;

    /// <summary>
    /// 顶部标题栏菜单数据（供 MainTitleBar 绑定）。
    /// </summary>
    public ObservableCollection<MenuItemModel> MenuItems { get; } = new();

    /// <summary>
    /// 左侧导航树数据（供 Sidebar 绑定）。
    /// </summary>
    public ObservableCollection<NavigationItem> NavigationItems { get; } = new();

    /// <summary>
    /// 左侧底部导航项（供 Sidebar 绑定）。
    /// </summary>
    public ObservableCollection<NavigationItem> BottomNavigationItems { get; } = new();

    public MainViewModel()
    {
        // 默认初始化示例导航（用于UI.WPF独立运行）
        InitializeNavigation();
    }
    
    /// <summary>
    /// 构造函数重载，允许跳过默认导航初始化（用于Host）
    /// </summary>
    public MainViewModel(bool skipDefaultNavigation)
    {
        if (!skipDefaultNavigation)
        {
            InitializeNavigation();
        }
    }
    
    /// <summary>
    /// 清除所有导航项（用于Host重新初始化）
    /// </summary>
    public void ClearNavigation()
    {
        MenuItems.Clear();
        NavigationItems.Clear();
        BottomNavigationItems.Clear();
    }

    /// <summary>
    /// 初始化示例菜单和导航数据，方便在主界面测试导航与标签页功能。
    /// </summary>
    private void InitializeNavigation()
    {
        // 顶部菜单：示例 -> 仪表盘 / 示例列表 / 设置示例
        var samplesMenu = new MenuItemModel
        {
            Header = "示例",
            Icon = "\uE8A5" // browse
        };
        samplesMenu.Children.Add(new MenuItemModel
        {
            Header = "仪表盘",
            PageKey = "dashboard",
            Icon = "\uE80F" // home
        });
        samplesMenu.Children.Add(new MenuItemModel
        {
            Header = "设置示例",
            PageKey = "settings",
            Icon = "\uE713" // settings
        });
        // Win11 下拉框示例（顶部菜单）
        samplesMenu.Children.Add(new MenuItemModel
        {
            Header = "Win11 下拉示例",
            PageKey = "win11-combobox",
            Icon = "\uE74E" // Combobox-like icon
        });

        MenuItems.Add(samplesMenu);

        // 侧边栏导航：总览、示例分组等
        NavigationItems.Add(new NavigationItem
        {
            Title = "总览",
            Icon = "\uE80F", // Home
            PageKey = "dashboard"
        });

        var examplesGroup = new NavigationItem
        {
            Title = "示例功能",
            Icon = "\uE8A5", // List / browse
            IsExpanded = true
        };
        examplesGroup.Children.Add(new NavigationItem
        {
            Title = "设置示例",
            Icon = "\uE713", // Settings
            PageKey = "settings"
        });
        // Win11 下拉框示例（侧边导航）
        examplesGroup.Children.Add(new NavigationItem
        {
            Title = "Win11 下拉示例",
            Icon = "\uE74E",
            PageKey = "win11-combobox"
        });
        NavigationItems.Add(examplesGroup);

        // 底部导航：设置
        BottomNavigationItems.Add(new NavigationItem
        {
            Title = "设置",
            Icon = "\uE713",
            PageKey = "settings"
        });
    }
    
}

