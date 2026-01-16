using System.Windows;
using System.Windows.Controls;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 环形进度条控件，用于显示加载状态。
    /// </summary>
    /// <remarks>
    /// ProgressRing 是一种现代化的进度指示器，提供流畅的动画效果。
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;!-- 确定性进度 --&gt;
    /// &lt;ui:ProgressRing Value="50" Maximum="100" /&gt;
    ///
    /// &lt;!-- 不确定性进度（加载动画） --&gt;
    /// &lt;ui:ProgressRing IsIndeterminate="True" /&gt;
    ///
    /// &lt;!-- 自定义大小 --&gt;
    /// &lt;ui:ProgressRing Width="100" Height="100" Value="75" /&gt;
    ///
    /// &lt;!-- 自定义颜色 --&gt;
    /// &lt;ui:ProgressRing
    ///     Value="60"
    ///     Foreground="{DynamicResource Brush.Primary}" /&gt;
    /// </code>
    /// </example>
    public class ProgressRing : ProgressBar
    {
        #region Constructor

        static ProgressRing()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressRing),
                new FrameworkPropertyMetadata(typeof(ProgressRing)));

            // 默认大小
            WidthProperty.OverrideMetadata(typeof(ProgressRing),
                new FrameworkPropertyMetadata(40.0));
            HeightProperty.OverrideMetadata(typeof(ProgressRing),
                new FrameworkPropertyMetadata(40.0));
        }

        public ProgressRing()
        {
        }

        #endregion

        #region RingThickness 依赖属性

        public static readonly DependencyProperty RingThicknessProperty =
            DependencyProperty.Register(
                nameof(RingThickness),
                typeof(double),
                typeof(ProgressRing),
                new PropertyMetadata(4.0));

        /// <summary>
        /// 获取或设置环的粗细。
        /// </summary>
        public double RingThickness
        {
            get => (double)GetValue(RingThicknessProperty);
            set => SetValue(RingThicknessProperty, value);
        }

        #endregion

        #region IsIndeterminate 依赖属性 (override to add metadata change callback)

        public new static readonly DependencyProperty IsIndeterminateProperty =
            DependencyProperty.Register(
                nameof(IsIndeterminate),
                typeof(bool),
                typeof(ProgressRing),
                new PropertyMetadata(false, OnIsIndeterminateChanged));

        private static void OnIsIndeterminateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ring = (ProgressRing)d;
            ring.UpdateVisualState(true);
        }

        /// <summary>
        /// 获取或设置进度是否为不确定状态。
        /// </summary>
        public new bool IsIndeterminate
        {
            get => (bool)GetValue(IsIndeterminateProperty);
            set => SetValue(IsIndeterminateProperty, value);
        }

        #endregion

        #region ShowPercentage 依赖属性

        public static readonly DependencyProperty ShowPercentageProperty =
            DependencyProperty.Register(
                nameof(ShowPercentage),
                typeof(bool),
                typeof(ProgressRing),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否显示百分比文本。
        /// </summary>
        public bool ShowPercentage
        {
            get => (bool)GetValue(ShowPercentageProperty);
            set => SetValue(ShowPercentageProperty, value);
        }

        #endregion

        #region 方法

        private void UpdateVisualState(bool useTransitions)
        {
            if (IsIndeterminate)
            {
                VisualStateManager.GoToState(this, "Indeterminate", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Determinate", useTransitions);
            }
        }

        #endregion
    }
}
