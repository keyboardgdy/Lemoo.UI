using System;
using System.Threading.Tasks;
using Lemoo.UI.WPF.Models;

namespace Lemoo.UI.WPF.Services;

/// <summary>
/// 用户设置服务接口
/// </summary>
public interface IUserSettingsService
{
    /// <summary>
    /// 获取当前用户设置
    /// </summary>
    UserSettings CurrentSettings { get; }

    /// <summary>
    /// 设置变更事件
    /// </summary>
    event EventHandler<SettingsChangedEventArgs>? SettingsChanged;

    /// <summary>
    /// 加载用户设置
    /// </summary>
    /// <returns>加载的用户设置</returns>
    Task<UserSettings> LoadAsync();

    /// <summary>
    /// 保存用户设置
    /// </summary>
    /// <param name="settings">要保存的设置</param>
    /// <returns>异步任务</returns>
    Task SaveAsync(UserSettings? settings = null);

    /// <summary>
    /// 更新主题设置
    /// </summary>
    /// <param name="themeName">主题名称</param>
    /// <returns>异步任务</returns>
    Task UpdateThemeAsync(string themeName);

    /// <summary>
    /// 更新窗口设置
    /// </summary>
    /// <param name="width">窗口宽度</param>
    /// <param name="height">窗口高度</param>
    /// <param name="left">窗口左侧位置</param>
    /// <param name="top">窗口顶部位置</param>
    /// <param name="windowState">窗口状态</param>
    /// <returns>异步任务</returns>
    Task UpdateWindowAsync(double width, double height, double left, double top, string windowState);

    /// <summary>
    /// 重置为默认设置
    /// </summary>
    /// <returns>异步任务</returns>
    Task ResetToDefaultAsync();
}

/// <summary>
/// 设置变更事件参数
/// </summary>
public class SettingsChangedEventArgs : EventArgs
{
    /// <summary>
    /// 设置的键名
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// 旧值
    /// </summary>
    public object? OldValue { get; }

    /// <summary>
    /// 新值
    /// </summary>
    public object? NewValue { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public SettingsChangedEventArgs(string key, object? oldValue, object? newValue)
    {
        Key = key;
        OldValue = oldValue;
        NewValue = newValue;
    }
}
