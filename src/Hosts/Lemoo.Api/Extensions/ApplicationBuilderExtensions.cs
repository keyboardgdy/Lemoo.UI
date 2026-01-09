using Lemoo.Api.Middleware;

namespace Lemoo.Api.Extensions;

/// <summary>
/// 应用程序构建器扩展方法
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// 使用请求ID追踪
    /// </summary>
    public static IApplicationBuilder UseRequestId(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestIdMiddleware>();
    }
    
    /// <summary>
    /// 使用全局异常处理
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}

