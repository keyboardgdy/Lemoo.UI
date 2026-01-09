namespace Lemoo.Core.Application.Common;

/// <summary>
/// API响应类 - 标准化的API响应格式
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// 响应数据
    /// </summary>
    public T? Data { get; set; }
    
    /// <summary>
    /// 消息
    /// </summary>
    public string? Message { get; set; }
    
    /// <summary>
    /// 错误信息（字典格式，键为字段名，值为错误消息数组）
    /// </summary>
    public IReadOnlyDictionary<string, string[]>? Errors { get; set; }
    
    /// <summary>
    /// 请求ID（用于追踪）
    /// </summary>
    public string? RequestId { get; set; }
    
    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 创建成功响应
    /// </summary>
    public static ApiResponse<T> SuccessResponse(T data, string? message = null, string? requestId = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            RequestId = requestId
        };
    }
    
    /// <summary>
    /// 创建失败响应
    /// </summary>
    public static ApiResponse<T> FailureResponse(
        string? message = null, 
        IReadOnlyDictionary<string, string[]>? errors = null,
        string? requestId = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors,
            RequestId = requestId
        };
    }
    
    /// <summary>
    /// 从Result转换
    /// </summary>
    public static ApiResponse<T> FromResult(Result<T> result, string? requestId = null)
    {
        if (result.IsSuccess && result.Data != null)
        {
            return SuccessResponse(result.Data, null, requestId);
        }
        
        var errors = result.Errors.Any() 
            ? new Dictionary<string, string[]> { { "General", result.Errors.ToArray() } }
            : null;
        
        return FailureResponse(result.Error ?? "操作失败", errors, requestId);
    }
}

/// <summary>
/// API响应类（无数据）
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse SuccessResponse(string? message = null, string? requestId = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            RequestId = requestId
        };
    }
    
    public static new ApiResponse FailureResponse(
        string? message = null,
        IReadOnlyDictionary<string, string[]>? errors = null,
        string? requestId = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors,
            RequestId = requestId
        };
    }
    
    public static ApiResponse FromResult(Result result, string? requestId = null)
    {
        if (result.IsSuccess)
        {
            return SuccessResponse(null, requestId);
        }
        
        var errors = result.Errors.Any()
            ? new Dictionary<string, string[]> { { "General", result.Errors.ToArray() } }
            : null;
        
        return FailureResponse(result.Error ?? "操作失败", errors, requestId);
    }
}

