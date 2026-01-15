namespace Lemoo.UI.Design;

/// <summary>
/// 定义工具箱分类的常量
/// </summary>
public static class ToolboxCategories
{
    /// <summary>
    /// 按钮类控件
    /// </summary>
    public const string Buttons = "Lemoo.UI - 按钮";

    /// <summary>
    /// 输入类控件
    /// </summary>
    public const string Inputs = "Lemoo.UI - 输入";

    /// <summary>
    /// 列表类控件
    /// </summary>
    public const string Lists = "Lemoo.UI - 列表";

    /// <summary>
    /// 菜单类控件
    /// </summary>
    public const string Menus = "Lemoo.UI - 菜单";

    /// <summary>
    /// 进度类控件
    /// </summary>
    public const string Progress = "Lemoo.UI - 进度";

    /// <summary>
    /// 滑块类控件
    /// </summary>
    public const string Sliders = "Lemoo.UI - 滑块";

    /// <summary>
    /// 卡片类控件
    /// </summary>
    public const string Cards = "Lemoo.UI - 卡片";

    /// <summary>
    /// 对话框类控件
    /// </summary>
    public const string Dialogs = "Lemoo.UI - 对话框";

    /// <summary>
    /// 通知类控件
    /// </summary>
    public const string Notifications = "Lemoo.UI - 通知";

    /// <summary>
    /// 导航类控件
    /// </summary>
    public const string Navigation = "Lemoo.UI - 导航";

    /// <summary>
    /// 窗口装饰类控件
    /// </summary>
    public const string Chrome = "Lemoo.UI - 窗口装饰";

    /// <summary>
    /// 其他控件
    /// </summary>
    public const string Others = "Lemoo.UI - 其他";

    /// <summary>
    /// 所有分类（用于遍历）
    /// </summary>
    public static readonly string[] AllCategories = new[]
    {
        Buttons,
        Inputs,
        Lists,
        Menus,
        Progress,
        Sliders,
        Cards,
        Dialogs,
        Notifications,
        Navigation,
        Chrome,
        Others
    };
}
