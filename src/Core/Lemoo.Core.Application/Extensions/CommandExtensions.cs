using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Application.Common;

namespace Lemoo.Core.Application.Extensions;

/// <summary>
/// 命令扩展方法
/// </summary>
public static class CommandExtensions
{
    /// <summary>
    /// 检查命令是否有返回值
    /// </summary>
    public static bool HasResponse<TCommand>(this TCommand command)
        where TCommand : ICommand
    {
        return command.GetType()
            .GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));
    }
    
    /// <summary>
    /// 获取命令的返回类型
    /// </summary>
    public static Type? GetResponseType<TCommand>(this TCommand command)
        where TCommand : ICommand
    {
        return command.GetType()
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>))
            ?.GetGenericArguments()
            .FirstOrDefault();
    }
}

