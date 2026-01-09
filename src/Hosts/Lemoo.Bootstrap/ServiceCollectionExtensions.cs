using Lemoo.Core.Abstractions.Caching;
using Lemoo.Core.Abstractions.Configuration;
using Lemoo.Core.Abstractions.Deployment;
using Lemoo.Core.Abstractions.Persistence;
using Lemoo.Core.Infrastructure.Caching;
using Lemoo.Core.Infrastructure.Configuration;
using Lemoo.Core.Infrastructure.Deployment;
using Lemoo.Core.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lemoo.Bootstrap;

/// <summary>
/// 服务集合扩展方法
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加Lemoo核心服务
    /// </summary>
    public static IServiceCollection AddLemooCore(this IServiceCollection services, IConfiguration configuration)
    {
        // 注册核心服务
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<IDeploymentModeService, DeploymentModeService>();
        services.AddSingleton<IModuleDbContextFactory, ModuleDbContextFactory>();
        services.AddSingleton<ICacheService, MemoryCacheService>();
        
        return services;
    }
}

