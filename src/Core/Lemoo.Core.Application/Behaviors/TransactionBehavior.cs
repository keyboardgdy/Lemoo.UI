using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Abstractions.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lemoo.Core.Application.Behaviors;

/// <summary>
/// 事务处理管道行为 - 自动管理数据库事务
/// </summary>
/// <typeparam name="TRequest">请求类型</typeparam>
/// <typeparam name="TResponse">返回类型</typeparam>
public class TransactionBehavior<TRequest, TResponse> : MediatR.IPipelineBehavior<TRequest, TResponse>
    where TRequest : MediatR.IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
    
    public TransactionBehavior(
        IUnitOfWork unitOfWork,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // 只对命令（ICommand）启用事务，查询不需要事务
        if (request is not ICommand)
        {
            return await next();
        }
        
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("开始事务: {RequestName}", requestName);
        
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var response = await next();
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            
            _logger.LogInformation("事务提交成功: {RequestName}", requestName);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "事务回滚: {RequestName}", requestName);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}

