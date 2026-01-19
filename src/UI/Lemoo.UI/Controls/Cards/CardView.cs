using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 卡片阴影深度枚举,定义不同的阴影强度。
    /// </summary>
    public enum CardElevation
    {
        /// <summary>
        /// 无阴影
        /// </summary>
        Flat = 0,

        /// <summary>
        /// 轻微阴影
        /// </summary>
        Low = 1,

        /// <summary>
        /// 标准阴影
        /// </summary>
        Medium = 2,

        /// <summary>
        /// 深度阴影
        /// </summary>
        High = 3,

        /// <summary>
        /// 浮动效果
        /// </summary>
        Float = 4,

        /// <summary>
        /// 弹出效果
        /// </summary>
        Popup = 5
    }

    /// <summary>
    /// CardView 是现代化的卡片容器控件,支持多种阴影深度、圆角样式和悬停效果。
    /// </summary>
    /// <remarks>
    /// CardView 扩展了基础卡片功能,提供:
    /// - 动态阴影深度(Elevation 0-5)
    /// - 可配置圆角半径(4px - 24px)
    /// - 内置悬停/按压动画
    /// - 支持卡片堆叠和展开模式
    /// - 响应式布局
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;!-- 基础用法 --&gt;
    /// &lt;ui:CardView Header="标题" Elevation="Medium"&gt;
    ///     &lt;TextBlock Text="内容" /&gt;
    /// &lt;/ui:CardView&gt;
    ///
    /// &lt;!-- 带悬停效果 --&gt;
    /// &lt;ui:CardView Header="可交互卡片"
    ///              Elevation="High"
    ///              EnableHoverAnimation="True"
    ///              ClickMode="Press"&gt;
    ///     &lt;TextBlock Text="悬停时会有动画效果" /&gt;
    /// &lt;/ui:CardView&gt;
    /// </code>
    /// </example>
    public class CardView : ContentControl
    {
        #region Constructor

        static CardView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CardView),
                new FrameworkPropertyMetadata(typeof(CardView)));
        }

        public CardView()
        {
            // 默认启用悬停动画
            EnableHoverAnimation = true;
            EnablePressAnimation = true;
        }

        #endregion

        #region Header 依赖属性

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                nameof(Header),
                typeof(object),
                typeof(CardView),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置卡片头部内容。
        /// </summary>
        public object? Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        #endregion

        #region HeaderTemplate 依赖属性

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
                nameof(HeaderTemplate),
                typeof(DataTemplate),
                typeof(CardView),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置卡片头部内容模板。
        /// </summary>
        public DataTemplate? HeaderTemplate
        {
            get => (DataTemplate?)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        #endregion

        #region Footer 依赖属性

        public static readonly DependencyProperty FooterProperty =
            DependencyProperty.Register(
                nameof(Footer),
                typeof(object),
                typeof(CardView),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置卡片尾部内容。
        /// </summary>
        public object? Footer
        {
            get => GetValue(FooterProperty);
            set => SetValue(FooterProperty, value);
        }

        #endregion

        #region FooterTemplate 依赖属性

        public static readonly DependencyProperty FooterTemplateProperty =
            DependencyProperty.Register(
                nameof(FooterTemplate),
                typeof(DataTemplate),
                typeof(CardView),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置卡片尾部内容模板。
        /// </summary>
        public DataTemplate? FooterTemplate
        {
            get => (DataTemplate?)GetValue(FooterTemplateProperty);
            set => SetValue(FooterTemplateProperty, value);
        }

        #endregion

        #region Elevation 依赖属性

        public static readonly DependencyProperty ElevationProperty =
            DependencyProperty.Register(
                nameof(Elevation),
                typeof(CardElevation),
                typeof(CardView),
                new PropertyMetadata(CardElevation.Flat, OnElevationChanged));

        /// <summary>
        /// 获取或设置卡片的阴影深度。
        /// </summary>
        public CardElevation Elevation
        {
            get => (CardElevation)GetValue(ElevationProperty);
            set => SetValue(ElevationProperty, value);
        }

        private static void OnElevationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardView cardView)
            {
                cardView.UpdateShadowEffect();
            }
        }

        #endregion

        #region CornerRadius 依赖属性

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(CardView),
                new PropertyMetadata(new CornerRadius(8)));

        /// <summary>
        /// 获取或设置卡片的圆角半径。
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        #endregion

        #region Padding 依赖属性

        public new static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(
                nameof(Padding),
                typeof(Thickness),
                typeof(CardView),
                new PropertyMetadata(new Thickness(16)));

        /// <summary>
        /// 获取或设置卡片内边距。
        /// </summary>
        public new Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        #endregion

        #region EnableHoverAnimation 依赖属性

        public static readonly DependencyProperty EnableHoverAnimationProperty =
            DependencyProperty.Register(
                nameof(EnableHoverAnimation),
                typeof(bool),
                typeof(CardView),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否启用悬停动画。
        /// </summary>
        public bool EnableHoverAnimation
        {
            get => (bool)GetValue(EnableHoverAnimationProperty);
            set => SetValue(EnableHoverAnimationProperty, value);
        }

        #endregion

        #region EnablePressAnimation 依赖属性

        public static readonly DependencyProperty EnablePressAnimationProperty =
            DependencyProperty.Register(
                nameof(EnablePressAnimation),
                typeof(bool),
                typeof(CardView),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否启用按压动画。
        /// </summary>
        public bool EnablePressAnimation
        {
            get => (bool)GetValue(EnablePressAnimationProperty);
            set => SetValue(EnablePressAnimationProperty, value);
        }

        #endregion

        #region HoverElevation 依赖属性

        public static readonly DependencyProperty HoverElevationProperty =
            DependencyProperty.Register(
                nameof(HoverElevation),
                typeof(CardElevation?),
                typeof(CardView),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置悬停时的阴影深度。如果为 null,则在当前 Elevation 基础上增加一级。
        /// </summary>
        public CardElevation? HoverElevation
        {
            get => (CardElevation?)GetValue(HoverElevationProperty);
            set => SetValue(HoverElevationProperty, value);
        }

        #endregion

        #region IsClickable 依赖属性

        public static readonly DependencyProperty IsClickableProperty =
            DependencyProperty.Register(
                nameof(IsClickable),
                typeof(bool),
                typeof(CardView),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置卡片是否可点击。
        /// </summary>
        public bool IsClickable
        {
            get => (bool)GetValue(IsClickableProperty);
            set => SetValue(IsClickableProperty, value);
        }

        #endregion

        #region 辅助方法

        private void UpdateShadowEffect()
        {
            // 根据阴影深度创建适当的阴影效果
            DropShadowEffect? shadow = CreateShadowEffect(Elevation);
            if (shadow != null)
            {
                SetValue(EffectProperty, shadow);
            }
        }

        private DropShadowEffect? CreateShadowEffect(CardElevation elevation)
        {
            return elevation switch
            {
                CardElevation.Flat => null,
                CardElevation.Low => new DropShadowEffect
                {
                    Color = Colors.Black,
                    Opacity = 0.1,
                    BlurRadius = 4,
                    ShadowDepth = 2,
                    Direction = 270
                },
                CardElevation.Medium => new DropShadowEffect
                {
                    Color = Colors.Black,
                    Opacity = 0.15,
                    BlurRadius = 8,
                    ShadowDepth = 4,
                    Direction = 270
                },
                CardElevation.High => new DropShadowEffect
                {
                    Color = Colors.Black,
                    Opacity = 0.2,
                    BlurRadius = 16,
                    ShadowDepth = 6,
                    Direction = 270
                },
                CardElevation.Float => new DropShadowEffect
                {
                    Color = Colors.Black,
                    Opacity = 0.25,
                    BlurRadius = 24,
                    ShadowDepth = 8,
                    Direction = 270
                },
                CardElevation.Popup => new DropShadowEffect
                {
                    Color = Colors.Black,
                    Opacity = 0.3,
                    BlurRadius = 32,
                    ShadowDepth = 12,
                    Direction = 270
                },
                _ => null
            };
        }

        #endregion
    }
}
