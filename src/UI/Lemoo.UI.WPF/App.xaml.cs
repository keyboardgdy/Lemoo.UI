using System.Windows;
using Lemoo.Core.Abstractions.UI;
using Lemoo.UI.WPF.Services;
using Lemoo.UI.WPF.ViewModels;
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
        
        // 注册示例页面（用于UI框架测试）
        RegisterSamplePages(pageRegistry);

        // 注册 ViewModel（使用简化版本，不依赖 MediatR 和 Logger）
        services.AddSingleton<MainViewModel>();

        // 注册 MainWindow（使用工厂方法以便注入服务）
        services.AddTransient<MainWindow>(provider =>
        {
            return new MainWindow(provider);
        });

        // 构建服务提供者
        _serviceProvider = services.BuildServiceProvider();

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
        // 注册示例页面
        pageRegistry.RegisterPage("dashboard", typeof(Views.Pages.DashboardPage), new NavigationItemMetadata
        {
            PageKey = "dashboard",
            Title = "仪表盘",
            Icon = "\uE80F",
            Module = "UI.Framework",
            Order = 1
        });
        
        pageRegistry.RegisterPage("settings", typeof(Views.Pages.SettingsSamplePage), new NavigationItemMetadata
        {
            PageKey = "settings",
            Title = "设置示例",
            Icon = "\uE713",
            Module = "UI.Framework",
            Order = 2
        });
        
        pageRegistry.RegisterPage("win11-combobox", typeof(Views.Pages.Win11ComboBoxSamplePage), new NavigationItemMetadata
        {
            PageKey = "win11-combobox",
            Title = "Win11 下拉示例",
            Icon = "\uE74E",
            Module = "UI.Framework",
            Order = 3
        });
    }
}
