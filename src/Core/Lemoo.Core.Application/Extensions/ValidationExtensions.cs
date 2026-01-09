using Lemoo.Core.Common.Exceptions;

namespace Lemoo.Core.Application.Extensions;

/// <summary>
/// 验证扩展方法 - 用于WPF验证反馈
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// 将ValidationException转换为WPF友好的错误字典
    /// </summary>
    public static Dictionary<string, string[]> ToWpfErrors(this ValidationException ex)
    {
        return ex.Errors.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value
        );
    }
    
    /// <summary>
    /// 将ValidationException转换为WPF友好的错误消息列表
    /// </summary>
    public static IReadOnlyList<string> ToWpfErrorMessages(this ValidationException ex)
    {
        var messages = new List<string>();
        
        foreach (var error in ex.Errors)
        {
            if (error.Value.Length == 1)
            {
                messages.Add($"{error.Key}: {error.Value[0]}");
            }
            else
            {
                foreach (var message in error.Value)
                {
                    messages.Add($"{error.Key}: {message}");
                }
            }
        }
        
        return messages;
    }
    
    /// <summary>
    /// 将ValidationException转换为单个错误消息字符串
    /// </summary>
    public static string ToWpfErrorMessage(this ValidationException ex)
    {
        var messages = ex.ToWpfErrorMessages();
        return string.Join(Environment.NewLine, messages);
    }
    
    /// <summary>
    /// 获取指定属性的错误消息
    /// </summary>
    public static IReadOnlyList<string> GetPropertyErrors(this ValidationException ex, string propertyName)
    {
        if (ex.Errors.TryGetValue(propertyName, out var errors))
        {
            return errors;
        }
        
        return Array.Empty<string>();
    }
    
    /// <summary>
    /// 获取指定属性的第一个错误消息
    /// </summary>
    public static string? GetPropertyError(this ValidationException ex, string propertyName)
    {
        var errors = ex.GetPropertyErrors(propertyName);
        return errors.FirstOrDefault();
    }
}

