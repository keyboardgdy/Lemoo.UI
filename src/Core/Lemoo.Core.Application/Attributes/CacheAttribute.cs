namespace Lemoo.Core.Application.Attributes;

/// <summary>
/// 缓存特性
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class CacheAttribute : Attribute
{
    /// <summary>
    /// 缓存过期时间
    /// </summary>
    public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(5);
    
    /// <summary>
    /// 缓存键前缀
    /// </summary>
    public string? KeyPrefix { get; set; }
}

