using Lemoo.UI.Models;

namespace Lemoo.UI.Services;

/// <summary>
/// 控件注册表，提供所有Lemoo.UI控件的元数据
/// </summary>
public static class ControlRegistry
{
    /// <summary>
    /// 获取所有控件信息
    /// </summary>
    public static IReadOnlyList<ControlInfo> GetAllControls() => _controls;

    /// <summary>
    /// 根据分类获取控件
    /// </summary>
    public static IReadOnlyList<ControlInfo> GetControlsByCategory(ControlCategory category)
        => _controls.Where(c => c.Category == category).ToList();

    /// <summary>
    /// 搜索控件
    /// </summary>
    public static IReadOnlyList<ControlInfo> SearchControls(string keyword)
        => _controls.Where(c =>
            c.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            c.DisplayName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            c.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();

    private static readonly List<ControlInfo> _controls = new()
    {
        // ===== 按钮 =====
        new ControlInfo
        {
            Name = "Button",
            DisplayName = "按钮",
            Description = "基础按钮控件，支持多种样式变体",
            Category = ControlCategory.Buttons,
            Type = ControlType.Styled,
            Icon = "M4 4h16v16H4V4z M6 6v12h12V6H6z",
            XamlNamespace = "",
            XamlNamespaceUri = "",
            SampleCode = "<Button Content=\"点击我\" />",
            StyleVariants = new List<ControlStyleVariant>
            {
                new() { StyleName = "Default", DisplayName = "默认", StyleKey = "" },
                new() { StyleName = "Primary", DisplayName = "主按钮", StyleKey = "Win11.Button.Primary" },
                new() { StyleName = "Outline", DisplayName = "轮廓按钮", StyleKey = "Win11.Button.Outline" },
                new() { StyleName = "Ghost", DisplayName = "幽灵按钮", StyleKey = "Win11.Button.Ghost" },
                new() { StyleName = "Danger", DisplayName = "危险按钮", StyleKey = "Win11.Button.Danger" }
            }
        },
        new ControlInfo
        {
            Name = "DropDownButton",
            DisplayName = "下拉按钮",
            Description = "点击显示下拉菜单的按钮",
            Category = ControlCategory.Buttons,
            Type = ControlType.Custom,
            Icon = "M4 6h16v2H4V6z M12 12l4 4-4 4v-8z",
            XamlNamespace = "ui",
            XamlNamespaceUri = "clr-namespace:Lemoo.UI.Controls;assembly=Lemoo.UI",
            SampleCode = "<ui:DropDownButton Content=\"选项\"><ui:DropDownButton.DropDownItems><MenuItem Header=\"选项1\"/><MenuItem Header=\"选项2\"/></ui:DropDownButton.DropDownItems></ui:DropDownButton>"
        },
        new ControlInfo
        {
            Name = "ToggleButton",
            DisplayName = "切换按钮",
            Description = "可以切换开关状态的按钮",
            Category = ControlCategory.Buttons,
            Type = ControlType.Styled,
            Icon = "M4 6h16v12H4V6z M6 8v8h12V8H6z",
            XamlNamespace = "",
            SampleCode = "<ToggleButton Content=\"切换\" />"
        },
        new ControlInfo
        {
            Name = "ToggleSwitch",
            DisplayName = "开关",
            Description = "现代化的开关控件",
            Category = ControlCategory.Buttons,
            Type = ControlType.Custom,
            Icon = "M4 10h12v4H4v-4z M20 8a6 6 0 1 0 0 12 6 6 0 0 0 0-12z",
            XamlNamespace = "ui",
            XamlNamespaceUri = "clr-namespace:Lemoo.UI.Controls;assembly=Lemoo.UI",
            SampleCode = "<ui:ToggleSwitch IsChecked=\"True\" />"
        },
        new ControlInfo
        {
            Name = "Badge",
            DisplayName = "徽章",
            Description = "用于显示通知计数或状态指示",
            Category = ControlCategory.Buttons,
            Type = ControlType.Custom,
            Icon = "M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2z",
            XamlNamespace = "ui",
            XamlNamespaceUri = "clr-namespace:Lemoo.UI.Controls;assembly=Lemoo.UI",
            SampleCode = "<ui:Badge Content=\"5\" />"
        },

        // ===== 输入 =====
        new ControlInfo
        {
            Name = "TextBox",
            DisplayName = "文本框",
            Description = "基础文本输入控件",
            Category = ControlCategory.Inputs,
            Type = ControlType.Styled,
            Icon = "M4 6h16v12H4V6z M6 8v8h12V8H6z",
            XamlNamespace = "",
            SampleCode = "<TextBox Text=\"示例文本\" />",
            StyleVariants = new List<ControlStyleVariant>
            {
                new() { StyleName = "Default", DisplayName = "默认", StyleKey = "Win11.TextBoxStyle" },
                new() { StyleName = "Icon", DisplayName = "带图标", StyleKey = "Win11.TextBox.Icon" },
                new() { StyleName = "Search", DisplayName = "搜索框", StyleKey = "Win11.TextBox.Search" },
                new() { StyleName = "ReadOnly", DisplayName = "只读", StyleKey = "Win11.TextBox.ReadOnly" },
                new() { StyleName = "MultiLine", DisplayName = "多行", StyleKey = "Win11.TextBox.MultiLine" },
                new() { StyleName = "Inline", DisplayName = "内联", StyleKey = "Win11.TextBox.Inline" }
            }
        },
        new ControlInfo
        {
            Name = "PasswordBox",
            DisplayName = "密码框",
            Description = "用于输入密码的文本框",
            Category = ControlCategory.Inputs,
            Type = ControlType.Styled,
            Icon = "M6 8v2h12V8H6z M12 12l2 2v-4l-2 2z",
            XamlNamespace = "",
            SampleCode = "<PasswordBox />"
        },
        new ControlInfo
        {
            Name = "NumericUpDown",
            DisplayName = "数字输入",
            Description = "专门用于数字输入的控件，带有增加/减少按钮",
            Category = ControlCategory.Inputs,
            Type = ControlType.Custom,
            Icon = "M8 8h8v8H8V8z M12 4v16 M4 12h16",
            XamlNamespace = "ui",
            XamlNamespaceUri = "clr-namespace:Lemoo.UI.Controls;assembly=Lemoo.UI",
            SampleCode = "<ui:NumericUpDown Value=\"10\" Minimum=\"0\" Maximum=\"100\" />"
        },
        new ControlInfo
        {
            Name = "SearchBox",
            DisplayName = "搜索框",
            Description = "带搜索图标的文本框",
            Category = ControlCategory.Inputs,
            Type = ControlType.Custom,
            Icon = "M15.5 14h-.79l-.28-.27C15.41 12.59 16 11.11 16 9.5 16 5.91 13.09 3 9.5 3S3 5.91 3 9.5 5.91 16 9.5 16c1.61 0 3.09-.59 4.23-1.57l.27.28v.79l5 4.99L20.49 19l-4.99-5zm-6 0C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z",
            XamlNamespace = "ui",
            XamlNamespaceUri = "clr-namespace:Lemoo.UI.Controls;assembly=Lemoo.UI",
            SampleCode = "<ui:SearchBox PlaceholderText=\"搜索...\" />"
        },
        new ControlInfo
        {
            Name = "CheckBox",
            DisplayName = "复选框",
            Description = "允许用户选择多个选项",
            Category = ControlCategory.Inputs,
            Type = ControlType.Styled,
            Icon = "M4 4h16v16H4V4z M6 6v12h12V6H6z M7 12l3 3 7-7",
            XamlNamespace = "",
            SampleCode = "<CheckBox Content=\"记住我\" />"
        },
        new ControlInfo
        {
            Name = "RadioButton",
            DisplayName = "单选按钮",
            Description = "允许用户从多个选项中选择一个",
            Category = ControlCategory.Inputs,
            Type = ControlType.Styled,
            Icon = "M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2z M12 20c-4.42 0-8-3.58-8-8s3.58-8 8-8 8 3.58 8 8-3.58 8-8 8z M12 8a4 4 0 1 0 0 8 4 4 0 0 0 0-8z",
            XamlNamespace = "",
            SampleCode = "<RadioButton Content=\"选项1\" />"
        },
        new ControlInfo
        {
            Name = "ComboBox",
            DisplayName = "下拉框",
            Description = "下拉选择控件",
            Category = ControlCategory.Inputs,
            Type = ControlType.Styled,
            Icon = "M4 6h16v12H4V6z M6 8v8h12V8H6z M12 12l4 4-4-4z",
            XamlNamespace = "",
            SampleCode = "<ComboBox><ComboBoxItem Content=\"选项1\"/><ComboBoxItem Content=\"选项2\"/></ComboBox>"
        },

        // ===== 列表 =====
        new ControlInfo
        {
            Name = "ListBox",
            DisplayName = "列表框",
            Description = "显示可选择项的列表",
            Category = ControlCategory.Lists,
            Type = ControlType.Styled,
            Icon = "M4 4h16v16H4V4z M6 6v12h12V6H6z M6 8h12 M6 10h12 M6 12h12",
            XamlNamespace = "",
            SampleCode = "<ListBox><ListBoxItem Content=\"项目1\"/><ListBoxItem Content=\"项目2\"/></ListBox>"
        },

        // ===== 菜单 =====
        new ControlInfo
        {
            Name = "MenuItem",
            DisplayName = "菜单项",
            Description = "菜单中的单个项",
            Category = ControlCategory.Menus,
            Type = ControlType.Styled,
            Icon = "M4 6h16v2H4V6z M4 10h16v2H4v-2z M4 14h16v2H4v-2z",
            XamlNamespace = "",
            SampleCode = "<MenuItem Header=\"文件\"/>"
        },

        // ===== 进度 =====
        new ControlInfo
        {
            Name = "ProgressBar",
            DisplayName = "进度条",
            Description = "显示操作进度的条形指示器",
            Category = ControlCategory.Progress,
            Type = ControlType.Styled,
            Icon = "M2 10h20v4H2v-4z M4 12h16",
            XamlNamespace = "",
            SampleCode = "<ProgressBar Value=\"50\" Maximum=\"100\" />"
        },
        new ControlInfo
        {
            Name = "ProgressRing",
            DisplayName = "进度环",
            Description = "圆形的进度指示器",
            Category = ControlCategory.Progress,
            Type = ControlType.Custom,
            Icon = "M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2z M12 4v8h8",
            XamlNamespace = "ui",
            XamlNamespaceUri = "clr-namespace:Lemoo.UI.Controls;assembly=Lemoo.UI",
            SampleCode = "<ui:ProgressRing IsActive=\"True\" />"
        },

        // ===== 滑块 =====
        new ControlInfo
        {
            Name = "Slider",
            DisplayName = "滑块",
            Description = "允许用户通过拖动滑块选择值",
            Category = ControlCategory.Sliders,
            Type = ControlType.Styled,
            Icon = "M2 12h20 M12 12v-8 M12 12v8",
            XamlNamespace = "",
            SampleCode = "<Slider Value=\"50\" Minimum=\"0\" Maximum=\"100\" />"
        },

        // ===== 卡片 =====
        new ControlInfo
        {
            Name = "Card",
            DisplayName = "卡片",
            Description = "用于展示内容的容器控件",
            Category = ControlCategory.Cards,
            Type = ControlType.Custom,
            Icon = "M4 4h16v16H4V4z M6 6v12h12V6H6z",
            XamlNamespace = "ui",
            XamlNamespaceUri = "clr-namespace:Lemoo.UI.Controls;assembly=Lemoo.UI",
            SampleCode = "<ui:Card Padding=\"16\"><TextBlock Text=\"卡片内容\"/></ui:Card>"
        },
        new ControlInfo
        {
            Name = "Expander",
            DisplayName = "折叠面板",
            Description = "可以展开/折叠内容的控件",
            Category = ControlCategory.Cards,
            Type = ControlType.Custom,
            Icon = "M4 6h16v12H4V6z M12 10l4 4-4 4v-8z",
            XamlNamespace = "ui",
            XamlNamespaceUri = "clr-namespace:Lemoo.UI.Controls;assembly=Lemoo.UI",
            SampleCode = "<ui:Expander Header=\"标题\"><TextBlock Text=\"可展开内容\"/></ui:Expander>"
        },

        // ===== 对话框 =====
        new ControlInfo
        {
            Name = "DialogHost",
            DisplayName = "对话框宿主",
            Description = "在窗口内显示模态对话框",
            Category = ControlCategory.Dialogs,
            Type = ControlType.Custom,
            Icon = "M4 4h16v12H4V4z M6 6v8h12V6H6z",
            XamlNamespace = "ui",
            XamlNamespaceUri = "clr-namespace:Lemoo.UI.Controls;assembly=Lemoo.UI",
            SampleCode = "<ui:DialogHost IsOpen=\"True\"><ui:DialogHost.DialogContent><TextBlock Text=\"对话框内容\"/></ui:DialogHost.DialogContent></ui:DialogHost>"
        },
        new ControlInfo
        {
            Name = "MessageBox",
            DisplayName = "消息框",
            Description = "显示消息的模态窗口",
            Category = ControlCategory.Dialogs,
            Type = ControlType.Custom,
            Icon = "M4 4h16v16H4V4z M6 6v12h12V6H6z M7 7l10 10 M17 7l-10 10",
            XamlNamespace = "ui",
            XamlNamespaceUri = "clr-namespace:Lemoo.UI.Controls;assembly=Lemoo.UI",
            SampleCode = "<ui:MessageBox.Show(\"提示\", \"这是一个消息框\") />"
        },

        // ===== 通知 =====
        new ControlInfo
        {
            Name = "Snackbar",
            DisplayName = "提示条",
            Description = "在屏幕底部显示的临时通知",
            Category = ControlCategory.Notifications,
            Type = ControlType.Custom,
            Icon = "M2 18h20v2H2v-2z M4 4h16v12H4V4z",
            XamlNamespace = "ui",
            XamlNamespaceUri = "clr-namespace:Lemoo.UI.Controls;assembly=Lemoo.UI",
            SampleCode = "<ui:Snackbar Message=\"操作成功\" IsOpen=\"True\" />"
        },
        new ControlInfo
        {
            Name = "ToolTip",
            DisplayName = "工具提示",
            Description = "鼠标悬停时显示的提示信息",
            Category = ControlCategory.Notifications,
            Type = ControlType.Styled,
            Icon = "M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2z M11 7h2v6h-2V7z M11 15h2v2h-2v-2z",
            XamlNamespace = "",
            SampleCode = "<Button Content=\"悬停\"><Button.ToolTip><ToolTip Content=\"这是提示\"/></Button.ToolTip></Button>"
        },

        // ===== 导航 =====
        new ControlInfo
        {
            Name = "Sidebar",
            DisplayName = "侧边栏",
            Description = "左侧导航栏",
            Category = ControlCategory.Navigation,
            Type = ControlType.Custom,
            Icon = "M4 4h6v16H4V4z M12 4h8v4h-8V4z M12 10h8v4h-8v-4z M12 16h8v4h-8v-4z",
            XamlNamespace = "ui",
            XamlNamespaceUri = "clr-namespace:Lemoo.UI.Controls;assembly=Lemoo.UI",
            SampleCode = "<ui:Sidebar /></ui:Sidebar>"
        },
        new ControlInfo
        {
            Name = "DocumentTabHost",
            DisplayName = "文档标签页",
            Description = "类似VS的文档标签页",
            Category = ControlCategory.Navigation,
            Type = ControlType.Custom,
            Icon = "M2 8h8v10H2V8z M4 6h8v10H4V6z M6 4h8v10H6V4z",
            XamlNamespace = "ui",
            XamlNamespaceUri = "clr-namespace:Lemoo.UI.Controls;assembly=Lemoo.UI",
            SampleCode = "<ui:DocumentTabHost />"
        },

        // ===== 窗口装饰 =====
        new ControlInfo
        {
            Name = "MainTitleBar",
            DisplayName = "主标题栏",
            Description = "自定义窗口标题栏",
            Category = ControlCategory.Chrome,
            Type = ControlType.Custom,
            Icon = "M2 6h20v4H2V6z M4 4h16v2H4V4z",
            XamlNamespace = "ui",
            XamlNamespaceUri = "clr-namespace:Lemoo.UI.Controls;assembly=Lemoo.UI",
            SampleCode = "<ui:MainTitleBar Title=\"窗口标题\" />"
        },

        // ===== 其他 =====
        new ControlInfo
        {
            Name = "ScrollBar",
            DisplayName = "滚动条",
            Description = "自定义样式的滚动条",
            Category = ControlCategory.Others,
            Type = ControlType.Styled,
            Icon = "M4 2h4v20H4V2z M16 2h4v20h-4V2z",
            XamlNamespace = "",
            SampleCode = "<ScrollBar />"
        },
        new ControlInfo
        {
            Name = "Separator",
            DisplayName = "分隔符",
            Description = "用于分隔内容的线条",
            Category = ControlCategory.Others,
            Type = ControlType.Styled,
            Icon = "M2 11h20v2H2v-2z",
            XamlNamespace = "",
            SampleCode = "<Separator />"
        }
    };

    /// <summary>
    /// 获取分类的显示名称
    /// </summary>
    public static string GetCategoryDisplayName(ControlCategory category) => category switch
    {
        ControlCategory.Buttons => "按钮",
        ControlCategory.Inputs => "输入",
        ControlCategory.Lists => "列表",
        ControlCategory.Menus => "菜单",
        ControlCategory.Progress => "进度",
        ControlCategory.Sliders => "滑块",
        ControlCategory.Cards => "卡片",
        ControlCategory.Dialogs => "对话框",
        ControlCategory.Notifications => "通知",
        ControlCategory.Navigation => "导航",
        ControlCategory.Chrome => "窗口装饰",
        ControlCategory.Others => "其他",
        _ => "未知"
    };
}
