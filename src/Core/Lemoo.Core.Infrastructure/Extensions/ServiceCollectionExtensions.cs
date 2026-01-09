using Lemoo.Core.Abstractions.Caching;
using Lemoo.Core.Abstractions.Configuration;
using Lemoo.Core.Abstractions.Files;
using Lemoo.Core.Abstractions.Jobs;
using Lemoo.Core.Abstractions.Localization;
using Lemoo.Core.Abstractions.Logging;
using Lemoo.Core.Abstractions.Messaging;
using Lemoo.Core.Abstractions.Security;
using Lemoo.Core.Abstractions.Services;
using Lemoo.Core.Infrastructure.Caching;
using Lemoo.Core.Infrastructure.Configuration;
using Lemoo.Core.Infrastructure.Files;
using Lemoo.Core.Infrastructure.Jobs;
using Lemoo.Core.Infrastructure.Localization;
using Lemoo.Core.Infrastructure.Logging;
using Lemoo.Core.Infrastructure.Messaging;
using Lemoo.Core.Infrastructure.Security;
using Lemoo.Core.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lemoo.Core.Infrastructure.Extensions;

/// <summary>
/// 基础设施服务注册扩展方法
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加基础设施服务
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // 注册缓存服务
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, MemoryCacheService>();
        
        // 注册配置服务（需要IConfigurationRoot）
        services.AddSingleton<IConfigurationService>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>() as IConfigurationRoot
                ?? throw new InvalidOperationException("配置必须是IConfigurationRoot类型以支持热更新");
            return new ConfigurationService(config, sp);
        });
        
        // 注册日志服务
        services.AddSingleton<ILoggerService, LoggerService>();
        
        // 注册文件服务
        services.AddSingleton<IFileService, LocalFileService>();
        
        // 注册消息总线（默认使用内存，可通过配置切换为RabbitMQ）
        var messageBusType = configuration.GetValue<string>("Lemoo:Messaging:Type", "InMemory");
        if (messageBusType.Equals("RabbitMQ", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IMessageBus, RabbitMqMessageBus>();
        }
        else
        {
            services.AddSingleton<IMessageBus, InMemoryMessageBus>();
        }
        
        // 注册服务客户端（需要按服务类型注册）
        services.AddHttpClient(); // 添加HttpClient支持
        services.AddScoped(typeof(IServiceClient<>), typeof(ServiceClient<>));
        
        // 注册后台任务服务（Hangfire）
        services.AddSingleton<IBackgroundJobService, HangfireJobService>();
        
        // 注册本地化服务
        services.AddSingleton<ILocalizationService, ResourceFileLocalizationService>();
        
        // 注册认证授权服务
        services.AddSingleton<IAuthenticationService, JwtAuthenticationService>();
        services.AddSingleton<IAuthorizationService, PolicyAuthorizationService>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IUserContext, UserContext>();
        
        return services;
    }
}

