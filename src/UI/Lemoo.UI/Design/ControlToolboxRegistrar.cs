using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Lemoo.UI.Design
{
    /// <summary>
    /// WPF核心控件的工具箱注册（通过附加属性提供Win11样式）
    /// </summary>
    public static class CoreControlsToolboxRegistration
    {
        #region Button 注册

        /// <summary>
        /// 为Button提供工具箱支持
        /// </summary>
        public static class Button
        {
            /// <summary>
            /// 获取控件的工具箱描述
            /// </summary>
            public static string Description => "支持多种Win11样式的按钮控件";

            /// <summary>
            /// 获取控件的工具箱分类
            /// </summary>
            public static string Category => ToolboxCategories.Buttons;

            /// <summary>
            /// 获取示例XAML代码
            /// </summary>
            public static string ExampleCode => "<Button Content=\"点击我\" Style=\"{StaticResource Win11.Button.Primary}\" />";

            /// <summary>
            /// 可用的样式变体
            /// </summary>
            public static readonly string[] StyleVariants = new[]
            {
                "Win11.ButtonStyle",
                "Win11.Button.Primary",
                "Win11.Button.Outline",
                "Win11.Button.Ghost",
                "Win11.Button.Danger"
            };
        }

        #endregion

        #region TextBox 注册

        /// <summary>
        /// 为TextBox提供工具箱支持
        /// </summary>
        public static class TextBox
        {
            public static string Description => "支持Win11样式的文本输入控件";
            public static string Category => ToolboxCategories.Inputs;
            public static string ExampleCode => "<TextBox Text=\"示例文本\" Style=\"{StaticResource Win11.TextBoxStyle}\" />";

            public static readonly string[] StyleVariants = new[]
            {
                "Win11.TextBoxStyle",
                "Win11.TextBox.Icon",
                "Win11.TextBox.Search",
                "Win11.TextBox.ReadOnly",
                "Win11.TextBox.MultiLine",
                "Win11.TextBox.Inline"
            };
        }

        #endregion

        #region PasswordBox 注册

        /// <summary>
        /// 为PasswordBox提供工具箱支持
        /// </summary>
        public static class PasswordBox
        {
            public static string Description => "支持Win11样式的密码输入控件";
            public static string Category => ToolboxCategories.Inputs;
            public static string ExampleCode => "<PasswordBox Style=\"{StaticResource Win11.PasswordBoxStyle}\" />";
        }

        #endregion

        #region CheckBox 注册

        /// <summary>
        /// 为CheckBox提供工具箱支持
        /// </summary>
        public static class CheckBox
        {
            public static string Description => "支持Win11样式的复选框控件";
            public static string Category => ToolboxCategories.Inputs;
            public static string ExampleCode => "<CheckBox Content=\"记住我\" Style=\"{StaticResource Win11.CheckBoxStyle}\" />";
        }

        #endregion

        #region RadioButton 注册

        /// <summary>
        /// 为RadioButton提供工具箱支持
        /// </summary>
        public static class RadioButton
        {
            public static string Description => "支持Win11样式的单选按钮控件";
            public static string Category => ToolboxCategories.Inputs;
            public static string ExampleCode => "<RadioButton Content=\"选项1\" Style=\"{StaticResource Win11.RadioButtonStyle}\" />";
        }

        #endregion

        #region ComboBox 注册

        /// <summary>
        /// 为ComboBox提供工具箱支持
        /// </summary>
        public static class ComboBox
        {
            public static string Description => "支持Win11样式的下拉选择控件";
            public static string Category => ToolboxCategories.Inputs;
            public static string ExampleCode => "<ComboBox Style=\"{StaticResource Win11.ComboBoxStyle}\"><ComboBoxItem Content=\"选项1\"/></ComboBox>";
        }

        #endregion

        #region ListBox 注册

        /// <summary>
        /// 为ListBox提供工具箱支持
        /// </summary>
        public static class ListBox
        {
            public static string Description => "支持Win11样式的列表框控件";
            public static string Category => ToolboxCategories.Lists;
            public static string ExampleCode => "<ListBox Style=\"{StaticResource Win11.ListBoxStyle}\"><ListBoxItem Content=\"项目1\"/></ListBox>";
        }

        #endregion

        #region MenuItem 注册

        /// <summary>
        /// 为MenuItem提供工具箱支持
        /// </summary>
        public static class MenuItem
        {
            public static string Description => "支持Win11样式的菜单项控件";
            public static string Category => ToolboxCategories.Menus;
            public static string ExampleCode => "<MenuItem Header=\"文件\" Style=\"{StaticResource Win11.MenuItemStyle}\" />";
        }

        #endregion

        #region ProgressBar 注册

        /// <summary>
        /// 为ProgressBar提供工具箱支持
        /// </summary>
        public static class ProgressBar
        {
            public static string Description => "支持Win11样式的进度条控件";
            public static string Category => ToolboxCategories.Progress;
            public static string ExampleCode => "<ProgressBar Value=\"50\" Maximum=\"100\" Style=\"{StaticResource Win11.ProgressBarStyle}\" />";
        }

        #endregion

        #region Slider 注册

        /// <summary>
        /// 为Slider提供工具箱支持
        /// </summary>
        public static class Slider
        {
            public static string Description => "支持Win11样式的滑块控件";
            public static string Category => ToolboxCategories.Sliders;
            public static string ExampleCode => "<Slider Value=\"50\" Minimum=\"0\" Maximum=\"100\" Style=\"{StaticResource Win11.SliderStyle}\" />";
        }

        #endregion

        #region ScrollBar 注册

        /// <summary>
        /// 为ScrollBar提供工具箱支持
        /// </summary>
        public static class ScrollBar
        {
            public static string Description => "自定义样式的滚动条控件";
            public static string Category => ToolboxCategories.Others;
            public static string ExampleCode => "<ScrollBar Style=\"{StaticResource Win11.ScrollBarStyle}\" />";
        }

        #endregion

        #region Separator 注册

        /// <summary>
        /// 为Separator提供工具箱支持
        /// </summary>
        public static class Separator
        {
            public static string Description => "用于分隔内容的线条控件";
            public static string Category => ToolboxCategories.Others;
            public static string ExampleCode => "<Separator Style=\"{StaticResource Win11.SeparatorStyle}\" />";
        }

        #endregion

        #region ToolTip 注册

        /// <summary>
        /// 为ToolTip提供工具箱支持
        /// </summary>
        public static class ToolTip
        {
            public static string Description => "支持Win11样式的工具提示控件";
            public static string Category => ToolboxCategories.Notifications;
            public static string ExampleCode => "<Button Content=\"悬停\"><Button.ToolTip><ToolTip Content=\"这是提示\" Style=\"{StaticResource Win11.ToolTipStyle}\"/></Button.ToolTip></Button>";
        }

        #endregion

        #region ToggleButton 注册

        /// <summary>
        /// 为ToggleButton提供工具箱支持
        /// </summary>
        public static class ToggleButton
        {
            public static string Description => "可以切换开关状态的按钮控件";
            public static string Category => ToolboxCategories.Buttons;
            public static string ExampleCode => "<ToggleButton Content=\"切换\" Style=\"{StaticResource Win11.ToggleButtonStyle}\" />";
        }

        #endregion
    }

    /// <summary>
    /// 控件工具箱信息接口
    /// </summary>
    public interface IControlToolboxInfo
    {
        /// <summary>
        /// 控件名称
        /// </summary>
        string ControlName { get; }

        /// <summary>
        /// 显示名称
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// 描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 分类
        /// </summary>
        string Category { get; }

        /// <summary>
        /// 示例XAML代码
        /// </summary>
        string ExampleCode { get; }

        /// <summary>
        /// 是否为自定义控件
        /// </summary>
        bool IsCustomControl { get; }

        /// <summary>
        /// XAML命名空间前缀
        /// </summary>
        string XamlNamespacePrefix { get; }

        /// <summary>
        /// 可用的样式变体
        /// </summary>
        string[] StyleVariants { get; }
    }

    /// <summary>
    /// 控件工具箱信息
    /// </summary>
    public class ControlToolboxInfo : IControlToolboxInfo
    {
        public string ControlName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ExampleCode { get; set; } = string.Empty;
        public bool IsCustomControl { get; set; }
        public string XamlNamespacePrefix { get; set; } = string.Empty;
        public string[] StyleVariants { get; set; } = Array.Empty<string>();
    }

    /// <summary>
    /// 工具箱信息提供者
    /// </summary>
    public static class ToolboxInfoProvider
    {
        /// <summary>
        /// 获取所有控件的信息
        /// </summary>
        public static IReadOnlyList<IControlToolboxInfo> GetAllControls()
        {
            var controls = new List<ControlToolboxInfo>();

            // 自定义控件
            controls.Add(new ControlToolboxInfo
            {
                ControlName = "Badge",
                DisplayName = "徽章",
                Description = "用于显示通知计数或状态指示",
                Category = ToolboxCategories.Buttons,
                ExampleCode = "<ui:Badge Content=\"5\" />",
                IsCustomControl = true,
                XamlNamespacePrefix = "ui"
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "DropDownButton",
                DisplayName = "下拉按钮",
                Description = "点击显示下拉菜单的按钮",
                Category = ToolboxCategories.Buttons,
                ExampleCode = "<ui:DropDownButton Content=\"选项\"><ui:DropDownButton.DropDownContent><StackPanel><Button Content=\"选项1\"/><Button Content=\"选项2\"/></StackPanel></ui:DropDownButton.DropDownContent></ui:DropDownButton>",
                IsCustomControl = true,
                XamlNamespacePrefix = "ui"
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "ToggleSwitch",
                DisplayName = "开关",
                Description = "现代化的开关控件",
                Category = ToolboxCategories.Buttons,
                ExampleCode = "<ui:ToggleSwitch IsChecked=\"True\" />",
                IsCustomControl = true,
                XamlNamespacePrefix = "ui"
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "NumericUpDown",
                DisplayName = "数字输入",
                Description = "专门用于数字输入的控件，带有增加/减少按钮",
                Category = ToolboxCategories.Inputs,
                ExampleCode = "<ui:NumericUpDown Value=\"10\" Minimum=\"0\" Maximum=\"100\" />",
                IsCustomControl = true,
                XamlNamespacePrefix = "ui"
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "SearchBox",
                DisplayName = "搜索框",
                Description = "带搜索图标的文本框",
                Category = ToolboxCategories.Inputs,
                ExampleCode = "<ui:SearchBox PlaceholderText=\"搜索...\" />",
                IsCustomControl = true,
                XamlNamespacePrefix = "ui"
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "Card",
                DisplayName = "卡片",
                Description = "用于展示内容的容器控件",
                Category = ToolboxCategories.Cards,
                ExampleCode = "<ui:Card Padding=\"16\"><TextBlock Text=\"卡片内容\"/></ui:Card>",
                IsCustomControl = true,
                XamlNamespacePrefix = "ui"
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "Expander",
                DisplayName = "折叠面板",
                Description = "可以展开/折叠内容的控件",
                Category = ToolboxCategories.Cards,
                ExampleCode = "<ui:Expander Header=\"标题\"><TextBlock Text=\"可展开内容\"/></ui:Expander>",
                IsCustomControl = true,
                XamlNamespacePrefix = "ui"
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "ProgressRing",
                DisplayName = "进度环",
                Description = "圆形的进度指示器",
                Category = ToolboxCategories.Progress,
                ExampleCode = "<ui:ProgressRing IsActive=\"True\" />",
                IsCustomControl = true,
                XamlNamespacePrefix = "ui"
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "DialogHost",
                DisplayName = "对话框宿主",
                Description = "在窗口内显示模态对话框",
                Category = ToolboxCategories.Dialogs,
                ExampleCode = "<ui:DialogHost IsOpen=\"True\"><ui:DialogHost.DialogContent><TextBlock Text=\"对话框内容\"/></ui:DialogHost.DialogContent></ui:DialogHost>",
                IsCustomControl = true,
                XamlNamespacePrefix = "ui"
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "Snackbar",
                DisplayName = "提示条",
                Description = "在屏幕底部显示的临时通知",
                Category = ToolboxCategories.Notifications,
                ExampleCode = "<ui:Snackbar Message=\"操作成功\" IsOpen=\"True\" />",
                IsCustomControl = true,
                XamlNamespacePrefix = "ui"
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "Sidebar",
                DisplayName = "侧边栏",
                Description = "左侧导航栏",
                Category = ToolboxCategories.Navigation,
                ExampleCode = "<ui:Sidebar />",
                IsCustomControl = true,
                XamlNamespacePrefix = "ui"
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "DocumentTabHost",
                DisplayName = "文档标签页",
                Description = "类似VS的文档标签页",
                Category = ToolboxCategories.Navigation,
                ExampleCode = "<ui:DocumentTabHost />",
                IsCustomControl = true,
                XamlNamespacePrefix = "ui"
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "MainTitleBar",
                DisplayName = "主标题栏",
                Description = "自定义窗口标题栏",
                Category = ToolboxCategories.Chrome,
                ExampleCode = "<ui:MainTitleBar Title=\"窗口标题\" />",
                IsCustomControl = true,
                XamlNamespacePrefix = "ui"
            });

            // 核心控件（带Win11样式）
            controls.Add(new ControlToolboxInfo
            {
                ControlName = "Button",
                DisplayName = "按钮",
                Description = CoreControlsToolboxRegistration.Button.Description,
                Category = CoreControlsToolboxRegistration.Button.Category,
                ExampleCode = CoreControlsToolboxRegistration.Button.ExampleCode,
                IsCustomControl = false,
                XamlNamespacePrefix = "",
                StyleVariants = CoreControlsToolboxRegistration.Button.StyleVariants
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "TextBox",
                DisplayName = "文本框",
                Description = CoreControlsToolboxRegistration.TextBox.Description,
                Category = CoreControlsToolboxRegistration.TextBox.Category,
                ExampleCode = CoreControlsToolboxRegistration.TextBox.ExampleCode,
                IsCustomControl = false,
                XamlNamespacePrefix = "",
                StyleVariants = CoreControlsToolboxRegistration.TextBox.StyleVariants
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "PasswordBox",
                DisplayName = "密码框",
                Description = CoreControlsToolboxRegistration.PasswordBox.Description,
                Category = CoreControlsToolboxRegistration.PasswordBox.Category,
                ExampleCode = CoreControlsToolboxRegistration.PasswordBox.ExampleCode,
                IsCustomControl = false,
                XamlNamespacePrefix = ""
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "CheckBox",
                DisplayName = "复选框",
                Description = CoreControlsToolboxRegistration.CheckBox.Description,
                Category = CoreControlsToolboxRegistration.CheckBox.Category,
                ExampleCode = CoreControlsToolboxRegistration.CheckBox.ExampleCode,
                IsCustomControl = false,
                XamlNamespacePrefix = ""
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "RadioButton",
                DisplayName = "单选按钮",
                Description = CoreControlsToolboxRegistration.RadioButton.Description,
                Category = CoreControlsToolboxRegistration.RadioButton.Category,
                ExampleCode = CoreControlsToolboxRegistration.RadioButton.ExampleCode,
                IsCustomControl = false,
                XamlNamespacePrefix = ""
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "ComboBox",
                DisplayName = "下拉框",
                Description = CoreControlsToolboxRegistration.ComboBox.Description,
                Category = CoreControlsToolboxRegistration.ComboBox.Category,
                ExampleCode = CoreControlsToolboxRegistration.ComboBox.ExampleCode,
                IsCustomControl = false,
                XamlNamespacePrefix = ""
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "ListBox",
                DisplayName = "列表框",
                Description = CoreControlsToolboxRegistration.ListBox.Description,
                Category = CoreControlsToolboxRegistration.ListBox.Category,
                ExampleCode = CoreControlsToolboxRegistration.ListBox.ExampleCode,
                IsCustomControl = false,
                XamlNamespacePrefix = ""
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "MenuItem",
                DisplayName = "菜单项",
                Description = CoreControlsToolboxRegistration.MenuItem.Description,
                Category = CoreControlsToolboxRegistration.MenuItem.Category,
                ExampleCode = CoreControlsToolboxRegistration.MenuItem.ExampleCode,
                IsCustomControl = false,
                XamlNamespacePrefix = ""
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "ProgressBar",
                DisplayName = "进度条",
                Description = CoreControlsToolboxRegistration.ProgressBar.Description,
                Category = CoreControlsToolboxRegistration.ProgressBar.Category,
                ExampleCode = CoreControlsToolboxRegistration.ProgressBar.ExampleCode,
                IsCustomControl = false,
                XamlNamespacePrefix = ""
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "Slider",
                DisplayName = "滑块",
                Description = CoreControlsToolboxRegistration.Slider.Description,
                Category = CoreControlsToolboxRegistration.Slider.Category,
                ExampleCode = CoreControlsToolboxRegistration.Slider.ExampleCode,
                IsCustomControl = false,
                XamlNamespacePrefix = ""
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "ScrollBar",
                DisplayName = "滚动条",
                Description = CoreControlsToolboxRegistration.ScrollBar.Description,
                Category = CoreControlsToolboxRegistration.ScrollBar.Category,
                ExampleCode = CoreControlsToolboxRegistration.ScrollBar.ExampleCode,
                IsCustomControl = false,
                XamlNamespacePrefix = ""
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "Separator",
                DisplayName = "分隔符",
                Description = CoreControlsToolboxRegistration.Separator.Description,
                Category = CoreControlsToolboxRegistration.Separator.Category,
                ExampleCode = CoreControlsToolboxRegistration.Separator.ExampleCode,
                IsCustomControl = false,
                XamlNamespacePrefix = ""
            });

            controls.Add(new ControlToolboxInfo
            {
                ControlName = "ToggleButton",
                DisplayName = "切换按钮",
                Description = CoreControlsToolboxRegistration.ToggleButton.Description,
                Category = CoreControlsToolboxRegistration.ToggleButton.Category,
                ExampleCode = CoreControlsToolboxRegistration.ToggleButton.ExampleCode,
                IsCustomControl = false,
                XamlNamespacePrefix = ""
            });

            return controls;
        }

        /// <summary>
        /// 根据分类获取控件信息
        /// </summary>
        public static IReadOnlyList<IControlToolboxInfo> GetControlsByCategory(string category)
        {
            return GetAllControls().Where(c => c.Category == category).ToList();
        }

        /// <summary>
        /// 搜索控件
        /// </summary>
        public static IReadOnlyList<IControlToolboxInfo> SearchControls(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAllControls();

            return GetAllControls().Where(c =>
                c.ControlName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                c.DisplayName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                c.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
