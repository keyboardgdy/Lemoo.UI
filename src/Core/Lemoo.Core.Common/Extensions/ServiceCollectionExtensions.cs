using Microsoft.Extensions.DependencyInjection;

namespace Lemoo.Core.Common.Extensions;

/// <summary>
/// 服务集合扩展方法
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 注册所有实现指定接口的类型
    /// </summary>
    public static IServiceCollection RegisterAllImplementations<TInterface>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TInterface : class
    {
        var interfaceType = typeof(TInterface);
        var implementations = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => interfaceType.IsAssignableFrom(type) 
                          && !type.IsInterface 
                          && !type.IsAbstract);
        
        foreach (var implementation in implementations)
        {
            services.Add(new ServiceDescriptor(interfaceType, implementation, lifetime));
        }
        
        return services;
    }
}

