using System.Windows;
using System.Windows.Controls;
using Lemoo.UI.Models.Icons;

namespace Lemoo.UI.Controls.Icons
{
    /// <summary>
    /// 图标控件，使用 Segoe Fluent Icons 字体显示图标
    /// </summary>
    public class Icon : Control
    {
        /// <summary>
        /// 标识 IconKind 依赖属性
        /// </summary>
        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register(
                nameof(IconKind),
                typeof(IconKind),
                typeof(Icon),
                new PropertyMetadata(IconKind.None, OnIconKindChanged));

        /// <summary>
        /// 标识 IconSize 依赖属性
        /// </summary>
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(
                nameof(IconSize),
                typeof(IconSize),
                typeof(Icon),
                new PropertyMetadata(IconSize.Normal, OnIconSizeChanged));

        /// <summary>
        /// 标识 IconVariant 依赖属性
        /// </summary>
        public static readonly DependencyProperty IconVariantProperty =
            DependencyProperty.Register(
                nameof(IconVariant),
                typeof(IconVariant),
                typeof(Icon),
                new PropertyMetadata(IconVariant.Regular));

        /// <summary>
        /// 获取或设置图标类型
        /// </summary>
        public IconKind IconKind
        {
            get => (IconKind)GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        /// <summary>
        /// 获取或设置图标大小
        /// </summary>
        public IconSize IconSize
        {
            get => (IconSize)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        /// <summary>
        /// 获取或设置图标变体
        /// </summary>
        public IconVariant IconVariant
        {
            get => (IconVariant)GetValue(IconVariantProperty);
            set => SetValue(IconVariantProperty, value);
        }

        private static void OnIconKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Icon icon)
            {
                // 触发重新渲染
                icon.InvalidateVisual();
            }
        }

        private static void OnIconSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Icon icon)
            {
                // 触发重新渲染
                icon.InvalidateVisual();
            }
        }

        static Icon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Icon),
                new FrameworkPropertyMetadata(typeof(Icon)));
        }
    }
}
