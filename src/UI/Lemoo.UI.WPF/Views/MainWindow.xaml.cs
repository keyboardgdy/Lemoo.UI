using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Shell;
using System.Windows.Threading;
using Lemoo.UI.Abstractions;
using Lemoo.UI.Controls.Chrome;
using Lemoo.UI.Controls.Tabs;
using Lemoo.UI.Helpers;
using Lemoo.UI.WPF.Abstractions;
using Lemoo.UI.WPF.Constants;
using Microsoft.Extensions.DependencyInjection;

namespace Lemoo.UI.WPF.Views;

/// <summary>
/// 主窗口视图
/// </summary>
public partial class MainWindow : Window
{
    private double _restoreLeft;
    private double _restoreTop;
    private double _restoreWidth;
    private double _restoreHeight;
    private bool _isInitialized;
    private WindowState _previousState = WindowState.Normal;
    private readonly IPageRegistry? _pageRegistry;
    private readonly IServiceProvider? _serviceProvider;

    public MainWindow()
    {
        InitializeComponent();
    }
    
    public MainWindow(IServiceProvider serviceProvider) : this()
    {
        _serviceProvider = serviceProvider;
        _pageRegistry = serviceProvider.GetService<IPageRegistry>();

        // 从服务提供者获取ViewModel
        var viewModel = serviceProvider.GetRequiredService<IMainViewModel>();
        DataContext = viewModel;

        // 使用 WindowChrome 完全控制窗口边框（标题栏区域由自定义控件接管）
        var chrome = new WindowChrome
        {
            CaptionHeight = 0,
            ResizeBorderThickness = new Thickness(0),
            GlassFrameThickness = new Thickness(0),
            UseAeroCaptionButtons = false,
            NonClientFrameEdges = NonClientFrameEdges.None
        };
        WindowChrome.SetWindowChrome(this, chrome);

        Loaded += MainWindow_Loaded;
        SourceInitialized += MainWindow_SourceInitialized;
        StateChanged += MainWindow_StateChanged;
    }

    #region Windows 11 圆角支持

    private enum DWMWINDOWATTRIBUTE
    {
        DWMWA_WINDOW_CORNER_PREFERENCE = 33
    }

    private enum DWM_WINDOW_CORNER_PREFERENCE : uint
    {
        DWMWCP_DEFAULT = 0,
        DWMWCP_DONOTROUND = 1,
        DWMWCP_ROUND = 2,
        DWMWCP_ROUNDSMALL = 3
    }

    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(
        IntPtr hwnd,
        DWMWINDOWATTRIBUTE dwAttribute,
        ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
        int cbAttribute);

    private static bool IsWindows11OrGreater()
    {
        var v = Environment.OSVersion.Version;
        return v.Major >= 10 && v.Build >= 22000;
    }

    private void EnableRoundedCornersIfSupported()
    {
        if (!IsWindows11OrGreater())
            return;

        var hwnd = new WindowInteropHelper(this).Handle;
        if (hwnd == IntPtr.Zero)
            return;

        var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
        DwmSetWindowAttribute(
            hwnd,
            DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE,
            ref preference,
            sizeof(uint));
    }

    #endregion

    /// <summary>
    /// 接收标题栏和侧边栏的导航事件，在右侧标签页中打开页面。
    /// </summary>
    private void OnNavigateToPage(object sender, RoutedEventArgs e)
    {
        if (e is not MainTitleBar.NavigateToPageEventArgs args)
            return;

        // 从页面注册服务创建页面
        Page? page = null;
        if (_pageRegistry != null && _serviceProvider != null)
        {
            var pageObject = _pageRegistry.CreatePage(args.PageKey, _serviceProvider);
            page = pageObject as Page;
        }

        // 如果未找到页面，创建占位页面
        if (page == null)
        {
            page = CreatePlaceholderPage(args);
        }

        DocumentTabHost.OpenPage(args.PageTitle, page, args.PageKey);
    }

    /// <summary>
    /// 原来的简单占位 Page，保留以便未知 PageKey 时仍然有反馈。
    /// </summary>
    private static Page CreatePlaceholderPage(MainTitleBar.NavigateToPageEventArgs args)
    {
        return new Page
        {
            Content = new TextBlock
            {
                Text = $"{args.PageTitle} ({args.PageKey})",
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 18,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            },
            Background = System.Windows.Media.Brushes.Transparent
        };
    }

    #region 窗口尺寸/拖动逻辑（匹配参考主窗体行为）

    public void RestoreWindowForDrag(double screenX, double screenY, double percentX, double posY)
    {
        var source = PresentationSource.FromVisual(this);
        Point wpfScreenPoint = new Point(screenX, screenY);
        if (source?.CompositionTarget != null)
        {
            wpfScreenPoint = source.CompositionTarget.TransformFromDevice.Transform(wpfScreenPoint);
        }

        double restoreWidth = _restoreWidth > 0 ? _restoreWidth : Math.Max(800, SystemParameters.WorkArea.Width * 0.8);
        double restoreHeight = _restoreHeight > 0 ? _restoreHeight : Math.Max(600, SystemParameters.WorkArea.Height * 0.8);

        double newLeft = wpfScreenPoint.X - percentX * restoreWidth;
        double newTop = wpfScreenPoint.Y - posY;

        var work = SystemParameters.WorkArea;
        if (newLeft + restoreWidth < work.Left + 50) newLeft = work.Left + 50 - restoreWidth;
        if (newLeft > work.Right - 50) newLeft = work.Right - 50;
        if (newTop < work.Top) newTop = work.Top;
        if (newTop + restoreHeight > work.Bottom) newTop = work.Bottom - restoreHeight;

        Left = newLeft;
        Top = newTop;
        Width = restoreWidth;
        Height = restoreHeight;
        WindowState = WindowState.Normal;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (Content is FrameworkElement content)
        {
            content.Margin = new Thickness(0);
        }

        if (!_isInitialized)
        {
            _restoreLeft = Left;
            _restoreTop = Top;
            _restoreWidth = Width;
            _restoreHeight = Height;
            _isInitialized = true;

            // 首次启动时自动打开默认页面
            if (DocumentTabHost.Tabs.Count == 0 && _pageRegistry != null && _serviceProvider != null)
            {
                var defaultPageObject = _pageRegistry.CreatePage(PageKeys.Settings, _serviceProvider);
                if (defaultPageObject is Page defaultPage)
                {
                    DocumentTabHost.OpenPage(NavigationConstants.MenuText.SettingsSample, defaultPage, PageKeys.Settings);
                }
            }
        }
    }

    private void MainWindow_SourceInitialized(object? sender, EventArgs e)
    {
        var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
        if (hwndSource != null)
        {
            hwndSource.AddHook(WndProc);
        }

        // Windows 11 上启用系统级圆角，减少锯齿
        EnableRoundedCornersIfSupported();
    }

    public void SaveWindowStateBeforeMaximize()
    {
        if (WindowState == WindowState.Normal)
        {
            _restoreLeft = Left;
            _restoreTop = Top;
            _restoreWidth = Width;
            _restoreHeight = Height;
        }
    }

    private const int WM_GETMINMAXINFO = 0x24;

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_GETMINMAXINFO)
        {
            WmGetMinMaxInfo(hwnd, lParam);
            handled = false;
        }

        return IntPtr.Zero;
    }

    private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
    {
        try
        {
            var monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                var mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                var mi = new MONITORINFO();
                mi.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                if (GetMonitorInfo(monitor, ref mi))
                {
                    int workWidth = mi.rcWork.right - mi.rcWork.left;
                    int workHeight = mi.rcWork.bottom - mi.rcWork.top;
                    mmi.ptMaxPosition.x = mi.rcWork.left;
                    mmi.ptMaxPosition.y = mi.rcWork.top;
                    mmi.ptMaxSize.x = workWidth;
                    mmi.ptMaxSize.y = workHeight;

                    Marshal.StructureToPtr(mmi, lParam, true);
                }
            }
        }
        catch
        {
            // 忽略 interop 错误
        }
    }

    private void MainWindow_StateChanged(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Normal && _previousState == WindowState.Maximized)
        {
            if (_restoreWidth > 0)
            {
                Left = _restoreLeft;
                Top = _restoreTop;
                Width = _restoreWidth;
                Height = _restoreHeight;
            }
        }

        _previousState = WindowState;
    }

    #endregion
}


