namespace Lemoo.Core.Abstractions.Resilience;

/// <summary>
/// Resilience strategy interface for retry and circuit breaker patterns
/// </summary>
public interface IResilienceStrategy
{
    /// <summary>
    /// Execute an action with retry and circuit breaker protection
    /// </summary>
    Task<T> ExecuteAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute an action with retry and circuit breaker protection
    /// </summary>
    Task ExecuteAsync(
        Func<Task> action,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Resilience strategy options
/// </summary>
public class ResilienceStrategyOptions
{
    /// <summary>
    /// Maximum number of retry attempts
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Initial delay between retries in milliseconds
    /// </summary>
    public int InitialDelayMs { get; set; } = 1000;

    /// <summary>
    /// Maximum delay between retries in milliseconds
    /// </summary>
    public int MaxDelayMs { get; set; } = 10000;

    /// <summary>
    /// Whether to use exponential backoff
    /// </summary>
    public bool UseExponentialBackoff { get; set; } = true;

    /// <summary>
    /// Number of exceptions before opening the circuit
    /// </summary>
    public int CircuitBreakerThreshold { get; set; } = 5;

    /// <summary>
    /// Duration to keep circuit open in milliseconds
    /// </summary>
    public int CircuitBreakerDurationMs { get; set; } = 30000;

    /// <summary>
    /// Exceptions that should trigger a retry
    /// </summary>
    public Type[] RetryableExceptions { get; set; } = Array.Empty<Type>();

    /// <summary>
    /// Exceptions that should open the circuit breaker
    /// </summary>
    public Type[] CircuitBreakerExceptions { get; set; } = Array.Empty<Type>();
}

/// <summary>
/// Circuit breaker state
/// </summary>
public enum CircuitState
{
    /// <summary>
    /// Circuit is closed, requests are allowed
    /// </summary>
    Closed,

    /// <summary>
    /// Circuit is open, requests are blocked
    /// </summary>
    Open,

    /// <summary>
    /// Circuit is half-open, testing if service has recovered
    /// </summary>
    HalfOpen
}

/// <summary>
/// Circuit breaker interface
/// </summary>
public interface ICircuitBreaker
{
    /// <summary>
    /// Current circuit state
    /// </summary>
    CircuitState State { get; }

    /// <summary>
    /// Exception count in current window
    /// </summary>
    int ExceptionCount { get; }

    /// <summary>
    /// Last exception timestamp
    /// </summary>
    DateTime? LastExceptionTimestamp { get; }

    /// <summary>
    /// Execute action with circuit breaker protection
    /// </summary>
    Task<T> ExecuteAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reset the circuit breaker to closed state
    /// </summary>
    void Reset();
}

/// <summary>
/// Retry policy interface
/// </summary>
public interface IRetryPolicy
{
    /// <summary>
    /// Execute action with retry protection
    /// </summary>
    Task<T> ExecuteAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute action with retry protection
    /// </summary>
    Task ExecuteAsync(
        Func<Task> action,
        CancellationToken cancellationToken = default);
}
