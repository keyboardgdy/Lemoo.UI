using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Lemoo.UI.Controls.Chrome;

/// <summary>
/// 自定义标题栏（UI + 基本窗口控制 + 菜单导航事件）。
/// </summary>
public partial class MainTitleBar : UserControl
{
    public MainTitleBar()
    {
        InitializeComponent();

        Loaded += OnLoaded;
        MouseLeftButtonDown += OnMouseLeftButtonDown;
    }

    private Window? _window;

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _window = Window.GetWindow(this);
        UpdateMaxRestoreIcon();
        if (_window is not null)
        {
            _window.StateChanged += (_, _) => UpdateMaxRestoreIcon();
        }
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // 点击菜单或按钮时不参与拖动
        var source = e.OriginalSource as DependencyObject;
        while (source != null)
        {
            if (source is Button || source is Menu || source is MenuItem)
            {
                return;
            }
            source = VisualTreeHelper.GetParent(source);
        }

        _window ??= Window.GetWindow(this);
        if (_window is null) return;

        if (e.ClickCount == 2)
        {
            ToggleWindowState();
            return;
        }

        // 如果最大化状态下拖动，尝试通过反射调用 RestoreWindowForDrag（如果窗口支持）
        if (_window.WindowState == WindowState.Maximized)
        {
            var windowType = _window.GetType();
            var restoreMethod = windowType.GetMethod("RestoreWindowForDrag");
            if (restoreMethod != null)
            {
                var posInTitle = e.GetPosition(this);
                var screenPoint = PointToScreen(posInTitle);
                double percentX = Math.Clamp(posInTitle.X / ActualWidth, 0.0, 1.0);

                try
                {
                    restoreMethod.Invoke(_window, new object[] { screenPoint.X, screenPoint.Y, percentX, posInTitle.Y });
                    _window.DragMove();
                }
                catch
                {
                }
                return;
            }
        }

        try
        {
            _window.DragMove();
        }
        catch
        {
            // ignore drag exceptions
        }
    }

    private void MinButton_Click(object sender, RoutedEventArgs e)
    {
        _window ??= Window.GetWindow(this);
        if (_window is null) return;
        _window.WindowState = WindowState.Minimized;
    }

    private void MaxRestoreButton_Click(object sender, RoutedEventArgs e)
    {
        ToggleWindowState();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        _window ??= Window.GetWindow(this);
        _window?.Close();
    }

    private void ToggleWindowState()
    {
        _window ??= Window.GetWindow(this);
        if (_window is null) return;

        // 在最大化前保存当前窗口状态，便于恢复（通过反射调用，如果窗口支持）
        if (_window.WindowState == WindowState.Normal)
        {
            var windowType = _window.GetType();
            var saveMethod = windowType.GetMethod("SaveWindowStateBeforeMaximize");
            saveMethod?.Invoke(_window, null);
        }

        _window.WindowState = _window.WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;

        UpdateMaxRestoreIcon();
    }

    private void UpdateMaxRestoreIcon()
    {
        _window ??= Window.GetWindow(this);
        if (_window is null) return;
        if (FindName("MaxRestoreIcon") is not TextBlock icon) return;
        icon.Text = _window.WindowState == WindowState.Maximized ? "\uE923" : "\uE922";
    }

    #region NavigateToPage 事件

    public static readonly RoutedEvent NavigateToPageEvent = EventManager.RegisterRoutedEvent(
        nameof(NavigateToPage), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MainTitleBar));

    public event RoutedEventHandler NavigateToPage
    {
        add => AddHandler(NavigateToPageEvent, value);
        remove => RemoveHandler(NavigateToPageEvent, value);
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem || menuItem.DataContext is null) return;

        var data = menuItem.DataContext;
        var type = data.GetType();
        var pageKeyProp = type.GetProperty("PageKey");
        var headerProp = type.GetProperty("Header");

        var pageKey = pageKeyProp?.GetValue(data)?.ToString();
        var pageTitle = headerProp?.GetValue(data)?.ToString() ?? pageKey ?? string.Empty;

        if (string.IsNullOrWhiteSpace(pageKey))
            return;

        RaiseEvent(new NavigateToPageEventArgs(NavigateToPageEvent, pageKey!, pageTitle));
    }

    /// <summary>
    /// 页面导航事件参数（简化版，与实际业务模型解耦）。
    /// </summary>
    public class NavigateToPageEventArgs : RoutedEventArgs
    {
        public string PageKey { get; }
        public string PageTitle { get; }

        public NavigateToPageEventArgs(RoutedEvent routedEvent, string pageKey, string pageTitle)
            : base(routedEvent)
        {
            PageKey = pageKey;
            PageTitle = pageTitle;
        }
    }

    #endregion
}

