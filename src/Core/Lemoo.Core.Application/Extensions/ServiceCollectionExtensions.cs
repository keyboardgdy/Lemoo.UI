using Lemoo.Core.Application.Behaviors;
using Lemoo.Core.Application.Metrics;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lemoo.Core.Application.Extensions;

/// <summary>
/// Service collection extensions for CQRS pipeline behaviors
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add CQRS pipeline behaviors with correct execution order
    ///
    /// Execution order (outer to inner):
    /// 1. Logging - logs all requests/responses
    /// 2. Validation - validates input (returns Result<T> on failure)
    /// 3. Caching - caches query results
    /// 4. Domain Events - dispatches domain events after command execution
    /// 5. Transaction - manages database transactions (innermost)
    /// </summary>
    public static IServiceCollection AddCqrsPipelineBehaviors(this IServiceCollection services)
    {
        // Register performance metrics collector (singleton)
        services.AddSingleton<PerformanceMetrics>();

        // Register pipeline behaviors in correct order (first registered = outermost)
        // 1. Logging behavior (outermost - wraps everything)
        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // 2. Validation behavior (validates before any processing)
        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // 3. Caching behavior (only for queries)
        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(CacheBehavior<,>));

        // 4. Domain event behavior (dispatches events after command execution)
        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(DomainEventBehavior<,>));

        // 5. Transaction behavior (innermost - runs actual handler)
        // Note: TransactionBehavior requires IUnitOfWork to be registered
        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        return services;
    }
}

