using Lemoo.Core.Abstractions.Caching;
using Lemoo.Core.Abstractions.Configuration;
using Lemoo.Core.Abstractions.Domain;
using Lemoo.Core.Abstractions.Files;
using Lemoo.Core.Abstractions.Jobs;
using Lemoo.Core.Abstractions.Localization;
using Lemoo.Core.Abstractions.Logging;
using Lemoo.Core.Abstractions.Messaging;
using Lemoo.Core.Abstractions.Security;
using Lemoo.Core.Abstractions.Services;
using Lemoo.Core.Abstractions.Specifications;
using Lemoo.Core.Infrastructure.Caching;
using Lemoo.Core.Infrastructure.Configuration;
using Lemoo.Core.Infrastructure.Domain;
using Lemoo.Core.Infrastructure.Files;
using Lemoo.Core.Infrastructure.Interceptors;
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
/// Infrastructure services registration extensions
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add infrastructure services with proper lifetime management
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register caching services
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, MemoryCacheService>();

        // Register configuration service (requires IConfigurationRoot)
        services.AddSingleton<IConfigurationService>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>() as IConfigurationRoot
                ?? throw new InvalidOperationException("Configuration must be IConfigurationRoot type to support hot reload");
            return new ConfigurationService(config, sp);
        });

        // Register logging service
        services.AddSingleton<ILoggerService, LoggerService>();

        // Register file service
        services.AddSingleton<IFileService, LocalFileService>();

        // Register message bus (default to InMemory, can switch to RabbitMQ via configuration)
        var messageBusType = configuration.GetValue<string>("Lemoo:Messaging:Type", "InMemory");
        if (messageBusType.Equals("RabbitMQ", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IMessageBus, RabbitMqMessageBus>();
        }
        else
        {
            services.AddSingleton<IMessageBus, InMemoryMessageBus>();
        }

        // Register service clients
        services.AddHttpClient();
        services.AddScoped(typeof(IServiceClient<>), typeof(ServiceClient<>));

        // Register background job service (Hangfire)
        services.AddSingleton<IBackgroundJobService, HangfireJobService>();

        // Register localization service
        services.AddSingleton<ILocalizationService, ResourceFileLocalizationService>();

        // Register authentication/authorization services
        services.AddSingleton<IAuthenticationService, JwtAuthenticationService>();
        services.AddSingleton<IAuthorizationService, PolicyAuthorizationService>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IUserContext, UserContext>();

        // Register domain event dispatcher
        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();

        // Register specification evaluator
        services.AddSingleton<ISpecificationEvaluator, SpecificationEvaluator>();

        // Register EF Core interceptors
        services.AddScoped<AuditSaveChangesInterceptor>();
        services.AddScoped<SoftDeleteSaveChangesInterceptor>();

        return services;
    }
}

