using System.Diagnostics;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Lemoo.Core.Application.Metrics;

/// <summary>
/// 性能指标收集器
/// </summary>
public class PerformanceMetrics
{
    private readonly ILogger<PerformanceMetrics> _logger;
    private readonly ConcurrentDictionary<string, RequestMetrics> _metrics = new();
    
    public PerformanceMetrics(ILogger<PerformanceMetrics> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// 记录请求开始
    /// </summary>
    public void RecordRequestStart(string requestName)
    {
        var metrics = _metrics.GetOrAdd(requestName, _ => new RequestMetrics());
        Interlocked.Increment(ref metrics._totalRequests);
        metrics.LastRequestTime = DateTime.UtcNow;
    }
    
    /// <summary>
    /// 记录请求完成
    /// </summary>
    public void RecordRequestEnd(string requestName, TimeSpan duration, bool success = true)
    {
        if (!_metrics.TryGetValue(requestName, out var metrics))
            return;
        
        if (success)
        {
            Interlocked.Increment(ref metrics._successfulRequests);
        }
        else
        {
            Interlocked.Increment(ref metrics._failedRequests);
        }
        
        // 更新平均响应时间（线程安全）
        var newDuration = (long)duration.TotalMilliseconds;
        var totalDuration = Interlocked.Add(ref metrics._totalDurationMs, newDuration);
        var totalRequests = metrics.TotalRequests;
        if (totalRequests > 0)
        {
            Interlocked.Exchange(ref metrics._averageResponseTimeMs, totalDuration / totalRequests);
        }
        
        // 更新最大响应时间（线程安全）
        long currentMax;
        do
        {
            currentMax = Interlocked.Read(ref metrics._maxResponseTimeMs);
            if (newDuration <= currentMax)
                break;
        }
        while (Interlocked.CompareExchange(ref metrics._maxResponseTimeMs, newDuration, currentMax) != currentMax);
        
        // 更新最小响应时间（线程安全）
        long currentMin;
        do
        {
            currentMin = Interlocked.Read(ref metrics._minResponseTimeMs);
            if (currentMin != 0 && newDuration >= currentMin)
                break;
            if (currentMin == 0 || newDuration < currentMin)
            {
                if (Interlocked.CompareExchange(ref metrics._minResponseTimeMs, newDuration, currentMin) == currentMin)
                    break;
            }
        }
        while (true);
    }
    
    /// <summary>
    /// 获取所有指标
    /// </summary>
    public IReadOnlyDictionary<string, RequestMetrics> GetAllMetrics()
    {
        return _metrics.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Clone());
    }
    
    /// <summary>
    /// 获取指定请求的指标
    /// </summary>
    public RequestMetrics? GetMetrics(string requestName)
    {
        return _metrics.TryGetValue(requestName, out var metrics) ? metrics.Clone() : null;
    }
    
    /// <summary>
    /// 重置所有指标
    /// </summary>
    public void Reset()
    {
        _metrics.Clear();
    }
}

/// <summary>
/// 请求指标（线程安全）
/// </summary>
public class RequestMetrics
{
    internal int _totalRequests;
    internal int _successfulRequests;
    internal int _failedRequests;
    internal long _totalDurationMs;
    internal long _averageResponseTimeMs;
    internal long _maxResponseTimeMs;
    internal long _minResponseTimeMs;
    private DateTime _lastRequestTime;
    
    public int TotalRequests 
    { 
        get => Interlocked.CompareExchange(ref _totalRequests, 0, 0); 
        set => Interlocked.Exchange(ref _totalRequests, value); 
    }
    
    public int SuccessfulRequests 
    { 
        get => Interlocked.CompareExchange(ref _successfulRequests, 0, 0); 
        set => Interlocked.Exchange(ref _successfulRequests, value); 
    }
    
    public int FailedRequests 
    { 
        get => Interlocked.CompareExchange(ref _failedRequests, 0, 0); 
        set => Interlocked.Exchange(ref _failedRequests, value); 
    }
    
    public long TotalDurationMs 
    { 
        get => Interlocked.Read(ref _totalDurationMs); 
        set => Interlocked.Exchange(ref _totalDurationMs, value); 
    }
    
    public long AverageResponseTimeMs 
    { 
        get => Interlocked.Read(ref _averageResponseTimeMs); 
        set => Interlocked.Exchange(ref _averageResponseTimeMs, value); 
    }
    
    public long MaxResponseTimeMs 
    { 
        get => Interlocked.Read(ref _maxResponseTimeMs); 
        set => Interlocked.Exchange(ref _maxResponseTimeMs, value); 
    }
    
    public long MinResponseTimeMs 
    { 
        get => Interlocked.Read(ref _minResponseTimeMs); 
        set => Interlocked.Exchange(ref _minResponseTimeMs, value); 
    }
    
    public DateTime LastRequestTime 
    { 
        get => _lastRequestTime; 
        set => _lastRequestTime = value; 
    }
    
    public RequestMetrics Clone()
    {
        return new RequestMetrics
        {
            TotalRequests = TotalRequests,
            SuccessfulRequests = SuccessfulRequests,
            FailedRequests = FailedRequests,
            TotalDurationMs = TotalDurationMs,
            AverageResponseTimeMs = AverageResponseTimeMs,
            MaxResponseTimeMs = MaxResponseTimeMs,
            MinResponseTimeMs = MinResponseTimeMs,
            LastRequestTime = LastRequestTime
        };
    }
}

