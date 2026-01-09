using MediatR;

namespace Lemoo.Core.Abstractions.CQRS;

/// <summary>
/// 命令接口 - 用于CQRS模式的命令
/// </summary>
public interface ICommand : MediatR.IRequest<Unit>
{
}

/// <summary>
/// 带返回值的命令接口
/// </summary>
/// <typeparam name="TResponse">返回类型</typeparam>
public interface ICommand<TResponse> : MediatR.IRequest<TResponse>
{
}

/// <summary>
/// 请求接口（MediatR）- 无返回值
/// </summary>
public interface IRequest : MediatR.IRequest<Unit>
{
}

/// <summary>
/// 带返回值的请求接口（MediatR）
/// </summary>
/// <typeparam name="TResponse">返回类型</typeparam>
public interface IRequest<TResponse> : MediatR.IRequest<TResponse>
{
}

