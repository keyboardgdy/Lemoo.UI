using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 时间线控件，用于展示时间序列数据。
    /// </summary>
    /// <remarks>
    /// TimelineView 是一个优雅的时间线控件，支持垂直/水平布局、
    /// 自定义节点样式、连接线动画和节点状态指示。
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;!-- 基础用法 --&gt;
    /// &lt;ui:TimelineView ItemsSource="{Binding TimelineItems}" /&gt;
    ///
    /// &lt;!-- 水平方向 --&gt;
    /// &lt;ui:TimelineView
    ///     Orientation="Horizontal"
    ///     ItemsSource="{Binding TimelineItems}" /&gt;
    ///
    /// &lt;!-- 自定义节点样式 --&gt;
    /// &lt;ui:TimelineView
    ///     NodeSize="32"
    ///     LineStyle="Animated"
    ///     ItemsSource="{Binding TimelineItems}"&gt;
    ///     &lt;ui:TimelineView.ItemTemplate&gt;
    ///         &lt;DataTemplate&gt;
    ///             &lt;StackPanel&gt;
    ///                 &lt;TextBlock Text="{Binding Title}" FontWeight="Bold" /&gt;
    ///                 &lt;TextBlock Text="{Binding Description}" /&gt;
    ///             &lt;/StackPanel&gt;
    ///         &lt;/DataTemplate&gt;
    ///     &lt;/ui:TimelineView.ItemTemplate&gt;
    /// &lt;/ui:TimelineView&gt;
    /// </code>
    /// </example>
    [TemplatePart(Name = PART_PanelsHost, Type = typeof(Panel))]
    public class TimelineView : ItemsControl
    {
        #region 常量

        private const string PART_PanelsHost = "PART_PanelsHost";

        #endregion

        #region Constructor

        static TimelineView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineView),
                new FrameworkPropertyMetadata(typeof(TimelineView)));
        }

        public TimelineView()
        {
            this.Loaded += OnLoaded;
        }

        #endregion

        #region Orientation 依赖属性

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(TimelineOrientation),
                typeof(TimelineView),
                new PropertyMetadata(TimelineOrientation.Vertical, OnOrientationChanged));

        /// <summary>
        /// 获取或设置时间线方向。
        /// </summary>
        public TimelineOrientation Orientation
        {
            get => (TimelineOrientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TimelineView timelineView)
            {
                timelineView.UpdateItemsPanel();
            }
        }

        #endregion

        #region NodeSize 依赖属性

        public static readonly DependencyProperty NodeSizeProperty =
            DependencyProperty.Register(
                nameof(NodeSize),
                typeof(double),
                typeof(TimelineView),
                new PropertyMetadata(24.0));

        /// <summary>
        /// 获取或设置节点大小。
        /// </summary>
        /// <remarks>
        /// 默认值为 24px，有效范围为 16px - 48px。
        /// </remarks>
        public double NodeSize
        {
            get => (double)GetValue(NodeSizeProperty);
            set => SetValue(NodeSizeProperty, value);
        }

        #endregion

        #region LineStyle 依赖属性

        public static readonly DependencyProperty LineStyleProperty =
            DependencyProperty.Register(
                nameof(LineStyle),
                typeof(TimelineLineStyle),
                typeof(TimelineView),
                new PropertyMetadata(TimelineLineStyle.Solid));

        /// <summary>
        /// 获取或设置连接线样式。
        /// </summary>
        public TimelineLineStyle LineStyle
        {
            get => (TimelineLineStyle)GetValue(LineStyleProperty);
            set => SetValue(LineStyleProperty, value);
        }

        #endregion

        #region LineStroke 依赖属性

        public static readonly DependencyProperty LineStrokeProperty =
            DependencyProperty.Register(
                nameof(LineStroke),
                typeof(System.Windows.Media.Brush),
                typeof(TimelineView),
                new PropertyMetadata(System.Windows.Media.Brushes.Gray));

        /// <summary>
        /// 获取或设置连接线画刷。
        /// </summary>
        public System.Windows.Media.Brush LineStroke
        {
            get => (System.Windows.Media.Brush)GetValue(LineStrokeProperty);
            set => SetValue(LineStrokeProperty, value);
        }

        #endregion

        #region LineStrokeThickness 依赖属性

        public static readonly DependencyProperty LineStrokeThicknessProperty =
            DependencyProperty.Register(
                nameof(LineStrokeThickness),
                typeof(double),
                typeof(TimelineView),
                new PropertyMetadata(2.0));

        /// <summary>
        /// 获取或设置连接线粗细。
        /// </summary>
        public double LineStrokeThickness
        {
            get => (double)GetValue(LineStrokeThicknessProperty);
            set => SetValue(LineStrokeThicknessProperty, value);
        }

        #endregion

        #region IsLineAnimationEnabled 依赖属性

        public static readonly DependencyProperty IsLineAnimationEnabledProperty =
            DependencyProperty.Register(
                nameof(IsLineAnimationEnabled),
                typeof(bool),
                typeof(TimelineView),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否启用连接线动画。
        /// </summary>
        public bool IsLineAnimationEnabled
        {
            get => (bool)GetValue(IsLineAnimationEnabledProperty);
            set => SetValue(IsLineAnimationEnabledProperty, value);
        }

        #endregion

        #region HeaderPath 依赖属性

        public static readonly DependencyProperty HeaderPathProperty =
            DependencyProperty.Register(
                nameof(HeaderPath),
                typeof(string),
                typeof(TimelineView),
                new PropertyMetadata("Header"));

        /// <summary>
        /// 获取或设置标题属性路径。
        /// </summary>
        public string HeaderPath
        {
            get => (string)GetValue(HeaderPathProperty);
            set => SetValue(HeaderPathProperty, value);
        }

        #endregion

        #region DateTimePath 依赖属性

        public static readonly DependencyProperty DateTimePathProperty =
            DependencyProperty.Register(
                nameof(DateTimePath),
                typeof(string),
                typeof(TimelineView),
                new PropertyMetadata("DateTime"));

        /// <summary>
        /// 获取或设置日期时间属性路径。
        /// </summary>
        public string DateTimePath
        {
            get => (string)GetValue(DateTimePathProperty);
            set => SetValue(DateTimePathProperty, value);
        }

        #endregion

        #region StatePath 依赖属性

        public static readonly DependencyProperty StatePathProperty =
            DependencyProperty.Register(
                nameof(StatePath),
                typeof(string),
                typeof(TimelineView),
                new PropertyMetadata("State"));

        /// <summary>
        /// 获取或设置状态属性路径。
        /// </summary>
        public string StatePath
        {
            get => (string)GetValue(StatePathProperty);
            set => SetValue(StatePathProperty, value);
        }

        #endregion

        #region IconPath 依赖属性

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register(
                nameof(IconPath),
                typeof(string),
                typeof(TimelineView),
                new PropertyMetadata("Icon"));

        /// <summary>
        /// 获取或设置图标属性路径。
        /// </summary>
        public string IconPath
        {
            get => (string)GetValue(IconPathProperty);
            set => SetValue(IconPathProperty, value);
        }

        #endregion

        #region 事件

        public static readonly RoutedEvent NodeClickEvent =
            EventManager.RegisterRoutedEvent(
                nameof(NodeClick),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TimelineView));

        /// <summary>
        /// 在节点被点击时发生。
        /// </summary>
        public event RoutedEventHandler NodeClick
        {
            add => AddHandler(NodeClickEvent, value);
            remove => RemoveHandler(NodeClickEvent, value);
        }

        #endregion

        #region 方法

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateItemsPanel();
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            UpdateItemsPanel();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateItemsPanel();
        }

        private void UpdateItemsPanel()
        {
            // 根据 Orientation 设置 ItemsPanel
            var itemsPanelTemplate = new ItemsPanelTemplate();
            var frameworkElementFactory = new FrameworkElementFactory(typeof(StackPanel));

            if (Orientation == TimelineOrientation.Vertical)
            {
                frameworkElementFactory.SetValue(StackPanel.OrientationProperty, System.Windows.Controls.Orientation.Vertical);
            }
            else
            {
                frameworkElementFactory.SetValue(StackPanel.OrientationProperty, System.Windows.Controls.Orientation.Horizontal);
            }

            itemsPanelTemplate.VisualTree = frameworkElementFactory;
            ItemsPanel = itemsPanelTemplate;
        }

        /// <summary>
        /// 触发节点点击事件。
        /// </summary>
        internal void RaiseNodeClick(object node)
        {
            RaiseEvent(new RoutedEventArgs(NodeClickEvent, node));
        }

        #endregion
    }
}
