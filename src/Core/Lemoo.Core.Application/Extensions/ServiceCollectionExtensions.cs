using Lemoo.Core.Application.Behaviors;
using Lemoo.Core.Application.Metrics;
using Lemoo.Core.Abstractions.CQRS;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lemoo.Core.Application.Extensions;

/// <summary>
/// 服务集合扩展方法
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加CQRS管道行为
    /// </summary>
    public static IServiceCollection AddCqrsPipelineBehaviors(this IServiceCollection services)
    {
        // 注册性能指标收集器（单例）
        services.AddSingleton<PerformanceMetrics>();
        
        // 注册管道行为（按执行顺序注册）
        // 1. 日志行为（最外层）
        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        
        // 2. 验证行为
        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        // 3. 缓存行为（仅对查询）
        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(CacheBehavior<,>));
        
        // 4. 事务行为（仅对命令，最内层）
        // 注意：TransactionBehavior 需要 IUnitOfWork，模块需要在 ConfigureServices 中注册 IUnitOfWork
        // 如果模块未注册 IUnitOfWork，此行为将无法正常工作
        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        
        return services;
    }
}

