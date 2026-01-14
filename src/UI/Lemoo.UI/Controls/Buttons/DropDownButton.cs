using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 下拉按钮控件。
    /// </summary>
    /// <remarks>
    /// DropDownButton 是一种带有下拉菜单的按钮控件。
    /// </remarks>
    /// <example>
    /// <code>
    /// <!-- 基础用法 -->
    /// <ui:DropDownButton Content="操作">
    ///     <ui:DropDownButton.DropDownContent>
    ///         <ContextMenu>
    ///             <MenuItem Header="新建" Command="{Binding NewCommand}" />
    ///             <MenuItem Header="打开" Command="{Binding OpenCommand}" />
    ///             <Separator />
    ///             <MenuItem Header="退出" Command="{Binding ExitCommand}" />
    ///         </ContextMenu>
    ///     </ui:DropDownButton.DropDownContent>
    /// </ui:DropDownButton>
    ///
    /// <!-- 使用工具条菜单 -->
    /// <ui:DropDownButton Content="保存">
    ///     <ui:DropDownButton.DropDownContent>
    ///         <Menu>
    ///             <MenuItem Header="保存" Command="{Binding SaveCommand}" />
    ///             <MenuItem Header="另存为" Command="{Binding SaveAsCommand}" />
    ///         </Menu>
    ///     </ui:DropDownButton.DropDownContent>
    /// </ui:DropDownButton>
    /// </code>
    /// </example>
    public class DropDownButton : Button
    {
        #region Constructor

        static DropDownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton),
                new FrameworkPropertyMetadata(typeof(DropDownButton)));
        }

        public DropDownButton()
        {
        }

        #endregion

        #region DropDownContent 依赖属性

        public static readonly DependencyProperty DropDownContentProperty =
            DependencyProperty.Register(
                nameof(DropDownContent),
                typeof(object),
                typeof(DropDownButton),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置下拉内容。
        /// </summary>
        public object? DropDownContent
        {
            get => GetValue(DropDownContentProperty);
            set => SetValue(DropDownContentProperty, value);
        }

        #endregion

        #region IsDropDownOpen 依赖属性

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(
                nameof(IsDropDownOpen),
                typeof(bool),
                typeof(DropDownButton),
                new PropertyMetadata(false, OnIsDropDownOpenChanged));

        /// <summary>
        /// 获取或设置下拉菜单是否打开。
        /// </summary>
        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (DropDownButton)d;
            if ((bool)e.NewValue)
            {
                button.OnDropDownOpened();
            }
            else
            {
                button.OnDropDownClosed();
            }
        }

        #endregion

        #region Placement 依赖属性

        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register(
                nameof(Placement),
                typeof(PlacementMode),
                typeof(DropDownButton),
                new PropertyMetadata(PlacementMode.Bottom));

        /// <summary>
        /// 获取或设置下拉菜单位置。
        /// </summary>
        public PlacementMode Placement
        {
            get => (PlacementMode)GetValue(PlacementProperty);
            set => SetValue(PlacementProperty, value);
        }

        #endregion

        #region PlacementTarget 依赖属性

        public static readonly DependencyProperty PlacementTargetProperty =
            DependencyProperty.Register(
                nameof(PlacementTarget),
                typeof(UIElement),
                typeof(DropDownButton),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置下拉菜单的定位目标。
        /// </summary>
        public UIElement? PlacementTarget
        {
            get => (UIElement?)GetValue(PlacementTargetProperty);
            set => SetValue(PlacementTargetProperty, value);
        }

        #endregion

        #region 事件

        public static readonly RoutedEvent DropDownOpenedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(DropDownOpened),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(DropDownButton));

        /// <summary>
        /// 在下拉菜单打开时发生。
        /// </summary>
        public event RoutedEventHandler DropDownOpened
        {
            add => AddHandler(DropDownOpenedEvent, value);
            remove => RemoveHandler(DropDownOpenedEvent, value);
        }

        public static readonly RoutedEvent DropDownClosedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(DropDownClosed),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(DropDownButton));

        /// <summary>
        /// 在下拉菜单关闭时发生。
        /// </summary>
        public event RoutedEventHandler DropDownClosed
        {
            add => AddHandler(DropDownClosedEvent, value);
            remove => RemoveHandler(DropDownClosedEvent, value);
        }

        #endregion

        #region 方法

        protected virtual void OnDropDownOpened()
        {
            RaiseEvent(new RoutedEventArgs(DropDownOpenedEvent, this));
        }

        protected virtual void OnDropDownClosed()
        {
            RaiseEvent(new RoutedEventArgs(DropDownClosedEvent, this));
        }

        protected override void OnClick()
        {
            ToggleDropDown();
            base.OnClick();
        }

        private void ToggleDropDown()
        {
            IsDropDownOpen = !IsDropDownOpen;
        }

        #endregion
    }
}
