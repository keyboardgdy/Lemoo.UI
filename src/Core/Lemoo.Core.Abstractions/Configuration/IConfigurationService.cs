using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Lemoo.Core.Abstractions.Configuration;

/// <summary>
/// 配置服务接口
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// 获取配置值
    /// </summary>
    T? GetValue<T>(string key);
    
    /// <summary>
    /// 获取配置值（带默认值）
    /// </summary>
    T GetValue<T>(string key, T defaultValue);
    
    /// <summary>
    /// 获取配置节
    /// </summary>
    IConfigurationSection GetSection(string key);
    
    /// <summary>
    /// 获取连接字符串
    /// </summary>
    string? GetConnectionString(string name);
    
    /// <summary>
    /// 配置变更事件
    /// </summary>
    event EventHandler<ConfigurationChangedEventArgs>? ConfigurationChanged;
    
    /// <summary>
    /// 获取配置选项监视器（支持热更新）
    /// </summary>
    IOptionsMonitor<T> GetOptionsMonitor<T>() where T : class;
    
    /// <summary>
    /// 重新加载配置
    /// </summary>
    void Reload();
}

/// <summary>
/// 配置变更事件参数
/// </summary>
public class ConfigurationChangedEventArgs : EventArgs
{
    public string Key { get; }
    public string? OldValue { get; }
    public string? NewValue { get; }
    
    public ConfigurationChangedEventArgs(string key, string? oldValue, string? newValue)
    {
        Key = key;
        OldValue = oldValue;
        NewValue = newValue;
    }
}

