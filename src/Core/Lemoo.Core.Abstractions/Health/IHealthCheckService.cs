namespace Lemoo.Core.Abstractions.Health;

/// <summary>
/// Health status enumeration
/// </summary>
public enum HealthStatus
{
    /// <summary>
    /// Service is healthy
    /// </summary>
    Healthy,

    /// <summary>
    /// Service is degraded but still functional
    /// </summary>
    Degraded,

    /// <summary>
    /// Service is unhealthy
    /// </summary>
    Unhealthy
}

/// <summary>
/// Health check result
/// </summary>
public class HealthCheckResult
{
    public string Name { get; set; } = string.Empty;
    public HealthStatus Status { get; set; }
    public string? Description { get; set; }
    public TimeSpan? Duration { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Health check service interface
/// </summary>
public interface IHealthCheckService
{
    /// <summary>
    /// Check health of a specific component
    /// </summary>
    Task<HealthCheckResult> CheckHealthAsync(
        string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check health of all registered components
    /// </summary>
    Task<Dictionary<string, HealthCheckResult>> CheckAllHealthAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Register a health check
    /// </summary>
    void RegisterHealthCheck(string name, Func<Task<HealthCheckResult>> check);
}

/// <summary>
/// Health check interface for components to implement
/// </summary>
public interface IHealthCheck
{
    /// <summary>
    /// Name of the health check
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Perform the health check
    /// </summary>
    Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default);
}
