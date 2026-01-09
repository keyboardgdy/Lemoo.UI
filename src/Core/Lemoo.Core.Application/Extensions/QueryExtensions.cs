using Lemoo.Core.Abstractions.CQRS;

namespace Lemoo.Core.Application.Extensions;

/// <summary>
/// 查询扩展方法
/// </summary>
public static class QueryExtensions
{
    /// <summary>
    /// 获取查询的返回类型
    /// </summary>
    public static Type? GetResponseType<TQuery>(this TQuery query)
    {
        if (query == null)
            return null;
            
        return query.GetType()
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>))
            ?.GetGenericArguments()
            .FirstOrDefault();
    }
}

