using System.IO;
using System.Reflection;
using System.Windows;
using Lemoo.Bootstrap;
using Lemoo.Core.Abstractions.UI;
using Lemoo.Core.Application.Extensions;
using Lemoo.Modules.Abstractions;
using Lemoo.UI.WPF.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lemoo.Host;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IHost? _host;
    
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        try
        {
            // 初始化主题系统（在创建任何窗口之前）
            Lemoo.UI.Helpers.ThemeManager.Initialize();
            
            // 构建配置
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            
            // 创建Host Builder并配置Lemoo应用程序
            var hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.Sources.Clear();
                    config.AddConfiguration(configuration);
                })
                .ConfigureLemooApplication(configuration, (services, config) =>
                {
                    // 直接注册通过项目引用方式加载的模块（避免仅从 /Modules 目录加载导致模块服务未注册）
                    // RegisterProjectReferencedModules(services, config);
                    
                    // 注册UI框架服务
                    RegisterUIFrameworkServices(services);
                    
                    // 注册所有模块UI
                    RegisterModuleUIs(services, config);
                });
            
            // 构建Host
            _host = hostBuilder.Build();
            
            // 获取引导器并执行引导
            var bootstrapper = _host.Services.GetRequiredService<Bootstrapper>();
            var bootstrapResult = await bootstrapper.BootstrapAsync();
            if (!bootstrapResult.IsSuccess)
            {
                MessageBox.Show(
                    $"应用程序启动失败:\n{string.Join("\n", bootstrapResult.Errors.Select(e => e.Message))}",
                    "启动错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
                return;
            }
            
            // 启动模块生命周期
            await bootstrapper.StartModulesAsync(_host.Services);
            
            // 初始化并显示主窗口
            InitializeMainWindow(_host.Services);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"应用程序启动时发生错误:\n{ex.Message}",
                "启动错误",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
        }
    }
    
    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host != null)
        {
            // 停止模块生命周期
            var bootstrapper = _host.Services.GetService<Bootstrapper>();
            if (bootstrapper != null)
            {
                await bootstrapper.StopModulesAsync(_host.Services);
            }
            
            // 停止Host
            await _host.StopAsync();
            _host.Dispose();
        }
        
        base.OnExit(e);
    }
    
    private void RegisterUIFrameworkServices(IServiceCollection services)
    {
        // 注册页面注册服务
        services.AddSingleton<IPageRegistry, PageRegistryService>();
        services.AddSingleton<NavigationService>();
        
        // 注册UI框架的MainWindow和MainViewModel（使用Transient创建独立实例，不影响UI.WPF）
        services.AddTransient<Lemoo.UI.WPF.ViewModels.MainViewModel>(provider =>
        {
            // 创建时不初始化默认导航（Host有自己的导航逻辑）
            return new Lemoo.UI.WPF.ViewModels.MainViewModel(skipDefaultNavigation: true);
        });
        
        services.AddTransient<Lemoo.UI.WPF.Views.MainWindow>();
    }

    /// <summary>
    /// 注册通过项目引用引入的模块（开发场景下无需拷贝到 Modules 目录也能使用）
    /// </summary>
    private void RegisterProjectReferencedModules(IServiceCollection services, IConfiguration configuration)
    {

    }
    
    private void RegisterModuleUIs(IServiceCollection services, IConfiguration configuration)
    {
        try
        {
            // 发现所有模块UI
            var moduleUIs = DiscoverModuleUIs();
            System.Diagnostics.Debug.WriteLine($"[ModuleUI] 发现 {moduleUIs.Count} 个模块UI");
            
            var pageRegistry = new PageRegistryService();
            int registeredPageCount = 0;
            
            foreach (var moduleUI in moduleUIs)
            {
                System.Diagnostics.Debug.WriteLine($"[ModuleUI] 注册模块UI: {moduleUI.ModuleName}");
                
                // 注册模块UI的服务
                moduleUI.RegisterUI(services);
                
                // 注册模块UI的页面和导航项
                var navItems = moduleUI.GetNavigationItems();
                System.Diagnostics.Debug.WriteLine($"[ModuleUI] 模块 {moduleUI.ModuleName} 有 {navItems.Count} 个导航项");
                
                foreach (var navItem in navItems)
                {
                    var pageType = DiscoverPageType(moduleUI.ModuleName, navItem.PageKey);
                    if (pageType != null)
                    {
                        pageRegistry.RegisterPage(navItem.PageKey, pageType, navItem);
                        registeredPageCount++;
                        System.Diagnostics.Debug.WriteLine($"[ModuleUI] 成功注册页面: {navItem.PageKey} -> {pageType.Name}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[ModuleUI] 警告: 未找到页面类型，PageKey: {navItem.PageKey}, Module: {moduleUI.ModuleName}");
                    }
                }
            }
            
            System.Diagnostics.Debug.WriteLine($"[ModuleUI] 总共注册了 {registeredPageCount} 个页面");
            
            // 注册页面注册服务实例（使用工厂方法）
            services.AddSingleton<IPageRegistry>(_ => pageRegistry);
            
            // 保存模块UI列表，用于后续构建导航树
            services.AddSingleton(moduleUIs);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ModuleUI] 注册模块UI时发生错误: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }
    
    private void InitializeMainWindow(IServiceProvider serviceProvider)
    {
        // 获取UI框架的MainWindow（独立实例）
        var mainWindow = serviceProvider.GetRequiredService<Lemoo.UI.WPF.Views.MainWindow>();
        
        // 获取MainViewModel（独立实例，已跳过默认导航初始化）
        var mainViewModel = serviceProvider.GetRequiredService<Lemoo.UI.WPF.ViewModels.MainViewModel>();
        
        // 清除可能存在的默认导航（确保干净状态）
        mainViewModel.ClearNavigation();
        
        // 设置Host专用的窗口标题
        mainViewModel.Title = "Lemoo - 应用程序";
        
        // 初始化模块UI的导航数据
        var moduleUIs = serviceProvider.GetRequiredService<IReadOnlyList<IModuleUI>>();
        var navigationService = serviceProvider.GetRequiredService<NavigationService>();
        var pageRegistry = serviceProvider.GetRequiredService<IPageRegistry>();
        
        // 构建导航树（只包含模块UI的导航项）
        var allNavItems = moduleUIs
            .SelectMany(m => m.GetNavigationItems())
            .Where(n => n.IsEnabled)
            .OrderBy(n => n.Order)
            .ToList();
        
        navigationService.BuildNavigationTree(mainViewModel, allNavItems);
        
        // 显示主窗口
        mainWindow.Show();
        
        // 启动时自动打开第一个模块页面（如果有）
        // 使用 Dispatcher 确保窗口已完全加载
        mainWindow.Dispatcher.InvokeAsync(() =>
        {
            if (allNavItems.Count > 0)
            {
                var firstPage = allNavItems.First();
                var pageObject = pageRegistry.CreatePage(firstPage.PageKey, serviceProvider);
                if (pageObject is System.Windows.Controls.Page page)
                {
                    // 通过 FindName 获取 DocumentTabHost（XAML 中定义的 x:Name）
                    var documentTabHost = mainWindow.FindName("DocumentTabHost") 
                        as Lemoo.UI.Controls.Tabs.DocumentTabHost;
                    documentTabHost?.OpenPage(firstPage.Title, page, firstPage.PageKey);
                }
            }
        }, System.Windows.Threading.DispatcherPriority.Loaded);
    }
    
    private IReadOnlyList<IModuleUI> DiscoverModuleUIs()
    {
        // 从当前程序集和引用的程序集中发现模块UI
        var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        System.Diagnostics.Debug.WriteLine($"[ModuleUI] 当前 AppDomain 中有 {allAssemblies.Length} 个程序集");
        
        var assemblies = allAssemblies
            .Where(a => 
            {
                var name = a.GetName().Name;
                var matches = name?.StartsWith("Lemoo.Modules.") == true 
                           && name?.EndsWith(".UI") == true;
                if (matches)
                {
                    System.Diagnostics.Debug.WriteLine($"[ModuleUI] 发现模块UI程序集: {name}");
                }
                return matches;
            })
            .ToList();
        
        System.Diagnostics.Debug.WriteLine($"[ModuleUI] 找到 {assemblies.Count} 个模块UI程序集");
        
        var moduleUITypes = assemblies
            .SelectMany(a => 
            {
                try
                {
                    return a.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ModuleUI] 加载程序集 {a.GetName().Name} 的类型时出错: {ex.Message}");
                    return ex.Types.Where(t => t != null)!;
                }
            })
            .Where(t => typeof(IModuleUI).IsAssignableFrom(t) 
                     && !t.IsAbstract 
                     && !t.IsInterface)
            .ToList();
        
        System.Diagnostics.Debug.WriteLine($"[ModuleUI] 找到 {moduleUITypes.Count} 个 IModuleUI 实现");
        
        var moduleUIs = new List<IModuleUI>();
        foreach (var type in moduleUITypes)
        {
            if (type == null) continue;
            
            try
            {
                var instance = Activator.CreateInstance(type);
                if (instance is IModuleUI moduleUI)
                {
                    moduleUIs.Add(moduleUI);
                    System.Diagnostics.Debug.WriteLine($"[ModuleUI] 成功创建模块UI实例: {type.Name} (模块: {moduleUI.ModuleName})");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ModuleUI] 创建模块UI实例失败 {type.Name ?? "Unknown"}: {ex.Message}");
            }
        }
        
        return moduleUIs;
    }
    
    private Type? DiscoverPageType(string moduleName, string pageKey)
    {
        // 从模块UI程序集中查找页面类型
        var expectedAssemblyName = $"Lemoo.Modules.{moduleName}.UI";
        var moduleUIAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == expectedAssemblyName);
        
        if (moduleUIAssembly == null)
        {
            System.Diagnostics.Debug.WriteLine($"[ModuleUI] 未找到模块UI程序集: {expectedAssemblyName}");
            return null;
        }
        
        System.Diagnostics.Debug.WriteLine($"[ModuleUI] 在程序集 {expectedAssemblyName} 中查找页面类型，PageKey: {pageKey}");
        
        // 查找Views命名空间下的页面类型
        Type[] types;
        try
        {
            types = moduleUIAssembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ModuleUI] 加载程序集类型时出错: {ex.Message}");
            types = ex.Types.Where(t => t != null).ToArray()!;
        }
        
        var pageTypes = types
            .Where(t => t.Namespace?.Contains(".Views") == true 
                     && typeof(System.Windows.Controls.Page).IsAssignableFrom(t)
                     && !t.IsAbstract)
            .ToList();
        
        System.Diagnostics.Debug.WriteLine($"[ModuleUI] 在 Views 命名空间下找到 {pageTypes.Count} 个页面类型: {string.Join(", ", pageTypes.Select(t => t.Name))}");
        
        // 尝试通过命名约定匹配（例如：ExampleListPage -> example-list）
        var expectedClassName = ConvertPageKeyToClassName(pageKey);
        System.Diagnostics.Debug.WriteLine($"[ModuleUI] 期望的页面类名: {expectedClassName}");
        
        var matchedType = pageTypes.FirstOrDefault(t => t.Name == expectedClassName);
        if (matchedType != null)
        {
            System.Diagnostics.Debug.WriteLine($"[ModuleUI] 找到匹配的页面类型: {matchedType.Name}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"[ModuleUI] 未找到匹配的页面类型，期望: {expectedClassName}");
        }
        
        return matchedType;
    }
    
    private string ConvertPageKeyToClassName(string pageKey)
    {
        // 将 page-key 转换为 PageKeyPage
        var parts = pageKey.Split('-');
        var className = string.Join("", parts.Select(p => 
            p.Length > 0 ? char.ToUpper(p[0]) + (p.Length > 1 ? p[1..] : "") : "")) + "Page";
        return className;
    }
    
    private IReadOnlyList<Assembly> DiscoverModuleAssemblies()
    {
        // 发现所有模块后端程序集
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => 
            {
                var name = a.GetName().Name;
                return name != null 
                    && name.StartsWith("Lemoo.Modules.") 
                    && !name.EndsWith(".UI")
                    && !name.EndsWith(".Abstractions");
            })
            .ToList();
    }
}
