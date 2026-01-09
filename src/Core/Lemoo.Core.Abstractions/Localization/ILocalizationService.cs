namespace Lemoo.Core.Abstractions.Localization;

/// <summary>
/// 本地化服务接口 - 提供多语言支持
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// 当前文化
    /// </summary>
    string CurrentCulture { get; set; }
    
    /// <summary>
    /// 文化变更事件
    /// </summary>
    event EventHandler<CultureChangedEventArgs>? CultureChanged;
    
    /// <summary>
    /// 获取本地化字符串
    /// </summary>
    /// <param name="key">资源键</param>
    /// <param name="args">格式化参数</param>
    /// <returns>本地化字符串</returns>
    string GetString(string key, params object[] args);
    
    /// <summary>
    /// 获取指定文化的本地化字符串
    /// </summary>
    /// <param name="key">资源键</param>
    /// <param name="culture">文化代码</param>
    /// <param name="args">格式化参数</param>
    /// <returns>本地化字符串</returns>
    string GetString(string key, string? culture, params object[] args);
    
    /// <summary>
    /// 获取本地化字符串（带默认值）
    /// </summary>
    /// <param name="key">资源键</param>
    /// <param name="defaultValue">默认值</param>
    /// <param name="args">格式化参数</param>
    /// <returns>本地化字符串</returns>
    string GetStringOrDefault(string key, string defaultValue, params object[] args);
    
    /// <summary>
    /// 检查资源键是否存在
    /// </summary>
    /// <param name="key">资源键</param>
    /// <returns>是否存在</returns>
    bool HasKey(string key);
    
    /// <summary>
    /// 获取支持的文化列表
    /// </summary>
    /// <returns>文化代码列表</returns>
    IReadOnlyList<string> GetSupportedCultures();
}

/// <summary>
/// 文化变更事件参数
/// </summary>
public class CultureChangedEventArgs : EventArgs
{
    public string OldCulture { get; }
    public string NewCulture { get; }
    
    public CultureChangedEventArgs(string oldCulture, string newCulture)
    {
        OldCulture = oldCulture;
        NewCulture = newCulture;
    }
}

