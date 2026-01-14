using Lemoo.UI.WPF.Models;
using System.Collections.ObjectModel;

namespace Lemoo.UI.WPF.Abstractions;

/// <summary>
/// 主视图模型接口，便于测试和松耦合
/// </summary>
public interface IMainViewModel
{
    /// <summary>
    /// 窗口标题
    /// </summary>
    string Title { get; }

    /// <summary>
    /// 状态消息
    /// </summary>
    string StatusMessage { get; set; }

    /// <summary>
    /// 是否正在加载
    /// </summary>
    bool IsLoading { get; set; }

    /// <summary>
    /// 顶部菜单项集合
    /// </summary>
    ObservableCollection<MenuItemModel> MenuItems { get; }

    /// <summary>
    /// 导航项集合
    /// </summary>
    ObservableCollection<NavigationItem> NavigationItems { get; }

    /// <summary>
    /// 底部导航项集合
    /// </summary>
    ObservableCollection<NavigationItem> BottomNavigationItems { get; }

    /// <summary>
    /// 初始化默认导航
    /// </summary>
    void InitializeDefaultNavigation();

    /// <summary>
    /// 从元数据初始化导航
    /// </summary>
    void InitializeNavigationFromMetadata(
        IEnumerable<MenuItemModel> menuItems,
        IEnumerable<NavigationItem> navItems,
        IEnumerable<NavigationItem>? bottomNavItems = null);

    /// <summary>
    /// 清除所有导航项
    /// </summary>
    void ClearNavigation();
}
