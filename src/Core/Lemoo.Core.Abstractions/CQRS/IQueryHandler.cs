using MediatR;

namespace Lemoo.Core.Abstractions.CQRS;

/// <summary>
/// 查询处理器接口
/// </summary>
/// <typeparam name="TQuery">查询类型</typeparam>
/// <typeparam name="TResponse">返回类型</typeparam>
public interface IQueryHandler<TQuery, TResponse> : MediatR.IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}

