using System.Diagnostics;
using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Application.Metrics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lemoo.Core.Application.Behaviors;

/// <summary>
/// 日志管道行为
/// </summary>
/// <typeparam name="TRequest">请求类型</typeparam>
/// <typeparam name="TResponse">返回类型</typeparam>
public class LoggingBehavior<TRequest, TResponse> : MediatR.IPipelineBehavior<TRequest, TResponse>
    where TRequest : MediatR.IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly PerformanceMetrics? _performanceMetrics;
    
    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        PerformanceMetrics? performanceMetrics = null)
    {
        _logger = logger;
        _performanceMetrics = performanceMetrics;
    }
    
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid().ToString("N")[..8]; // 生成短请求ID
        
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["RequestId"] = requestId,
            ["RequestName"] = requestName
        }))
        {
            _logger.LogInformation("处理请求: {RequestName}, RequestId: {RequestId}", requestName, requestId);
            
            _performanceMetrics?.RecordRequestStart(requestName);
            
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var response = await next();
                stopwatch.Stop();
                
                _logger.LogInformation(
                    "请求处理完成: {RequestName}, RequestId: {RequestId}，耗时: {ElapsedMilliseconds}ms",
                    requestName,
                    requestId,
                    stopwatch.ElapsedMilliseconds);
                
                _performanceMetrics?.RecordRequestEnd(requestName, stopwatch.Elapsed, success: true);
                    
                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(
                    ex,
                    "请求处理失败: {RequestName}, RequestId: {RequestId}，耗时: {ElapsedMilliseconds}ms",
                    requestName,
                    requestId,
                    stopwatch.ElapsedMilliseconds);
                
                _performanceMetrics?.RecordRequestEnd(requestName, stopwatch.Elapsed, success: false);
                
                throw;
            }
        }
    }
}

