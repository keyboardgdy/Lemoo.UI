using MediatR;

namespace Lemoo.Core.Abstractions.CQRS;

/// <summary>
/// 查询接口 - 用于CQRS模式的查询
/// </summary>
/// <typeparam name="TResponse">返回类型</typeparam>
public interface IQuery<TResponse> : MediatR.IRequest<TResponse>
{
}

