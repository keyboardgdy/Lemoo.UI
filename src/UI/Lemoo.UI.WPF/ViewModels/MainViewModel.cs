using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lemoo.UI.WPF.Abstractions;
using Lemoo.UI.WPF.Constants;
using Lemoo.UI.WPF.Models;
using System.Collections.ObjectModel;

namespace Lemoo.UI.WPF.ViewModels;

/// <summary>
/// 主视图模型
/// </summary>
public partial class MainViewModel : ObservableObject, IMainViewModel
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

    /// <summary>
    /// 默认构造函数，初始化示例导航
    /// </summary>
    public MainViewModel()
    {
        InitializeDefaultNavigation();
    }

    /// <summary>
    /// 初始化默认示例导航（用于独立运行）
    /// </summary>
    public void InitializeDefaultNavigation()
    {
        ClearNavigation();
        InitializeNavigation();
    }

    /// <summary>
    /// 从元数据初始化导航（用于模块化场景）
    /// </summary>
    /// <param name="menuItems">菜单项集合</param>
    /// <param name="navItems">导航项集合</param>
    /// <param name="bottomNavItems">底部导航项集合</param>
    public void InitializeNavigationFromMetadata(
        IEnumerable<MenuItemModel> menuItems,
        IEnumerable<NavigationItem> navItems,
        IEnumerable<NavigationItem>? bottomNavItems = null)
    {
        ClearNavigation();

        foreach (var item in menuItems)
        {
            MenuItems.Add(item);
        }

        foreach (var item in navItems)
        {
            NavigationItems.Add(item);
        }

        if (bottomNavItems != null)
        {
            foreach (var item in bottomNavItems)
            {
                BottomNavigationItems.Add(item);
            }
        }
    }

    /// <summary>
    /// 清除所有导航项（用于重新初始化）
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
        var samplesMenu = new MenuItemModel
        {
            Header = NavigationConstants.MenuText.Samples,
            Icon = NavigationConstants.Icons.Browse
        };

        samplesMenu.Children.Add(new MenuItemModel
        {
            Header = NavigationConstants.MenuText.Dashboard,
            PageKey = PageKeys.Dashboard,
            Icon = NavigationConstants.Icons.Home
        });

        samplesMenu.Children.Add(new MenuItemModel
        {
            Header = NavigationConstants.MenuText.SettingsSample,
            PageKey = PageKeys.Settings,
            Icon = NavigationConstants.Icons.Settings
        });

        MenuItems.Add(samplesMenu);

        NavigationItems.Add(new NavigationItem
        {
            Title = NavigationConstants.MenuText.Overview,
            Icon = NavigationConstants.Icons.Home,
            PageKey = PageKeys.Dashboard
        });

        var examplesGroup = new NavigationItem
        {
            Title = NavigationConstants.MenuText.SampleFeatures,
            Icon = NavigationConstants.Icons.Browse,
            IsExpanded = true
        };

        examplesGroup.Children.Add(new NavigationItem
        {
            Title = NavigationConstants.MenuText.SettingsSample,
            Icon = NavigationConstants.Icons.Settings,
            PageKey = PageKeys.Settings
        });

        NavigationItems.Add(examplesGroup);

        BottomNavigationItems.Add(new NavigationItem
        {
            Title = NavigationConstants.MenuText.Settings,
            Icon = NavigationConstants.Icons.Settings,
            PageKey = PageKeys.Settings
        });
    }
    
}

