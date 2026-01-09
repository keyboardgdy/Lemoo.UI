using Lemoo.Core.Application.Common;

namespace Lemoo.Core.Application.Extensions;

/// <summary>
/// Result扩展方法
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// 将Result转换为ApiResponse
    /// </summary>
    public static ApiResponse<T> ToApiResponse<T>(this Result<T> result, string? requestId = null)
    {
        return ApiResponse<T>.FromResult(result, requestId);
    }
    
    /// <summary>
    /// 将Result转换为ApiResponse（无数据）
    /// </summary>
    public static ApiResponse ToApiResponse(this Result result, string? requestId = null)
    {
        return ApiResponse.FromResult(result, requestId);
    }
    
    /// <summary>
    /// 将PagedResult转换为ApiResponse
    /// </summary>
    public static ApiResponse<PagedResult<T>> ToApiResponse<T>(this PagedResult<T> result, string? requestId = null)
    {
        if (result.IsSuccess)
        {
            return ApiResponse<PagedResult<T>>.SuccessResponse(result, null, requestId);
        }
        
        var errors = result.Errors.Any() 
            ? new Dictionary<string, string[]> { { "General", result.Errors.ToArray() } }
            : null;
        
        return ApiResponse<PagedResult<T>>.FailureResponse(result.Error ?? "操作失败", errors, requestId);
    }
}

