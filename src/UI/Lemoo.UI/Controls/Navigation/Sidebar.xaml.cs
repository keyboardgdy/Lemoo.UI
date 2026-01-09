using System;
using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Lemoo.UI.Controls.Chrome;

namespace Lemoo.UI.Controls.Navigation;

/// <summary>
/// 侧边栏导航控件：实现收缩动画、搜索框与导航树 UI。
/// 仅依赖 DataContext 提供 NavigationItems / BottomNavigationItems 等属性。
/// </summary>
public partial class Sidebar : UserControl
{
    private bool _isCollapsed = false;
    private const double ExpandedWidth = 240;
    private const double CollapsedWidth = 56;

    public Sidebar()
    {
        InitializeComponent();
    }

    private void ToggleButton_Click(object sender, RoutedEventArgs e)
    {
        _isCollapsed = !_isCollapsed;
        AnimateWidth(_isCollapsed);
        UpdateCollapsedState(_isCollapsed);
    }

    private void AnimateWidth(bool collapse)
    {
        var targetWidth = collapse ? CollapsedWidth : ExpandedWidth;
        var animation = new System.Windows.Media.Animation.DoubleAnimation
        {
            To = targetWidth,
            Duration = new Duration(TimeSpan.FromMilliseconds(200)),
            EasingFunction = new System.Windows.Media.Animation.CubicEase { EasingMode = System.Windows.Media.Animation.EasingMode.EaseInOut }
        };

        BeginAnimation(WidthProperty, animation);
    }

    private void UpdateCollapsedState(bool collapsed)
    {
        // 切换搜索框和搜索按钮的显示
        if (collapsed)
        {
            SearchContainer.Visibility = Visibility.Collapsed;
            SearchButton.Visibility = Visibility.Visible;
        }
        else
        {
            SearchContainer.Visibility = Visibility.Visible;
            SearchButton.Visibility = Visibility.Collapsed;
        }

        // 更新导航项样式
        UpdateNavItemStyles(NavTree, collapsed);
        UpdateNavItemStyles(BottomNavItems, collapsed);

        // 收缩时折叠导航树（将所有具有 IsExpanded 属性的节点设置为 false）
        if (collapsed)
        {
            CollapseAllNavigationItems();
        }
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isCollapsed)
        {
            _isCollapsed = false;
            AnimateWidth(_isCollapsed);
            UpdateCollapsedState(_isCollapsed);
        }

        Dispatcher.BeginInvoke(new Action(() =>
        {
            SearchBox.Focus();
            SearchBox.SelectAll();
        }), System.Windows.Threading.DispatcherPriority.Loaded);
    }

    private void SearchContainer_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (_isCollapsed)
        {
            _isCollapsed = false;
            AnimateWidth(_isCollapsed);
            UpdateCollapsedState(_isCollapsed);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                SearchBox.Focus();
                SearchBox.SelectAll();
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }
        else
        {
            SearchBox.Focus();
            SearchBox.SelectAll();
        }
    }

    private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (_isCollapsed)
        {
            _isCollapsed = false;
            AnimateWidth(_isCollapsed);
            UpdateCollapsedState(_isCollapsed);
        }
    }

    private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
    {
        // 可根据需要添加逻辑
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // 此处预留搜索过滤逻辑（根据需要实现）
    }

    #region 导航点击 & 事件

    public static readonly RoutedEvent NavigateToPageEvent = EventManager.RegisterRoutedEvent(
        nameof(NavigateToPage), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Sidebar));

    public event RoutedEventHandler NavigateToPage
    {
        add => AddHandler(NavigateToPageEvent, value);
        remove => RemoveHandler(NavigateToPageEvent, value);
    }

    private void NavItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.DataContext is null) return;
        var item = button.DataContext;
        var type = item.GetType();

        // 处理父级节点展开/折叠（HasChildren / IsExpanded 属性）
        var hasChildrenProp = type.GetProperty("HasChildren");
        var isExpandedProp = type.GetProperty("IsExpanded");
        if (hasChildrenProp?.GetValue(item) is bool hasChildren && hasChildren)
        {
            // 如果当前处于收缩状态：先展开侧边栏，再展开当前父级节点
            if (_isCollapsed)
            {
                _isCollapsed = false;
                AnimateWidth(_isCollapsed);
                UpdateCollapsedState(_isCollapsed);

                if (isExpandedProp != null)
                {
                    // 直接展开该父级节点
                    isExpandedProp.SetValue(item, true);
                }
            }
            else if (isExpandedProp != null)
            {
                var current = (bool)(isExpandedProp.GetValue(item) ?? false);
                isExpandedProp.SetValue(item, !current);
            }
            e.Handled = true;
            return;
        }

        // 触发导航事件（依赖 PageKey / Title 属性）
        var pageKeyProp = type.GetProperty("PageKey");
        var titleProp = type.GetProperty("Title");
        var pageKey = pageKeyProp?.GetValue(item)?.ToString();
        var title = titleProp?.GetValue(item)?.ToString() ?? pageKey ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(pageKey))
        {
            RaiseEvent(new MainTitleBar.NavigateToPageEventArgs(NavigateToPageEvent, pageKey!, title));
        }
    }

    private void BottomNavItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.DataContext is null) return;
        var item = button.DataContext;
        var type = item.GetType();

        var pageKeyProp = type.GetProperty("PageKey");
        var titleProp = type.GetProperty("Title");
        var pageKey = pageKeyProp?.GetValue(item)?.ToString();
        var title = titleProp?.GetValue(item)?.ToString() ?? pageKey ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(pageKey))
        {
            RaiseEvent(new MainTitleBar.NavigateToPageEventArgs(NavigateToPageEvent, pageKey!, title));
        }
    }

    #endregion

    /// <summary>
    /// 递归更新导航项样式（切换收缩/展开时调用）。
    /// </summary>
    private void UpdateNavItemStyles(ItemsControl? itemsControl, bool collapsed)
    {
        if (itemsControl is null) return;

        foreach (var item in itemsControl.Items)
        {
            var container = itemsControl.ItemContainerGenerator.ContainerFromItem(item);
            if (container is FrameworkElement element)
            {
                var button = FindVisualChild<Button>(element);
                if (button != null)
                {
                    button.Style = collapsed
                        ? (Style)FindResource("NavItemCollapsedStyle")
                        : (Style)FindResource("NavItemStyle");
                }
            }
        }
    }

    private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T result)
                return result;

            var childOfChild = FindVisualChild<T>(child);
            if (childOfChild != null)
                return childOfChild;
        }
        return null;
    }

    /// <summary>
    /// 收缩侧边栏时，折叠所有导航节点（NavigationItems / BottomNavigationItems 的 IsExpanded=false）。
    /// 仅依赖属性名，不依赖具体 ViewModel 类型。
    /// </summary>
    private void CollapseAllNavigationItems()
    {
        if (DataContext is null) return;

        var dcType = DataContext.GetType();
        var navProp = dcType.GetProperty("NavigationItems");
        var bottomProp = dcType.GetProperty("BottomNavigationItems");

        if (navProp?.GetValue(DataContext) is IEnumerable navItems)
        {
            foreach (var item in navItems)
            {
                CollapseItemRecursive(item);
            }
        }

        if (bottomProp?.GetValue(DataContext) is IEnumerable bottomItems)
        {
            foreach (var item in bottomItems)
            {
                CollapseItemRecursive(item);
            }
        }
    }

    private static void CollapseItemRecursive(object? item)
    {
        if (item is null) return;
        var type = item.GetType();

        // 设置 IsExpanded=false（如果存在）
        var isExpandedProp = type.GetProperty("IsExpanded", BindingFlags.Public | BindingFlags.Instance);
        if (isExpandedProp != null && isExpandedProp.CanWrite)
        {
            isExpandedProp.SetValue(item, false);
        }

        // 递归 Children
        var childrenProp = type.GetProperty("Children", BindingFlags.Public | BindingFlags.Instance);
        if (childrenProp?.GetValue(item) is IEnumerable children)
        {
            foreach (var child in children)
            {
                CollapseItemRecursive(child);
            }
        }
    }
}


