using Lemoo.Core.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo.Api.Extensions;

/// <summary>
/// Result扩展方法
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// 将Result转换为ActionResult
    /// </summary>
    public static ActionResult<T> ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess && result.Data != null)
        {
            return new OkObjectResult(result);
        }
        
        if (result.IsSuccess && result.Data == null)
        {
            return new NotFoundObjectResult(result);
        }
        
        return new BadRequestObjectResult(result);
    }
    
    /// <summary>
    /// 将Result转换为ActionResult（无数据）
    /// </summary>
    public static ActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result);
        }
        
        return new BadRequestObjectResult(result);
    }
    
    /// <summary>
    /// 将Result转换为CreatedAtActionResult（用于创建资源）
    /// </summary>
    public static ActionResult<T> ToCreatedAtActionResult<T>(
        this Result<T> result,
        string actionName,
        object routeValues)
    {
        if (result.IsSuccess && result.Data != null)
        {
            return new CreatedAtActionResult(actionName, null, routeValues, result);
        }
        
        return new BadRequestObjectResult(result);
    }
    
    /// <summary>
    /// 映射Result中的数据
    /// </summary>
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapper)
    {
        if (result.IsFailure)
        {
            return Result<TOut>.Failure(result.Error ?? string.Join(", ", result.Errors));
        }
        
        if (result.Data == null)
        {
            return Result<TOut>.Failure("数据为空");
        }
        
        try
        {
            var mappedData = mapper(result.Data);
            return Result<TOut>.Success(mappedData);
        }
        catch (Exception ex)
        {
            return Result<TOut>.Failure($"映射失败: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 异步映射Result中的数据
    /// </summary>
    public static async Task<Result<TOut>> MapAsync<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Task<TOut>> mapper)
    {
        if (result.IsFailure)
        {
            return Result<TOut>.Failure(result.Error ?? string.Join(", ", result.Errors));
        }
        
        if (result.Data == null)
        {
            return Result<TOut>.Failure("数据为空");
        }
        
        try
        {
            var mappedData = await mapper(result.Data);
            return Result<TOut>.Success(mappedData);
        }
        catch (Exception ex)
        {
            return Result<TOut>.Failure($"映射失败: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 如果Result成功，执行操作
    /// </summary>
    public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess && result.Data != null)
        {
            action(result.Data);
        }
        
        return result;
    }
    
    /// <summary>
    /// 如果Result失败，执行操作
    /// </summary>
    public static Result<T> OnFailure<T>(this Result<T> result, Action<string> action)
    {
        if (result.IsFailure)
        {
            action(result.Error ?? string.Join(", ", result.Errors));
        }
        
        return result;
    }
}

