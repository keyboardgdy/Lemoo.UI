namespace Lemoo.Core.Common.Extensions;

/// <summary>
/// 字符串扩展方法
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// 检查字符串是否为null或空
    /// </summary>
    public static bool IsNullOrEmpty(this string? value) => string.IsNullOrEmpty(value);
    
    /// <summary>
    /// 检查字符串是否为null、空或仅包含空白字符
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value);
    
    /// <summary>
    /// 如果字符串为null或空，返回默认值
    /// </summary>
    public static string DefaultIfNullOrEmpty(this string? value, string defaultValue) 
        => string.IsNullOrEmpty(value) ? defaultValue : value;
    
    /// <summary>
    /// 如果字符串为null或空白，返回默认值
    /// </summary>
    public static string DefaultIfNullOrWhiteSpace(this string? value, string defaultValue) 
        => string.IsNullOrWhiteSpace(value) ? defaultValue : value;
}

