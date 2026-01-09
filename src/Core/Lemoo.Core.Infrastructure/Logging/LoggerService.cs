using Lemoo.Core.Abstractions.Logging;
using Microsoft.Extensions.Logging;

namespace Lemoo.Core.Infrastructure.Logging;

/// <summary>
/// 日志服务实现 - 基于 Microsoft.Extensions.Logging
/// </summary>
public class LoggerService : ILoggerService
{
    private readonly ILogger<LoggerService> _logger;

    public LoggerService(ILogger<LoggerService> logger)
    {
        _logger = logger;
    }

    public void LogTrace(string message, params object[] args)
    {
        _logger.LogTrace(message, args);
    }

    public void LogDebug(string message, params object[] args)
    {
        _logger.LogDebug(message, args);
    }

    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        _logger.LogWarning(message, args);
    }

    public void LogError(string message, params object[] args)
    {
        _logger.LogError(message, args);
    }

    public void LogError(Exception exception, string message, params object[] args)
    {
        _logger.LogError(exception, message, args);
    }

    public void LogCritical(string message, params object[] args)
    {
        _logger.LogCritical(message, args);
    }

    public void LogCritical(Exception exception, string message, params object[] args)
    {
        _logger.LogCritical(exception, message, args);
    }
}

