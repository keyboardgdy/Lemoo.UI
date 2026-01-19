using System;
using System.Windows;
using System.Windows.Media;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 时间线节点状态枚举。
    /// </summary>
    public enum TimelineNodeState
    {
        /// <summary>
        /// 待处理状态 - 灰色圆点
        /// </summary>
        Pending,

        /// <summary>
        /// 进行中状态 - 蓝色旋转动画
        /// </summary>
        InProgress,

        /// <summary>
        /// 已完成状态 - 绿色勾选
        /// </summary>
        Completed,

        /// <summary>
        /// 错误状态 - 红色感叹号
        /// </summary>
        Error,

        /// <summary>
        /// 警告状态 - 橙色警告
        /// </summary>
        Warning
    }

    /// <summary>
    /// 时间线方向枚举。
    /// </summary>
    public enum TimelineOrientation
    {
        /// <summary>
        /// 垂直方向
        /// </summary>
        Vertical,

        /// <summary>
        /// 水平方向
        /// </summary>
        Horizontal
    }

    /// <summary>
    /// 连接线样式枚举。
    /// </summary>
    public enum TimelineLineStyle
    {
        /// <summary>
        /// 实线
        /// </summary>
        Solid,

        /// <summary>
        /// 虚线
        /// </summary>
        Dashed,

        /// <summary>
        /// 点线
        /// </summary>
        Dotted,

        /// <summary>
        /// 动画效果
        /// </summary>
        Animated
    }

    /// <summary>
    /// 时间线节点数据模型。
    /// </summary>
    /// <remarks>
    /// 用于定义时间线上单个节点的数据。
    /// </remarks>
    public class TimelineNode : DependencyObject
    {
        #region Content 依赖属性

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                nameof(Content),
                typeof(object),
                typeof(TimelineNode),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置节点内容。
        /// </summary>
        public object? Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        #endregion

        #region Header 依赖属性

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                nameof(Header),
                typeof(object),
                typeof(TimelineNode),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置节点标题。
        /// </summary>
        public object? Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        #endregion

        #region DateTime 依赖属性

        public static readonly DependencyProperty DateTimeProperty =
            DependencyProperty.Register(
                nameof(DateTime),
                typeof(DateTime),
                typeof(TimelineNode),
                new PropertyMetadata(DateTime.MinValue));

        /// <summary>
        /// 获取或设置节点日期时间。
        /// </summary>
        public DateTime DateTime
        {
            get => (DateTime)GetValue(DateTimeProperty);
            set => SetValue(DateTimeProperty, value);
        }

        #endregion

        #region State 依赖属性

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(
                nameof(State),
                typeof(TimelineNodeState),
                typeof(TimelineNode),
                new PropertyMetadata(TimelineNodeState.Pending));

        /// <summary>
        /// 获取或设置节点状态。
        /// </summary>
        public TimelineNodeState State
        {
            get => (TimelineNodeState)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        #endregion

        #region Icon 依赖属性

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                nameof(Icon),
                typeof(object),
                typeof(TimelineNode),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置节点图标。
        /// </summary>
        public object? Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        #endregion

        #region IsEnabled 依赖属性

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(
                nameof(IsEnabled),
                typeof(bool),
                typeof(TimelineNode),
                new PropertyMetadata(true));

        /// <summary>
        /// 获取或设置节点是否启用。
        /// </summary>
        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        #endregion

        #region Tag 依赖属性

        public static readonly DependencyProperty TagProperty =
            DependencyProperty.Register(
                nameof(Tag),
                typeof(object),
                typeof(TimelineNode),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置节点的自定义数据。
        /// </summary>
        public object? Tag
        {
            get => GetValue(TagProperty);
            set => SetValue(TagProperty, value);
        }

        #endregion
    }
}
