using Lemoo.UI.Models.Icons;

namespace Lemoo.UI.WPF.Constants;

/// <summary>
/// 导航相关常量
/// </summary>
public static class NavigationConstants
{
    /// <summary>
    /// 导航菜单图标
    /// </summary>
    public static class Icons
    {
        /// <summary>
        /// 设置图标
        /// </summary>
        public const IconKind Settings = IconKind.Settings;

        /// <summary>
        /// 工具箱图标
        /// </summary>
        public const IconKind Toolbox = IconKind.IntegrationTest;

        /// <summary>
        /// 图标浏览器图标
        /// </summary>
        public const IconKind IconBrowser = IconKind.Paste;
    }

    /// <summary>
    /// 默认菜单文本
    /// </summary>
    public static class MenuText
    {
        /// <summary>
        /// 设置示例
        /// </summary>
        public const string SettingsSample = "设置示例";

        /// <summary>
        /// 设置
        /// </summary>
        public const string Settings = "设置";

        /// <summary>
        /// 工具箱
        /// </summary>
        public const string ToolboxSample = "工具箱";

        /// <summary>
        /// 图标浏览器
        /// </summary>
        public const string IconBrowser = "图标浏览器";
    }
}
