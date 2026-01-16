using System.Windows;
using System.Windows.Controls;
using Lemoo.UI.Models.Icons;

namespace Lemoo.UI.Controls.Icons
{
    /// <summary>
    /// 图标按钮变体样式
    /// </summary>
    public enum IconButtonVariant
    {
        /// <summary>
        /// 主要按钮
        /// </summary>
        Primary,

        /// <summary>
        /// 次要按钮
        /// </summary>
        Secondary,

        /// <summary>
        /// 轮廓按钮
        /// </summary>
        Outline,

        /// <summary>
        /// 幽灵按钮（透明背景）
        /// </summary>
        Ghost,

        /// <summary>
        /// 危险按钮
        /// </summary>
        Danger,

        /// <summary>
        /// 成功按钮
        /// </summary>
        Success
    }

    /// <summary>
    /// 图标按钮控件，结合图标和按钮功能
    /// </summary>
    public class IconButton : Button
    {
        /// <summary>
        /// 标识 IconKind 依赖属性
        /// </summary>
        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register(
                nameof(IconKind),
                typeof(IconKind),
                typeof(IconButton),
                new PropertyMetadata(IconKind.None, OnIconKindChanged));

        /// <summary>
        /// 标识 IconSize 依赖属性
        /// </summary>
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(
                nameof(IconSize),
                typeof(IconSize),
                typeof(IconButton),
                new PropertyMetadata(IconSize.Normal));

        /// <summary>
        /// 标识 IconVariant 依赖属性
        /// </summary>
        public static readonly DependencyProperty IconVariantProperty =
            DependencyProperty.Register(
                nameof(IconVariant),
                typeof(IconVariant),
                typeof(IconButton),
                new PropertyMetadata(IconVariant.Regular));

        /// <summary>
        /// 标识 ButtonVariant 依赖属性
        /// </summary>
        public static readonly DependencyProperty ButtonVariantProperty =
            DependencyProperty.Register(
                nameof(ButtonVariant),
                typeof(IconButtonVariant),
                typeof(IconButton),
                new PropertyMetadata(IconButtonVariant.Primary));

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

        /// <summary>
        /// 获取或设置按钮变体样式
        /// </summary>
        public IconButtonVariant ButtonVariant
        {
            get => (IconButtonVariant)GetValue(ButtonVariantProperty);
            set => SetValue(ButtonVariantProperty, value);
        }

        private static void OnIconKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IconButton button)
            {
                button.InvalidateVisual();
            }
        }

        static IconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconButton),
                new FrameworkPropertyMetadata(typeof(IconButton)));
        }
    }
}
