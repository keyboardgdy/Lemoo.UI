using System;
using Lemoo.UI.Helpers;

namespace Lemoo.UI.WPF.Models;

/// <summary>
/// 用户设置配置模型
/// </summary>
public class UserSettings
{
    /// <summary>
    /// 主题设置
    /// </summary>
    public ThemeSettings Theme { get; set; } = new();

    /// <summary>
    /// 窗口设置
    /// </summary>
    public WindowSettings Window { get; set; } = new();

    /// <summary>
    /// 编辑器设置
    /// </summary>
    public EditorSettings Editor { get; set; } = new();

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.Now;
}

/// <summary>
/// 主题设置
/// </summary>
public class ThemeSettings
{
    /// <summary>
    /// 当前主题
    /// </summary>
    public string CurrentTheme { get; set; } = "Base";

    /// <summary>
    /// 是否跟随系统主题
    /// </summary>
    public bool FollowSystem { get; set; } = false;
}

/// <summary>
/// 窗口设置
/// </summary>
public class WindowSettings
{
    /// <summary>
    /// 窗口宽度
    /// </summary>
    public double Width { get; set; } = 1200;

    /// <summary>
    /// 窗口高度
    /// </summary>
    public double Height { get; set; } = 800;

    /// <summary>
    /// 窗口左侧位置
    /// </summary>
    public double Left { get; set; } = 0;

    /// <summary>
    /// 窗口顶部位置
    /// </summary>
    public double Top { get; set; } = 0;

    /// <summary>
    /// 窗口状态（Normal, Maximized, Minimized）
    /// </summary>
    public string WindowState { get; set; } = "Normal";

    /// <summary>
    /// 是否记住窗口位置
    /// </summary>
    public bool RememberPosition { get; set; } = true;
}

/// <summary>
/// 编辑器设置
/// </summary>
public class EditorSettings
{
    /// <summary>
    /// 字体大小
    /// </summary>
    public int FontSize { get; set; } = 14;

    /// <summary>
    /// 字体家族
    /// </summary>
    public string FontFamily { get; set; } = "Microsoft YaHei UI";

    /// <summary>
    /// 是否自动换行
    /// </summary>
    public bool WordWrap { get; set; } = true;

    /// <summary>
    /// 是否显示行号
    /// </summary>
    public bool ShowLineNumbers { get; set; } = true;
}
