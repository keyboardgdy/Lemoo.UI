using System.Windows;
using System.Windows.Media;
using Lemoo.UI.Models.Icons;

namespace Lemoo.UI.Controls.Icons
{
    /// <summary>
    /// 图标徽章控件，在图标上显示徽章通知
    /// </summary>
    public class IconBadge : Icon
    {
        /// <summary>
        /// 标识 BadgeValue 依赖属性
        /// </summary>
        public static readonly DependencyProperty BadgeValueProperty =
            DependencyProperty.Register(
                nameof(BadgeValue),
                typeof(int),
                typeof(IconBadge),
                new PropertyMetadata(0, OnBadgeValueChanged));

        /// <summary>
        /// 标识 BadgeBackground 依赖属性
        /// </summary>
        public static readonly DependencyProperty BadgeBackgroundProperty =
            DependencyProperty.Register(
                nameof(BadgeBackground),
                typeof(Brush),
                typeof(IconBadge),
                new PropertyMetadata(Brushes.Red));

        /// <summary>
        /// 标识 BadgeForeground 依赖属性
        /// </summary>
        public static readonly DependencyProperty BadgeForegroundProperty =
            DependencyProperty.Register(
                nameof(BadgeForeground),
                typeof(Brush),
                typeof(IconBadge),
                new PropertyMetadata(Brushes.White));

        /// <summary>
        /// 标识 ShowDot 依赖属性，是否仅显示点徽章
        /// </summary>
        public static readonly DependencyProperty ShowDotProperty =
            DependencyProperty.Register(
                nameof(ShowDot),
                typeof(bool),
                typeof(IconBadge),
                new PropertyMetadata(false));

        /// <summary>
        /// 标识 BadgePlacement 依赖属性，徽章位置
        /// </summary>
        public static readonly DependencyProperty BadgePlacementProperty =
            DependencyProperty.Register(
                nameof(BadgePlacement),
                typeof(BadgePlacement),
                typeof(IconBadge),
                new PropertyMetadata(BadgePlacement.TopRight));

        /// <summary>
        /// 获取或设置徽章值
        /// </summary>
        public int BadgeValue
        {
            get => (int)GetValue(BadgeValueProperty);
            set => SetValue(BadgeValueProperty, value);
        }

        /// <summary>
        /// 获取或设置徽章背景色
        /// </summary>
        public Brush BadgeBackground
        {
            get => (Brush)GetValue(BadgeBackgroundProperty);
            set => SetValue(BadgeBackgroundProperty, value);
        }

        /// <summary>
        /// 获取或设置徽章前景色
        /// </summary>
        public Brush BadgeForeground
        {
            get => (Brush)GetValue(BadgeForegroundProperty);
            set => SetValue(BadgeForegroundProperty, value);
        }

        /// <summary>
        /// 获取或设置是否仅显示点徽章
        /// </summary>
        public bool ShowDot
        {
            get => (bool)GetValue(ShowDotProperty);
            set => SetValue(ShowDotProperty, value);
        }

        /// <summary>
        /// 获取或设置徽章位置
        /// </summary>
        public BadgePlacement BadgePlacement
        {
            get => (BadgePlacement)GetValue(BadgePlacementProperty);
            set => SetValue(BadgePlacementProperty, value);
        }

        private static void OnBadgeValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IconBadge badge)
            {
                badge.InvalidateVisual();
            }
        }

        static IconBadge()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconBadge),
                new FrameworkPropertyMetadata(typeof(IconBadge)));
        }
    }

    /// <summary>
    /// 徽章位置枚举
    /// </summary>
    public enum BadgePlacement
    {
        /// <summary>
        /// 左上角
        /// </summary>
        TopLeft,

        /// <summary>
        /// 右上角
        /// </summary>
        TopRight,

        /// <summary>
        /// 左下角
        /// </summary>
        BottomLeft,

        /// <summary>
        /// 右下角
        /// </summary>
        BottomRight
    }
}
