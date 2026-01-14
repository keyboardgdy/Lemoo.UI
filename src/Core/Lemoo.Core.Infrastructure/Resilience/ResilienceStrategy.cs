using Lemoo.Core.Abstractions.Logging;
using Lemoo.Core.Abstractions.Resilience;
using Microsoft.Extensions.DependencyInjection;

namespace Lemoo.Core.Infrastructure.Resilience;

/// <summary>
/// Combined resilience strategy with retry and circuit breaker
/// </summary>
public class ResilienceStrategy : IResilienceStrategy
{
    private readonly IRetryPolicy _retryPolicy;
    private readonly ICircuitBreaker _circuitBreaker;
    private readonly ILoggerService _logger;

    public ResilienceStrategy(
        IRetryPolicy retryPolicy,
        ICircuitBreaker circuitBreaker,
        ILoggerService logger)
    {
        _retryPolicy = retryPolicy ?? throw new ArgumentNullException(nameof(retryPolicy));
        _circuitBreaker = circuitBreaker ?? throw new ArgumentNullException(nameof(circuitBreaker));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<T> ExecuteAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        return await _circuitBreaker.ExecuteAsync(async () =>
        {
            return await _retryPolicy.ExecuteAsync(action, cancellationToken);
        }, cancellationToken);
    }

    public async Task ExecuteAsync(
        Func<Task> action,
        CancellationToken cancellationToken = default)
    {
        await _circuitBreaker.ExecuteAsync(async () =>
        {
            await _retryPolicy.ExecuteAsync(action, cancellationToken);
        }, cancellationToken);
    }
}

/// <summary>
/// Circuit breaker implementation
/// </summary>
public class CircuitBreaker : ICircuitBreaker
{
    private readonly ResilienceStrategyOptions _options;
    private readonly ILoggerService _logger;
    private readonly object _lock = new();
    private DateTime _circuitOpenedTimestamp;

    public CircuitState State { get; private set; } = CircuitState.Closed;
    public int ExceptionCount { get; private set; }
    public DateTime? LastExceptionTimestamp { get; private set; }

    public CircuitBreaker(ResilienceStrategyOptions options, ILoggerService logger)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<T> ExecuteAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            // Check if circuit should transition from Open to HalfOpen
            if (State == CircuitState.Open)
            {
                var timeSinceOpened = DateTime.UtcNow - _circuitOpenedTimestamp;
                if (timeSinceOpened.TotalMilliseconds >= _options.CircuitBreakerDurationMs)
                {
                    _logger.LogInformation("Circuit breaker transitioning to HalfOpen state");
                    State = CircuitState.HalfOpen;
                }
                else
                {
                    throw new CircuitBreakerOpenException(
                        $"Circuit breaker is open. Try again after {(_options.CircuitBreakerDurationMs - timeSinceOpened.TotalMilliseconds):F0}ms");
                }
            }
        }

        try
        {
            var result = await action();

            // Success - reset exception count and close circuit if half-open
            lock (_lock)
            {
                if (State == CircuitState.HalfOpen)
                {
                    _logger.LogInformation("Circuit breaker closing after successful request");
                    State = CircuitState.Closed;
                    ExceptionCount = 0;
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            HandleException(ex);
            throw;
        }
    }

    private void HandleException(Exception exception)
    {
        lock (_lock)
        {
            LastExceptionTimestamp = DateTime.UtcNow;
            ExceptionCount++;

            _logger.LogWarning(
                "Circuit breaker exception count: {Count}, State: {State}, Exception: {Exception}",
                ExceptionCount,
                State,
                exception.Message);

            // Check if we should open the circuit
            if (ExceptionCount >= _options.CircuitBreakerThreshold)
            {
                _logger.LogError("Circuit breaker opening after {Count} exceptions", ExceptionCount);
                State = CircuitState.Open;
                _circuitOpenedTimestamp = DateTime.UtcNow;
            }
        }
    }

    public void Reset()
    {
        lock (_lock)
        {
            _logger.LogInformation("Circuit breaker manually reset");
            State = CircuitState.Closed;
            ExceptionCount = 0;
            LastExceptionTimestamp = null;
        }
    }
}

/// <summary>
/// Retry policy implementation with exponential backoff
/// </summary>
public class RetryPolicy : IRetryPolicy
{
    private readonly ResilienceStrategyOptions _options;
    private readonly ILoggerService _logger;
    private readonly Random _random = new();

    public RetryPolicy(ResilienceStrategyOptions options, ILoggerService logger)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<T> ExecuteAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        var attempt = 0;
        var delay = _options.InitialDelayMs;

        while (true)
        {
            attempt++;

            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                if (attempt >= _options.MaxRetries || !IsRetryable(ex))
                {
                    _logger.LogError(
                        "Operation failed after {Attempt} attempts: {Exception}",
                        attempt,
                        ex.Message);
                    throw;
                }

                // Add jitter to delay to prevent thundering herd
                var jitter = _random.Next(0, (int)(delay * 0.1));
                var actualDelay = delay + jitter;

                _logger.LogWarning(
                    "Retry {Attempt}/{MaxRetries} after {Delay}ms: {Exception}",
                    attempt,
                    _options.MaxRetries,
                    actualDelay,
                    ex.Message);

                await Task.Delay(actualDelay, cancellationToken);

                // Calculate next delay with exponential backoff
                if (_options.UseExponentialBackoff)
                {
                    delay = Math.Min(delay * 2, _options.MaxDelayMs);
                }
            }
        }
    }

    public async Task ExecuteAsync(
        Func<Task> action,
        CancellationToken cancellationToken = default)
    {
        await ExecuteAsync(async () =>
        {
            await action();
            return true;
        }, cancellationToken);
    }

    private bool IsRetryable(Exception exception)
    {
        // Check if exception type is in retryable list
        if (_options.RetryableExceptions.Length > 0)
        {
            return _options.RetryableExceptions.Any(t => t.IsInstanceOfType(exception));
        }

        // Default retryable exceptions
        return exception is TimeoutException ||
               exception is HttpRequestException ||
               (exception is InvalidOperationException && exception.Message.Contains("connection", StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>
/// Exception thrown when circuit breaker is open
/// </summary>
public class CircuitBreakerOpenException : Exception
{
    public CircuitBreakerOpenException(string message) : base(message) { }
}

/// <summary>
/// Extension methods for resilience strategy registration
/// </summary>
public static class ResilienceExtensions
{
    /// <summary>
    /// Add resilience strategy with default options
    /// </summary>
    public static IServiceCollection AddResilienceStrategy(
        this IServiceCollection services,
        Action<ResilienceStrategyOptions>? configure = null)
    {
        var options = new ResilienceStrategyOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<ICircuitBreaker, CircuitBreaker>();
        services.AddSingleton<IRetryPolicy, RetryPolicy>();
        services.AddSingleton<IResilienceStrategy, ResilienceStrategy>();

        return services;
    }
}
