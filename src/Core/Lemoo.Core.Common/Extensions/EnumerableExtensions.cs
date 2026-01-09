namespace Lemoo.Core.Common.Extensions;

/// <summary>
/// 集合扩展方法
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// 检查集合是否为null或空
    /// </summary>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source == null || !source.Any();
    }
    
    /// <summary>
    /// 如果集合为null，返回空集合
    /// </summary>
    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source)
    {
        return source ?? Enumerable.Empty<T>();
    }
    
    /// <summary>
    /// 对集合中的每个元素执行操作
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }
    
    /// <summary>
    /// 异步对集合中的每个元素执行操作
    /// </summary>
    public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> action)
    {
        foreach (var item in source)
        {
            await action(item);
        }
    }
}

