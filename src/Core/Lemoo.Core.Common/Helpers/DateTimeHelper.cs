namespace Lemoo.Core.Common.Helpers;

/// <summary>
/// 日期时间辅助类
/// </summary>
public static class DateTimeHelper
{
    /// <summary>
    /// 获取当前UTC时间
    /// </summary>
    public static DateTime UtcNow => DateTime.UtcNow;
    
    /// <summary>
    /// 获取当前本地时间
    /// </summary>
    public static DateTime Now => DateTime.Now;
    
    /// <summary>
    /// 将UTC时间转换为本地时间
    /// </summary>
    public static DateTime ToLocalTime(DateTime utcTime)
    {
        return utcTime.ToLocalTime();
    }
    
    /// <summary>
    /// 将本地时间转换为UTC时间
    /// </summary>
    public static DateTime ToUtcTime(DateTime localTime)
    {
        return localTime.ToUniversalTime();
    }
}

