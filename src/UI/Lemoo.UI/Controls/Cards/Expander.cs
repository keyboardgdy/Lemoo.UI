using System.Windows;
using System.Windows.Controls;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 扩展器控件，用于可展开/折叠的内容区域。
    /// </summary>
    /// <remarks>
    /// Expander 允许用户通过点击头部来展开或折叠内容区域。
    /// </remarks>
    /// <example>
    /// <code>
    /// <!-- 基础用法 -->
    /// <ui:Expander Header="详细信息">
    ///     <TextBlock Text="这是展开的内容" />
    /// </ui:Expander>
    ///
    /// <!-- 默认展开 -->
    /// <ui:Expander Header="选项" IsExpanded="True">
    ///     <StackPanel>
    ///         <CheckBox Content="选项 1" />
    ///         <CheckBox Content="选项 2" />
    ///     </StackPanel>
    /// </ui:Expander>
    ///
    /// <!-- 自定义展开方向 -->
    /// <ui:Expander Header="向右展开" ExpandDirection="Right">
    ///     <TextBlock Text="从左侧展开的内容" />
    /// </ui:Expander>
    /// </code>
    /// </example>
    public class Expander : HeaderedContentControl
    {
        #region Constructor

        static Expander()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Expander),
                new FrameworkPropertyMetadata(typeof(Expander)));
        }

        public Expander()
        {
        }

        #endregion

        #region IsExpanded 依赖属性

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(
                nameof(IsExpanded),
                typeof(bool),
                typeof(Expander),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsExpandedChanged));

        /// <summary>
        /// 获取或设置内容区域是否展开。
        /// </summary>
        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var expander = (Expander)d;
            if ((bool)e.NewValue)
            {
                expander.OnExpanded();
            }
            else
            {
                expander.OnCollapsed();
            }
        }

        #endregion

        #region ExpandDirection 依赖属性

        public static readonly DependencyProperty ExpandDirectionProperty =
            DependencyProperty.Register(
                nameof(ExpandDirection),
                typeof(ExpandDirection),
                typeof(Expander),
                new PropertyMetadata(ExpandDirection.Down));

        /// <summary>
        /// 获取或设置内容展开的方向。
        /// </summary>
        public ExpandDirection ExpandDirection
        {
            get => (ExpandDirection)GetValue(ExpandDirectionProperty);
            set => SetValue(ExpandDirectionProperty, value);
        }

        #endregion

        #region Header 依赖属性 (override)

        public new static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                nameof(Header),
                typeof(object),
                typeof(Expander),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置扩展器头部内容。
        /// </summary>
        public new object? Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        #endregion

        #region 事件

        public static readonly RoutedEvent ExpandedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(Expanded),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(Expander));

        /// <summary>
        /// 在内容区域展开时发生。
        /// </summary>
        public event RoutedEventHandler Expanded
        {
            add => AddHandler(ExpandedEvent, value);
            remove => RemoveHandler(ExpandedEvent, value);
        }

        public static readonly RoutedEvent CollapsedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(Collapsed),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(Expander));

        /// <summary>
        /// 在内容区域折叠时发生。
        /// </summary>
        public event RoutedEventHandler Collapsed
        {
            add => AddHandler(CollapsedEvent, value);
            remove => RemoveHandler(CollapsedEvent, value);
        }

        #endregion

        #region 方法

        protected virtual void OnExpanded()
        {
            RaiseEvent(new RoutedEventArgs(ExpandedEvent, this));
        }

        protected virtual void OnCollapsed()
        {
            RaiseEvent(new RoutedEventArgs(CollapsedEvent, this));
        }

        #endregion
    }
}
