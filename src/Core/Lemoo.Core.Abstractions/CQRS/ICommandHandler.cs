using MediatR;

namespace Lemoo.Core.Abstractions.CQRS;

/// <summary>
/// 命令处理器接口
/// </summary>
/// <typeparam name="TCommand">命令类型</typeparam>
public interface ICommandHandler<TCommand> : MediatR.IRequestHandler<TCommand, Unit>
    where TCommand : ICommand
{
}

/// <summary>
/// 带返回值的命令处理器接口
/// </summary>
/// <typeparam name="TCommand">命令类型</typeparam>
/// <typeparam name="TResponse">返回类型</typeparam>
public interface ICommandHandler<TCommand, TResponse> : MediatR.IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}

/// <summary>
/// 请求处理器接口（MediatR）- 无返回值
/// </summary>
/// <typeparam name="TRequest">请求类型</typeparam>
public interface IRequestHandler<TRequest> : MediatR.IRequestHandler<TRequest, Unit>
    where TRequest : IRequest
{
}

/// <summary>
/// 请求处理器接口（MediatR）- 带返回值
/// </summary>
/// <typeparam name="TRequest">请求类型</typeparam>
/// <typeparam name="TResponse">返回类型</typeparam>
public interface IRequestHandler<TRequest, TResponse> : MediatR.IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
}

/// <summary>
/// 表示无返回值的类型
/// </summary>
public readonly struct Unit : IEquatable<Unit>
{
    public static readonly Unit Value = default;
    
    public bool Equals(Unit other) => true;
    public override bool Equals(object? obj) => obj is Unit;
    public override int GetHashCode() => 0;
}

