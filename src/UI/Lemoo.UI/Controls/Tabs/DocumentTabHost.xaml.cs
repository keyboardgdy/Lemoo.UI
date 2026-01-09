using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lemoo.UI.Controls.Tabs;

/// <summary>
/// 主内容区域的标签页管理控件：支持新建、关闭、拖拽排序和当前标签高亮。
/// </summary>
public partial class DocumentTabHost : UserControl, INotifyPropertyChanged
{
    public ObservableCollection<DocumentTab> Tabs { get; } = new();
    private readonly Dictionary<string, DocumentTab> _tabsByPageKey = new();

    private DocumentTab? _selectedTab;

    public DocumentTab? SelectedTab
    {
        get => _selectedTab;
        set
        {
            if (_selectedTab == value) return;
            _selectedTab = value;
            UpdateActiveState();
            OnPropertyChanged();
        }
    }

    public DocumentTabHost()
    {
        InitializeComponent();
        DataContext = this;

        // 注册 F11 快捷键以切换窗口全屏
        Loaded += (_, _) =>
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.KeyDown += Window_KeyDown;
            }
        };
    }

    private bool _isFullscreen = false;
    private bool _isWindowFullscreen = false;
    private Visibility? _prevSidebarVisibility;
    private Visibility? _prevTitleBarVisibility;
    private Thickness? _prevContentMargin;
    private bool _prevTopmost;
    private WindowState _prevWindowState;

    private void FullscreenButton_Click(object sender, RoutedEventArgs e)
    {
        _isFullscreen = !_isFullscreen;
        UpdateFullscreenState();
    }

    private void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.F11)
        {
            ToggleWindowFullscreen();
            e.Handled = true;
        }
    }

    private void UpdateFullscreenState()
    {
        var mainWindow = Window.GetWindow(this);
        if (mainWindow is null) return;

        if (_isFullscreen)
        {
            var sidebar = mainWindow.FindName("Sidebar") as UIElement;
            if (sidebar != null) sidebar.Visibility = Visibility.Collapsed;
            var contentGrid = mainWindow.FindName("DocumentHostGrid") as Grid;
            if (contentGrid != null) contentGrid.Margin = new Thickness(0);
        }
        else
        {
            var sidebar = mainWindow.FindName("Sidebar") as UIElement;
            if (sidebar != null) sidebar.Visibility = Visibility.Visible;
            var contentGrid = mainWindow.FindName("DocumentHostGrid") as Grid;
            if (contentGrid != null) contentGrid.Margin = new Thickness(8, 8, 8, 0);
        }

        if (FindName("FullscreenIcon") is TextBlock pageIcon)
        {
            pageIcon.Text = _isFullscreen ? "\uE73F" : "\uE740";
        }
    }

    private void WindowFullscreenButton_Click(object sender, RoutedEventArgs e)
    {
        ToggleWindowFullscreen();
    }

    private void ToggleWindowFullscreen()
    {
        var mainWindow = Window.GetWindow(this);
        if (mainWindow is null) return;

        _isWindowFullscreen = !_isWindowFullscreen;

        if (_isWindowFullscreen)
        {
            _prevSidebarVisibility = (mainWindow.FindName("Sidebar") as UIElement)?.Visibility;
            _prevTitleBarVisibility = (mainWindow.FindName("MainTitleBar") as UIElement)?.Visibility;
            _prevContentMargin = (mainWindow.FindName("DocumentHostGrid") as Grid)?.Margin;
            _prevTopmost = mainWindow.Topmost;
            _prevWindowState = mainWindow.WindowState;

            var sidebar = mainWindow.FindName("Sidebar") as UIElement;
            if (sidebar != null) sidebar.Visibility = Visibility.Collapsed;
            var titleBar = mainWindow.FindName("MainTitleBar") as UIElement;
            if (titleBar != null) titleBar.Visibility = Visibility.Collapsed;
            var contentGrid = mainWindow.FindName("DocumentHostGrid") as Grid;
            if (contentGrid != null) contentGrid.Margin = new Thickness(0);

            var titleRow = mainWindow.FindName("TitleRow") as RowDefinition;
            if (titleRow != null) titleRow.Height = new GridLength(0);

            mainWindow.Topmost = true;
            mainWindow.WindowState = WindowState.Maximized;
        }
        else
        {
            var sidebar = mainWindow.FindName("Sidebar") as UIElement;
            if (sidebar != null && _prevSidebarVisibility.HasValue) sidebar.Visibility = _prevSidebarVisibility.Value;
            var titleBar = mainWindow.FindName("MainTitleBar") as UIElement;
            if (titleBar != null && _prevTitleBarVisibility.HasValue) titleBar.Visibility = _prevTitleBarVisibility.Value;
            var contentGrid = mainWindow.FindName("DocumentHostGrid") as Grid;
            if (contentGrid != null && _prevContentMargin.HasValue) contentGrid.Margin = _prevContentMargin.Value;

            var titleRow = mainWindow.FindName("TitleRow") as RowDefinition;
            if (titleRow != null) titleRow.Height = new GridLength(32);

            mainWindow.Topmost = _prevTopmost;
            mainWindow.WindowState = _prevWindowState;
        }

        if (FindName("WindowFullscreenIcon") is TextBlock winIcon)
        {
            winIcon.Text = _isWindowFullscreen ? "\uE923" : "\uE922";
        }
    }

    /// <summary>
    /// 打开页面，如果页面已存在则聚焦到该标签页，否则创建新标签页。
    /// </summary>
    public void OpenPage(string title, Page page, string pageKey)
    {
        if (_tabsByPageKey.TryGetValue(pageKey, out var existingTab))
        {
            SelectedTab = existingTab;
            return;
        }

        var tab = new DocumentTab(title, page, pageKey);
        Tabs.Add(tab);
        _tabsByPageKey[pageKey] = tab;
        SelectedTab = tab;
    }

    private void CloseTab_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement fe || fe.DataContext is not DocumentTab tab) return;

        var index = Tabs.IndexOf(tab);
        if (index < 0) return;

        _tabsByPageKey.Remove(tab.PageKey);
        Tabs.RemoveAt(index);

        if (Tabs.Count == 0)
        {
            SelectedTab = null;
            return;
        }

        if (index >= Tabs.Count)
            index = Tabs.Count - 1;
        SelectedTab = Tabs[index];
    }

    private Point _dragStartPoint;

    private void TabHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _dragStartPoint = e.GetPosition(null);

        if (sender is FrameworkElement fe && fe.DataContext is DocumentTab tab)
        {
            SelectedTab = tab;
        }
    }

    private void TabHeader_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
            return;

        var position = e.GetPosition(null);
        if (Math.Abs(position.X - _dragStartPoint.X) < SystemParameters.MinimumHorizontalDragDistance &&
            Math.Abs(position.Y - _dragStartPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
        {
            return;
        }

        if (sender is FrameworkElement fe && fe.DataContext is DocumentTab tab)
        {
            DragDrop.DoDragDrop(fe, tab, DragDropEffects.Move);
        }
    }

    private void TabHeader_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(typeof(DocumentTab))) return;
        var sourceTab = (DocumentTab?)e.Data.GetData(typeof(DocumentTab));
        if (sourceTab is null) return;

        if (sender is not FrameworkElement fe || fe.DataContext is not DocumentTab targetTab) return;

        var sourceIndex = Tabs.IndexOf(sourceTab);
        var targetIndex = Tabs.IndexOf(targetTab);
        if (sourceIndex < 0 || targetIndex < 0 || sourceIndex == targetIndex) return;

        Tabs.Move(sourceIndex, targetIndex);
        SelectedTab = sourceTab;
    }

    private void UpdateActiveState()
    {
        foreach (var tab in Tabs)
        {
            var shouldBeActive = tab == SelectedTab;
            if (tab.IsActive != shouldBeActive)
            {
                tab.IsActive = shouldBeActive;
            }
        }
    }

    // 右键菜单
    private DocumentTab? _contextMenuTab;

    private void TabHeader_ContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is DocumentTab tab)
        {
            _contextMenuTab = tab;
            SelectedTab = tab;
        }
    }

    private void ContextMenu_CloseTab_Click(object sender, RoutedEventArgs e)
    {
        if (_contextMenuTab is null) return;

        var index = Tabs.IndexOf(_contextMenuTab);
        if (index < 0) return;

        _tabsByPageKey.Remove(_contextMenuTab.PageKey);
        Tabs.RemoveAt(index);

        if (Tabs.Count == 0)
        {
            SelectedTab = null;
            _contextMenuTab = null;
            return;
        }

        if (index >= Tabs.Count)
            index = Tabs.Count - 1;
        SelectedTab = Tabs[index];
        _contextMenuTab = null;
    }

    private void ContextMenu_CloseOtherTabs_Click(object sender, RoutedEventArgs e)
    {
        if (_contextMenuTab is null) return;

        var tabsToRemove = Tabs.Where(t => t != _contextMenuTab).ToList();
        foreach (var tab in tabsToRemove)
        {
            _tabsByPageKey.Remove(tab.PageKey);
        }

        for (int i = Tabs.Count - 1; i >= 0; i--)
        {
            if (Tabs[i] != _contextMenuTab)
            {
                Tabs.RemoveAt(i);
            }
        }

        SelectedTab = _contextMenuTab;
        _contextMenuTab = null;
    }

    private void ContextMenu_CloseAllTabs_Click(object sender, RoutedEventArgs e)
    {
        _tabsByPageKey.Clear();
        Tabs.Clear();
        SelectedTab = null;
        _contextMenuTab = null;
    }

    private void TabBar_ContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        if (Tabs.Count == 0)
        {
            e.Handled = true;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

/// <summary>
/// 标签页模型（标题、页面和激活状态）。
/// </summary>
public class DocumentTab : INotifyPropertyChanged
{
    private bool _isActive;

    public string Title { get; set; }
    public Page? Page { get; set; }
    public string PageKey { get; set; }

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive == value) return;
            _isActive = value;
            OnPropertyChanged();
        }
    }

    public DocumentTab(string title, Page? page, string pageKey)
    {
        Title = title;
        Page = page;
        PageKey = pageKey;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

