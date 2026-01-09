namespace Lemoo.Core.Common.Helpers;

/// <summary>
/// 参数守卫类
/// </summary>
public static class Guard
{
    /// <summary>
    /// 检查参数是否为null
    /// </summary>
    public static void AgainstNull<T>(T? value, string parameterName) where T : class
    {
        if (value == null)
            throw new ArgumentNullException(parameterName);
    }
    
    /// <summary>
    /// 检查字符串是否为null或空
    /// </summary>
    public static void AgainstNullOrEmpty(string? value, string parameterName)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException($"参数 '{parameterName}' 不能为null或空", parameterName);
    }
    
    /// <summary>
    /// 检查字符串是否为null或空白
    /// </summary>
    public static void AgainstNullOrWhiteSpace(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"参数 '{parameterName}' 不能为null或空白", parameterName);
    }
    
    /// <summary>
    /// 检查条件是否为true
    /// </summary>
    public static void Against(bool condition, string message)
    {
        if (condition)
            throw new ArgumentException(message);
    }
    
    /// <summary>
    /// 检查条件是否为false
    /// </summary>
    public static void Require(bool condition, string message)
    {
        if (!condition)
            throw new ArgumentException(message);
    }
}

