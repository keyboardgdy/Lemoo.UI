using Lemoo.Core.Application.Metrics;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo.Api.Controllers;

/// <summary>
/// 性能指标控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    private readonly PerformanceMetrics _metrics;
    
    public MetricsController(PerformanceMetrics metrics)
    {
        _metrics = metrics;
    }
    
    /// <summary>
    /// 获取所有性能指标
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Dictionary<string, RequestMetrics>), StatusCodes.Status200OK)]
    public IActionResult GetAllMetrics()
    {
        var allMetrics = _metrics.GetAllMetrics();
        return Ok(new
        {
            timestamp = DateTime.UtcNow,
            metrics = allMetrics.Select(kvp => new
            {
                requestName = kvp.Key,
                totalRequests = kvp.Value.TotalRequests,
                successfulRequests = kvp.Value.SuccessfulRequests,
                failedRequests = kvp.Value.FailedRequests,
                averageResponseTimeMs = kvp.Value.AverageResponseTimeMs,
                maxResponseTimeMs = kvp.Value.MaxResponseTimeMs,
                minResponseTimeMs = kvp.Value.MinResponseTimeMs,
                lastRequestTime = kvp.Value.LastRequestTime
            })
        });
    }
    
    /// <summary>
    /// 获取指定请求的性能指标
    /// </summary>
    [HttpGet("{requestName}")]
    [ProducesResponseType(typeof(RequestMetrics), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetMetrics(string requestName)
    {
        var metrics = _metrics.GetMetrics(requestName);
        if (metrics == null)
        {
            return NotFound(new { message = $"未找到请求 '{requestName}' 的性能指标" });
        }
        
        return Ok(new
        {
            requestName,
            timestamp = DateTime.UtcNow,
            totalRequests = metrics.TotalRequests,
            successfulRequests = metrics.SuccessfulRequests,
            failedRequests = metrics.FailedRequests,
            averageResponseTimeMs = metrics.AverageResponseTimeMs,
            maxResponseTimeMs = metrics.MaxResponseTimeMs,
            minResponseTimeMs = metrics.MinResponseTimeMs,
            lastRequestTime = metrics.LastRequestTime
        });
    }
    
    /// <summary>
    /// 重置所有性能指标
    /// </summary>
    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ResetMetrics()
    {
        _metrics.Reset();
        return Ok(new { message = "性能指标已重置", timestamp = DateTime.UtcNow });
    }
}

