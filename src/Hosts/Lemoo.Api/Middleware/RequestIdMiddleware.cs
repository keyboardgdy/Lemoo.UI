using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Lemoo.Api.Middleware;

/// <summary>
/// 请求ID追踪中间件 - 为每个请求生成唯一ID
/// </summary>
public class RequestIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string RequestIdHeader = "X-Request-ID";
    private const string RequestIdItemKey = "RequestId";
    
    public RequestIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // 尝试从请求头获取请求ID，如果没有则生成新的
        var requestId = context.Request.Headers[RequestIdHeader].FirstOrDefault();
        
        if (string.IsNullOrEmpty(requestId))
        {
            requestId = Activity.Current?.Id ?? Guid.NewGuid().ToString("N");
        }
        
        // 将请求ID存储到HttpContext.Items中，供后续使用
        context.Items[RequestIdItemKey] = requestId;
        
        // 将请求ID添加到响应头
        context.Response.Headers[RequestIdHeader] = requestId;
        
        // 将请求ID添加到日志作用域
        using (context.RequestServices
            .GetRequiredService<ILogger<RequestIdMiddleware>>()
            .BeginScope(new Dictionary<string, object> { { "RequestId", requestId } }))
        {
            await _next(context);
        }
    }
    
    /// <summary>
    /// 从HttpContext获取请求ID
    /// </summary>
    public static string? GetRequestId(HttpContext context)
    {
        return context.Items[RequestIdItemKey] as string;
    }
}

