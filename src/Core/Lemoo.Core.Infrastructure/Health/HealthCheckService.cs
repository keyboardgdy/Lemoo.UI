using Lemoo.Core.Abstractions.Health;
using Lemoo.Core.Abstractions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Lemoo.Core.Infrastructure.Health;

/// <summary>
/// Health check service implementation
/// </summary>
public class HealthCheckService : IHealthCheckService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerService _logger;
    private readonly Dictionary<string, Func<Task<HealthCheckResult>>> _healthChecks = new();

    public HealthCheckService(IServiceProvider serviceProvider, ILoggerService logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Health check name cannot be empty", nameof(name));
        }

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            // Try to get registered health check function
            if (_healthChecks.TryGetValue(name, out var checkFunc))
            {
                var result = await checkFunc();
                result.Duration = stopwatch.Elapsed;
                return result;
            }

            // Try to resolve IHealthCheck implementation
            using var scope = _serviceProvider.CreateScope();
            var healthChecks = scope.ServiceProvider.GetServices<IHealthCheck>();
            var healthCheck = healthChecks.FirstOrDefault(h => h.Name == name);

            if (healthCheck != null)
            {
                var result = await healthCheck.CheckHealthAsync(cancellationToken);
                result.Duration = stopwatch.Elapsed;
                return result;
            }

            return new HealthCheckResult
            {
                Name = name,
                Status = HealthStatus.Unhealthy,
                Description = "Health check not found"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing health check: {Name}", name);

            return new HealthCheckResult
            {
                Name = name,
                Status = HealthStatus.Unhealthy,
                Description = ex.Message,
                Duration = stopwatch.Elapsed
            };
        }
    }

    public async Task<Dictionary<string, HealthCheckResult>> CheckAllHealthAsync(
        CancellationToken cancellationToken = default)
    {
        var results = new Dictionary<string, HealthCheckResult>();
        var tasks = new List<Task<KeyValuePair<string, HealthCheckResult>>>();

        // Get all registered health check names
        var names = _healthChecks.Keys.ToList();

        // Also get IHealthCheck implementations
        using var scope = _serviceProvider.CreateScope();
        var healthChecks = scope.ServiceProvider.GetServices<IHealthCheck>();
        names.AddRange(healthChecks.Select(h => h.Name));

        foreach (var name in names.Distinct())
        {
            tasks.Add(Task.Run(async () =>
            {
                var result = await CheckHealthAsync(name, cancellationToken);
                return new KeyValuePair<string, HealthCheckResult>(name, result);
            }, cancellationToken));
        }

        var resultsList = await Task.WhenAll(tasks);
        foreach (var kvp in resultsList)
        {
            results[kvp.Key] = kvp.Value;
        }

        return results;
    }

    public void RegisterHealthCheck(string name, Func<Task<HealthCheckResult>> check)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Health check name cannot be empty", nameof(name));
        }

        _healthChecks[name] = check ?? throw new ArgumentNullException(nameof(check));
        _logger.LogInformation("Registered health check: {Name}", name);
    }
}

/// <summary>
/// Base class for implementing health checks
/// </summary>
public abstract class HealthCheckBase : IHealthCheck
{
    public abstract string Name { get; }

    public abstract Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a healthy result
    /// </summary>
    protected HealthCheckResult Healthy(string? description = null, Dictionary<string, object>? data = null)
    {
        return new HealthCheckResult
        {
            Name = Name,
            Status = HealthStatus.Healthy,
            Description = description ?? "Service is healthy",
            Data = data ?? new Dictionary<string, object>()
        };
    }

    /// <summary>
    /// Create a degraded result
    /// </summary>
    protected HealthCheckResult Degraded(string description, Dictionary<string, object>? data = null)
    {
        return new HealthCheckResult
        {
            Name = Name,
            Status = HealthStatus.Degraded,
            Description = description,
            Data = data ?? new Dictionary<string, object>()
        };
    }

    /// <summary>
    /// Create an unhealthy result
    /// </summary>
    protected HealthCheckResult Unhealthy(string description, Dictionary<string, object>? data = null)
    {
        return new HealthCheckResult
        {
            Name = Name,
            Status = HealthStatus.Unhealthy,
            Description = description,
            Data = data ?? new Dictionary<string, object>()
        };
    }
}

/// <summary>
/// Database health check
/// </summary>
public class DatabaseHealthCheck : HealthCheckBase
{
    private readonly Func<Task<bool>> _checkConnection;

    public override string Name => "Database";

    public DatabaseHealthCheck(Func<Task<bool>> checkConnection)
    {
        _checkConnection = checkConnection ?? throw new ArgumentNullException(nameof(checkConnection));
    }

    public override async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var isConnected = await _checkConnection();

            return isConnected
                ? Healthy("Database connection is healthy")
                : Degraded("Database connection is slow");
        }
        catch (Exception ex)
        {
            return Unhealthy($"Database connection failed: {ex.Message}");
        }
    }
}

/// <summary>
/// Extension methods for health check registration
/// </summary>
public static class HealthCheckExtensions
{
    /// <summary>
    /// Add health check service
    /// </summary>
    public static IServiceCollection AddHealthCheckService(this IServiceCollection services)
    {
        services.AddSingleton<IHealthCheckService, HealthCheckService>();
        return services;
    }

    /// <summary>
    /// Register a health check from its implementation
    /// </summary>
    public static IServiceCollection AddHealthCheck<T>(this IServiceCollection services)
        where T : class, IHealthCheck
    {
        services.AddSingleton<IHealthCheck, T>();
        return services;
    }
}
