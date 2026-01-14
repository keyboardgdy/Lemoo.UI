using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 开关控件，用于二进制状态切换。
    /// </summary>
    /// <remarks>
    /// ToggleSwitch 是一种现代化的开关控件，提供直观的开关交互体验。
    /// </remarks>
    /// <example>
    /// <code>
    /// <!-- 基础用法 -->
    /// <ui:ToggleSwitch Header="无线网络" IsChecked="{Binding IsWifiEnabled}" />
    ///
    /// <!-- 带标签文本 -->
    /// <ui:ToggleSwitch
    ///     Header="飞行模式"
    ///     OnLabel="开"
    ///     OffLabel="关"
    ///     IsChecked="{Binding IsAirplaneModeEnabled}" />
    ///
    /// <!-- 禁用状态 -->
    /// <ui:ToggleSwitch Header="蓝牙" IsEnabled="False" />
    /// </code>
    /// </example>
    public class ToggleSwitch : ContentControl
    {
        #region Constructor

        static ToggleSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleSwitch),
                new FrameworkPropertyMetadata(typeof(ToggleSwitch)));
        }

        public ToggleSwitch()
        {
        }

        #endregion

        #region IsChecked 依赖属性

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(
                nameof(IsChecked),
                typeof(bool?),
                typeof(ToggleSwitch),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsCheckedChanged));

        /// <summary>
        /// 获取或设置开关是否已选中。
        /// </summary>
        public bool? IsChecked
        {
            get => (bool?)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toggle = (ToggleSwitch)d;
            toggle.OnToggle();
        }

        #endregion

        #region Header 依赖属性

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                nameof(Header),
                typeof(object),
                typeof(ToggleSwitch),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置开关头部内容。
        /// </summary>
        public object? Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        #endregion

        #region OnLabel 依赖属性

        public static readonly DependencyProperty OnLabelProperty =
            DependencyProperty.Register(
                nameof(OnLabel),
                typeof(string),
                typeof(ToggleSwitch),
                new PropertyMetadata("开"));

        /// <summary>
        /// 获取或设置开启时的标签文本。
        /// </summary>
        public string OnLabel
        {
            get => (string)GetValue(OnLabelProperty);
            set => SetValue(OnLabelProperty, value);
        }

        #endregion

        #region OffLabel 依赖属性

        public static readonly DependencyProperty OffLabelProperty =
            DependencyProperty.Register(
                nameof(OffLabel),
                typeof(string),
                typeof(ToggleSwitch),
                new PropertyMetadata("关"));

        /// <summary>
        /// 获取或设置关闭时的标签文本。
        /// </summary>
        public string OffLabel
        {
            get => (string)GetValue(OffLabelProperty);
            set => SetValue(OffLabelProperty, value);
        }

        #endregion

        #region Command 依赖属性

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(ToggleSwitch),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置开关状态改变时执行的命令。
        /// </summary>
        public ICommand? Command
        {
            get => (ICommand?)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        #endregion

        #region CommandParameter 依赖属性

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                nameof(CommandParameter),
                typeof(object),
                typeof(ToggleSwitch),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置命令参数。
        /// </summary>
        public object? CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        #endregion

        #region 事件

        public static readonly RoutedEvent CheckedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(Checked),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ToggleSwitch));

        /// <summary>
        /// 在开关被选中时发生。
        /// </summary>
        public event RoutedEventHandler Checked
        {
            add => AddHandler(CheckedEvent, value);
            remove => RemoveHandler(CheckedEvent, value);
        }

        public static readonly RoutedEvent UncheckedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(Unchecked),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ToggleSwitch));

        /// <summary>
        /// 在开关取消选中时发生。
        /// </summary>
        public event RoutedEventHandler Unchecked
        {
            add => AddHandler(UncheckedEvent, value);
            remove => RemoveHandler(UncheckedEvent, value);
        }

        #endregion

        #region 方法

        private void OnToggle()
        {
            var isChecked = IsChecked == true;

            if (isChecked)
            {
                RaiseEvent(new RoutedEventArgs(CheckedEvent, this));
            }
            else
            {
                RaiseEvent(new RoutedEventArgs(UncheckedEvent, this));
            }

            if (Command != null && Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);
            }
        }

        #endregion
    }
}
