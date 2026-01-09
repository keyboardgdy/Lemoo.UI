using Lemoo.Core.Application.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lemoo.Bootstrap;

/// <summary>
/// Host构建器扩展方法
/// </summary>
public static class HostBuilderExtensions
{
    /// <summary>
    /// 配置Lemoo应用程序（用于WPF等桌面应用）
    /// </summary>
    public static IHostBuilder ConfigureLemooApplication(
        this IHostBuilder hostBuilder,
        IConfiguration configuration,
        Action<IServiceCollection, IConfiguration>? configureServices = null)
    {
        var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
        var logger = loggerFactory.CreateLogger<Bootstrapper>();
        var bootstrapper = new Bootstrapper(configuration, logger);
        
        // 注册Bootstrapper为单例，以便后续使用
        hostBuilder.ConfigureServices((context, services) =>
        {
            services.AddSingleton(bootstrapper);
        });
        
        // 配置Host
        bootstrapper.ConfigureHost(hostBuilder);
        
        // 注册服务
        hostBuilder.ConfigureServices((context, services) =>
        {
            // 注册核心服务
            services.AddLemooCore(configuration);
            
            // 注册模块服务
            bootstrapper.RegisterServices(services, configuration);
            
            // 注册CQRS管道行为
            services.AddCqrsPipelineBehaviors();
            
            // 自定义服务配置
            configureServices?.Invoke(services, configuration);
        });
        
        return hostBuilder;
    }
}

