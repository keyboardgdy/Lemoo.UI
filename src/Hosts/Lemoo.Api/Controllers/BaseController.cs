using Lemoo.Api.Extensions;
using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo.Api.Controllers;

/// <summary>
/// 控制器基类 - 提供通用功能
/// </summary>
[ApiController]
public abstract class BaseController : ControllerBase
{
    protected readonly IMediator Mediator;
    protected readonly ILogger Logger;
    
    protected BaseController(IMediator mediator, ILogger logger)
    {
        Mediator = mediator;
        Logger = logger;
    }
    
    /// <summary>
    /// 发送命令并返回ActionResult
    /// </summary>
    protected async Task<ActionResult<T>> SendCommand<T>(
        ICommand<Result<T>> command)
    {
        try
        {
            var result = await Mediator.Send(command);
            if (result is Result<T> typedResult)
            {
                return typedResult.ToActionResult();
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "执行命令失败: {CommandType}", command.GetType().Name);
            return StatusCode(500, Result<T>.Failure("服务器内部错误"));
        }
    }
    
    /// <summary>
    /// 发送查询并返回ActionResult
    /// </summary>
    protected async Task<ActionResult<T>> SendQuery<T>(
        IQuery<Result<T>> query)
    {
        try
        {
            var result = await Mediator.Send(query);
            if (result is Result<T> typedResult)
            {
                return typedResult.ToActionResult();
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "执行查询失败: {QueryType}", query.GetType().Name);
            return StatusCode(500, Result<T>.Failure("服务器内部错误"));
        }
    }
    
    /// <summary>
    /// 发送命令并返回CreatedAtActionResult
    /// </summary>
    protected async Task<ActionResult<T>> SendCreateCommand<T>(
        ICommand<Result<T>> command,
        string actionName,
        Func<Result<T>, object> routeValueSelector)
    {
        try
        {
            var result = await Mediator.Send(command);
            if (result is Result<T> typedResult && typedResult.IsSuccess && typedResult.Data != null)
            {
                var routeValues = routeValueSelector(typedResult);
                return typedResult.ToCreatedAtActionResult(actionName, routeValues);
            }
            
            if (result is Result<T> typedResult2)
            {
                return BadRequest(typedResult2);
            }
            
            return BadRequest();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "创建资源失败: {CommandType}", command.GetType().Name);
            return StatusCode(500, Result<T>.Failure("服务器内部错误"));
        }
    }
}

