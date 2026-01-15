using FluentAssertions;
using Lemoo.Core.Abstractions.Logging;
using Lemoo.Core.Abstractions.Resilience;
using Lemoo.Core.Infrastructure.Resilience;
using Moq;
using Xunit;

namespace Lemoo.Core.Infrastructure.Tests.Resilience;

public class CircuitBreakerTests
{
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly ResilienceStrategyOptions _options;
    private readonly CircuitBreaker _circuitBreaker;

    public CircuitBreakerTests()
    {
        _loggerMock = new Mock<ILoggerService>();
        _options = new ResilienceStrategyOptions
        {
            CircuitBreakerThreshold = 3,
            CircuitBreakerDurationMs = 100
        };
        _circuitBreaker = new CircuitBreaker(_options, _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSucceeds_ShouldReturnResult()
    {
        // Arrange
        var expectedResult = "success";
        var action = () => Task.FromResult(expectedResult);

        // Act
        var result = await _circuitBreaker.ExecuteAsync(action);

        // Assert
        result.Should().Be(expectedResult);
        _circuitBreaker.State.Should().Be(CircuitState.Closed);
        _circuitBreaker.ExceptionCount.Should().Be(0);
    }

    [Fact]
    public async Task ExecuteAsync_WhenFails_ShouldIncrementExceptionCount()
    {
        // Arrange
        Func<Task<string>> action = () => throw new InvalidOperationException("Test exception");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _circuitBreaker.ExecuteAsync(action));
        _circuitBreaker.ExceptionCount.Should().Be(1);
        _circuitBreaker.State.Should().Be(CircuitState.Closed);
    }

    [Fact]
    public async Task ExecuteAsync_WhenThresholdReached_ShouldOpenCircuit()
    {
        // Arrange
        Func<Task<string>> action = () => throw new InvalidOperationException("Test exception");

        // Act & Assert - First two exceptions
        await Assert.ThrowsAsync<InvalidOperationException>(() => _circuitBreaker.ExecuteAsync(action));
        await Assert.ThrowsAsync<InvalidOperationException>(() => _circuitBreaker.ExecuteAsync(action));
        _circuitBreaker.State.Should().Be(CircuitState.Closed);

        // Third exception opens the circuit
        await Assert.ThrowsAsync<InvalidOperationException>(() => _circuitBreaker.ExecuteAsync(action));
        _circuitBreaker.State.Should().Be(CircuitState.Open);
        _circuitBreaker.ExceptionCount.Should().Be(3);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCircuitIsOpen_ShouldThrowCircuitBreakerOpenException()
    {
        // Arrange - Open the circuit
        Func<Task<string>> failingAction = () => throw new InvalidOperationException("Test exception");
        for (int i = 0; i < 3; i++)
        {
            try { await _circuitBreaker.ExecuteAsync(failingAction); } catch { }
        }

        // Act & Assert
        await Assert.ThrowsAsync<CircuitBreakerOpenException>(() => _circuitBreaker.ExecuteAsync(() => Task.FromResult("result")));
    }

    [Fact]
    public async Task ExecuteAsync_WhenCircuitDurationPasses_ShouldTransitionToHalfOpen()
    {
        // Arrange - Open the circuit
        Func<Task<string>> failingAction = () => throw new InvalidOperationException("Test exception");
        for (int i = 0; i < 3; i++)
        {
            try { await _circuitBreaker.ExecuteAsync(failingAction); } catch { }
        }
        _circuitBreaker.State.Should().Be(CircuitState.Open);

        // Act - Wait for circuit duration
        await Task.Delay(150);
        var result = await _circuitBreaker.ExecuteAsync(() => Task.FromResult("success"));

        // Assert
        result.Should().Be("success");
        _circuitBreaker.State.Should().Be(CircuitState.Closed);
    }

    [Fact]
    public async Task ExecuteAsync_WhenHalfOpenAndSucceeds_ShouldCloseCircuit()
    {
        // Arrange - Open the circuit
        Func<Task<string>> failingAction = () => throw new InvalidOperationException("Test exception");
        for (int i = 0; i < 3; i++)
        {
            try { await _circuitBreaker.ExecuteAsync(failingAction); } catch { }
        }

        // Act - Wait for circuit duration and succeed
        await Task.Delay(150);
        var result = await _circuitBreaker.ExecuteAsync(() => Task.FromResult("success"));

        // Assert
        result.Should().Be("success");
        _circuitBreaker.State.Should().Be(CircuitState.Closed);
        _circuitBreaker.ExceptionCount.Should().Be(0);
    }

    [Fact]
    public async Task ExecuteAsync_WhenHalfOpenAndFails_ShouldReopenCircuit()
    {
        // Arrange - Open the circuit
        Func<Task<string>> failingAction = () => throw new InvalidOperationException("Test exception");
        for (int i = 0; i < 3; i++)
        {
            try { await _circuitBreaker.ExecuteAsync(failingAction); } catch { }
        }

        // Act - Wait for circuit duration and fail again
        await Task.Delay(150);
        try { await _circuitBreaker.ExecuteAsync(failingAction); } catch { }

        // Assert
        _circuitBreaker.State.Should().Be(CircuitState.Open);
        _circuitBreaker.ExceptionCount.Should().Be(4);
    }

    [Fact]
    public void Reset_ShouldCloseCircuitAndResetExceptionCount()
    {
        // Arrange - Open the circuit
        Func<Task<string>> failingAction = () => throw new InvalidOperationException("Test exception");
        for (int i = 0; i < 3; i++)
        {
            try { _circuitBreaker.ExecuteAsync(failingAction).Wait(); } catch { }
        }
        _circuitBreaker.State.Should().Be(CircuitState.Open);

        // Act
        _circuitBreaker.Reset();

        // Assert
        _circuitBreaker.State.Should().Be(CircuitState.Closed);
        _circuitBreaker.ExceptionCount.Should().Be(0);
        _circuitBreaker.LastExceptionTimestamp.Should().BeNull();
    }

    [Fact]
    public void State_ShouldTrackCircuitState()
    {
        // Assert
        _circuitBreaker.State.Should().Be(CircuitState.Closed);
    }
}

public class RetryPolicyTests
{
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly ResilienceStrategyOptions _options;
    private readonly RetryPolicy _retryPolicy;
    private int _attemptCount;

    public RetryPolicyTests()
    {
        _loggerMock = new Mock<ILoggerService>();
        _options = new ResilienceStrategyOptions
        {
            MaxRetries = 3,
            InitialDelayMs = 10,
            MaxDelayMs = 100,
            UseExponentialBackoff = true
        };
        _retryPolicy = new RetryPolicy(_options, _loggerMock.Object);
        _attemptCount = 0;
    }

    [Fact]
    public async Task ExecuteAsync_WhenSucceeds_ShouldReturnResult()
    {
        // Arrange
        var expectedResult = "success";
        var action = () => Task.FromResult(expectedResult);

        // Act
        var result = await _retryPolicy.ExecuteAsync(action);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task ExecuteAsync_WhenFailsWithRetryableException_ShouldRetry()
    {
        // Arrange
        _attemptCount = 0;
        var action = () =>
        {
            _attemptCount++;
            if (_attemptCount < 3)
            {
                throw new TimeoutException("Test timeout");
            }
            return Task.FromResult("success");
        };

        // Act
        var result = await _retryPolicy.ExecuteAsync(action);

        // Assert
        result.Should().Be("success");
        _attemptCount.Should().Be(3);
    }

    [Fact]
    public async Task ExecuteAsync_WhenExceedsMaxRetries_ShouldThrowException()
    {
        // Arrange
        Func<Task<string>> action = () => throw new TimeoutException("Test timeout");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<TimeoutException>(() => _retryPolicy.ExecuteAsync(action));
        exception.Message.Should().Be("Test timeout");
    }

    [Fact]
    public async Task ExecuteAsync_WhenFailsWithNonRetryableException_ShouldNotRetry()
    {
        // Arrange
        _attemptCount = 0;
        Func<Task<string>> action = () =>
        {
            _attemptCount++;
            throw new ArgumentException("Test argument exception");
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _retryPolicy.ExecuteAsync(action));
        _attemptCount.Should().Be(1);
    }

    [Fact]
    public async Task ExecuteAsync_WithCustomRetryableExceptions_ShouldRetryOnlySpecified()
    {
        // Arrange
        _options.RetryableExceptions = new[] { typeof(InvalidOperationException) };
        var customRetryPolicy = new RetryPolicy(_options, _loggerMock.Object);
        _attemptCount = 0;

        Func<Task<string>> invalidOperationExceptionAction = () =>
        {
            _attemptCount++;
            if (_attemptCount < 2)
            {
                throw new InvalidOperationException("Test invalid operation");
            }
            return Task.FromResult("success");
        };

        Func<Task<string>> timeoutExceptionAction = () =>
        {
            throw new TimeoutException("Test timeout");
        };

        // Act & Assert - InvalidOperationException should retry
        var result = await customRetryPolicy.ExecuteAsync(invalidOperationExceptionAction);
        result.Should().Be("success");

        // Reset and test TimeoutException - should not retry
        _attemptCount = 0;
        await Assert.ThrowsAsync<TimeoutException>(() => customRetryPolicy.ExecuteAsync(timeoutExceptionAction));
    }

    [Fact]
    public async Task ExecuteAsync_WithExponentialBackoff_ShouldIncreaseDelay()
    {
        // Arrange
        _options.InitialDelayMs = 10;
        _options.MaxRetries = 3;
        var retryPolicy = new RetryPolicy(_options, _loggerMock.Object);
        _attemptCount = 0;
        var delays = new List<long>();

        var action = () =>
        {
            _attemptCount++;
            if (_attemptCount <= 2)
            {
                throw new TimeoutException("Test timeout");
            }
            return Task.FromResult("success");
        };

        // Act
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await retryPolicy.ExecuteAsync(action);
        sw.Stop();

        // Assert - With exponential backoff, total time should be at least initial delay + 2x initial delay
        sw.ElapsedMilliseconds.Should().BeGreaterOrEqualTo(25); // 10ms + 20ms (with some tolerance)
    }

    [Fact]
    public async Task ExecuteAsync_WithoutExponentialBackoff_ShouldUseConstantDelay()
    {
        // Arrange
        _options.UseExponentialBackoff = false;
        _options.InitialDelayMs = 20;
        var retryPolicy = new RetryPolicy(_options, _loggerMock.Object);
        _attemptCount = 0;

        var action = () =>
        {
            _attemptCount++;
            if (_attemptCount <= 2)
            {
                throw new TimeoutException("Test timeout");
            }
            return Task.FromResult("success");
        };

        // Act
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await retryPolicy.ExecuteAsync(action);
        sw.Stop();

        // Assert - With constant delay, total time should be around 2 * initial delay
        sw.ElapsedMilliseconds.Should().BeGreaterOrEqualTo(40);
    }
}

public class ResilienceStrategyTests
{
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IRetryPolicy> _retryPolicyMock;
    private readonly Mock<ICircuitBreaker> _circuitBreakerMock;
    private readonly ResilienceStrategy _resilienceStrategy;

    public ResilienceStrategyTests()
    {
        _loggerMock = new Mock<ILoggerService>();
        _retryPolicyMock = new Mock<IRetryPolicy>();
        _circuitBreakerMock = new Mock<ICircuitBreaker>();
        _resilienceStrategy = new ResilienceStrategy(
            _retryPolicyMock.Object,
            _circuitBreakerMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Constructor_WhenRetryPolicyIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ResilienceStrategy(
            null!,
            _circuitBreakerMock.Object,
            _loggerMock.Object));
    }

    [Fact]
    public void Constructor_WhenCircuitBreakerIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ResilienceStrategy(
            _retryPolicyMock.Object,
            null!,
            _loggerMock.Object));
    }

    [Fact]
    public void Constructor_WhenLoggerIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ResilienceStrategy(
            _retryPolicyMock.Object,
            _circuitBreakerMock.Object,
            null!));
    }
}
