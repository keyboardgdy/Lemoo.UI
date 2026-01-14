using Lemoo.Core.Abstractions.Module;
using Microsoft.Extensions.DependencyInjection;

namespace Lemoo.Core.Infrastructure.ModuleFactory;

/// <summary>
/// Interface for module factory with DI support
/// </summary>
public interface IModuleFactory
{
    /// <summary>
    /// Create a module instance using DI
    /// </summary>
    IModule CreateModule(Type moduleType);

    /// <summary>
    /// Create a module instance using DI
    /// </summary>
    TModule CreateModule<TModule>()
        where TModule : IModule;
}

/// <summary>
/// Module factory implementation that uses IServiceProvider for DI
/// </summary>
public class ModuleFactory : IModuleFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceScopeFactory _scopeFactory;

    public ModuleFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
    }

    /// <summary>
    /// Create a module instance using DI
    /// </summary>
    public IModule CreateModule(Type moduleType)
    {
        if (moduleType == null) throw new ArgumentNullException(nameof(moduleType));
        if (!typeof(IModule).IsAssignableFrom(moduleType))
        {
            throw new ArgumentException($"Type {moduleType.Name} does not implement IModule", nameof(moduleType));
        }

        // Create a scope for module instantiation to avoid scoped service issues
        using var scope = _scopeFactory.CreateScope();

        try
        {
            // Try to resolve from service provider first (supports constructor injection)
            var module = (IModule?)scope.ServiceProvider.GetService(moduleType);

            if (module != null)
            {
                return module;
            }

            // If not registered, use ActivatorUtilities which supports constructor injection
            module = (IModule?)ActivatorUtilities.CreateInstance(scope.ServiceProvider, moduleType);

            if (module == null)
            {
                throw new InvalidOperationException($"Failed to create instance of module type {moduleType.Name}");
            }

            return module;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error creating module instance for {moduleType.Name}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Create a module instance using DI
    /// </summary>
    public TModule CreateModule<TModule>()
        where TModule : IModule
    {
        return (TModule)CreateModule(typeof(TModule));
    }
}

/// <summary>
/// Extension methods for module factory registration
/// </summary>
public static class ModuleFactoryExtensions
{
    /// <summary>
    /// Register module factory in the service collection
    /// </summary>
    public static IServiceCollection AddModuleFactory(this IServiceCollection services)
    {
        services.AddSingleton<IModuleFactory, ModuleFactory>();
        return services;
    }

    /// <summary>
    /// Register a module type in the service collection
    /// </summary>
    public static IServiceCollection AddModule<TModule>(this IServiceCollection services)
        where TModule : class, IModule
    {
        services.AddScoped<TModule>();
        return services;
    }

    /// <summary>
    /// Register a module type with specific lifetime
    /// </summary>
    public static IServiceCollection AddModule<TModule>(
        this IServiceCollection services,
        ServiceLifetime lifetime)
        where TModule : class, IModule
    {
        services.Add(new ServiceDescriptor(typeof(TModule), typeof(TModule), lifetime));
        return services;
    }
}
