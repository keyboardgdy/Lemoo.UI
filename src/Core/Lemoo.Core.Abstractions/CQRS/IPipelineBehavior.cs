using MediatR;

namespace Lemoo.Core.Abstractions.CQRS;

/// <summary>
/// 管道行为接口 - 用于MediatR的管道中间件
/// </summary>
/// <typeparam name="TRequest">请求类型</typeparam>
/// <typeparam name="TResponse">返回类型</typeparam>
public interface IPipelineBehavior<TRequest, TResponse> : MediatR.IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
}

