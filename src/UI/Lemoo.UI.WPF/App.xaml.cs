using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using Lemoo.UI.Abstractions;
using Lemoo.UI.WPF.Abstractions;
using Lemoo.UI.WPF.Constants;
using Lemoo.UI.WPF.Services;
using Lemoo.UI.WPF.ViewModels;
using Lemoo.UI.WPF.ViewModels.Pages;
using Lemoo.UI.WPF.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Lemoo.UI.WPF;

/// <summary>
/// Lemoo.UI.WPF 应用程序入口
/// 简洁的启动配置，仅实现 UI 内容的启动
/// </summary>
public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    /// <summary>
    /// 获取服务提供者（用于依赖注入）
    /// </summary>
    public IServiceProvider? Services => _serviceProvider;

    /// <summary>
    /// 应用程序启动时调用
    /// </summary>
    private void OnStartup(object sender, StartupEventArgs e)
    {
        // 初始化主题系统（在创建任何窗口之前）
        Lemoo.UI.Helpers.ThemeManager.Initialize();

        // 配置依赖注入容器
        var services = new ServiceCollection();

        // 注册页面注册服务
        var pageRegistry = new PageRegistryService();
        services.AddSingleton<IPageRegistry>(pageRegistry);

        // 注册导航服务
        services.AddSingleton<INavigationService, NavigationService>();

        // 注册剪贴板服务
        services.AddSingleton<IClipboardService, ClipboardService>();

        // 注册通知服务
        services.AddSingleton<INotificationService, NotificationService>();

        // 注册示例页面（用于UI框架测试）
        RegisterSamplePages(pageRegistry);

        // 注册 ViewModel（使用简化版本，不依赖 MediatR 和 Logger）
        services.AddSingleton<IMainViewModel, MainViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<ToolboxSampleViewModel>();

        // 注册 MainWindow（使用工厂方法以便注入服务）
        services.AddTransient<MainWindow>(provider =>
        {
            return new MainWindow(provider);
        });

        // 构建服务提供者
        _serviceProvider = services.BuildServiceProvider();

        // 配置 CommunityToolkit.Mvvm 的 Ioc
        Ioc.Default.ConfigureServices(_serviceProvider);

        // 显示主窗口
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    /// <summary>
    /// 应用程序退出时调用
    /// </summary>
    private void OnExit(object sender, ExitEventArgs e)
    {
        // 清理资源
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    /// <summary>
    /// 注册示例页面（用于UI框架测试，不包含业务模块）
    /// </summary>
    private void RegisterSamplePages(PageRegistryService pageRegistry)
    {
        // 设置示例
        pageRegistry.RegisterPage(PageKeys.Settings, typeof(Views.Pages.SettingsSamplePage), new NavigationItemMetadata
        {
            PageKey = PageKeys.Settings,
            Title = NavigationConstants.MenuText.SettingsSample,
            Icon = NavigationConstants.Icons.Settings,
            Module = "UI.Framework",
            Order = 1
        });

        // 工具箱示例
        pageRegistry.RegisterPage(PageKeys.ToolboxSample, typeof(Views.Pages.ToolboxSamplePage), new NavigationItemMetadata
        {
            PageKey = PageKeys.ToolboxSample,
            Title = NavigationConstants.MenuText.ToolboxSample,
            Icon = NavigationConstants.Icons.Toolbox,
            Module = "UI.Framework",
            Order = 2
        });

        // 图标浏览器
        pageRegistry.RegisterPage(PageKeys.IconBrowser, typeof(Views.Pages.IconBrowserPage), new NavigationItemMetadata
        {
            PageKey = PageKeys.IconBrowser,
            Title = NavigationConstants.MenuText.IconBrowser,
            Icon = NavigationConstants.Icons.IconBrowser,
            Module = "UI.Framework",
            Order = 3
        });

    }
}
