using System.Net;
using System.Text.Json;
using Lemoo.Core.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Lemoo.Api.Middleware;

/// <summary>
/// 全局异常处理中间件
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    
    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "未处理的异常: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var message = "发生内部服务器错误";
        var details = (string?)null;
        
        switch (exception)
        {
            case ValidationException validationEx:
                code = HttpStatusCode.BadRequest;
                message = "验证失败";
                details = JsonSerializer.Serialize(validationEx.Errors);
                break;
                
            case NotFoundException:
                code = HttpStatusCode.NotFound;
                message = exception.Message;
                break;
                
            case UnauthorizedException:
                code = HttpStatusCode.Unauthorized;
                message = exception.Message;
                break;
                
            case ModuleException moduleEx:
                code = HttpStatusCode.BadRequest;
                message = $"模块错误 [{moduleEx.ModuleName}]: {moduleEx.Message}";
                break;
                
            case BusinessException businessEx:
                code = HttpStatusCode.BadRequest;
                message = businessEx.Message;
                if (!string.IsNullOrEmpty(businessEx.ErrorCode))
                {
                    details = businessEx.ErrorCode;
                }
                break;
        }
        
        var response = new
        {
            error = new
            {
                message,
                code = code.ToString(),
                details,
                timestamp = DateTime.UtcNow
            }
        };
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        return context.Response.WriteAsync(json);
    }
}

