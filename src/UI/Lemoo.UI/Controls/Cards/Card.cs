using System.Windows;
using System.Windows.Controls;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 卡片容器控件，用于组织相关内容。
    /// </summary>
    /// <remarks>
    /// 卡片是一种容器控件，可以包含头部、内容和尾部。
    /// 它提供了统一的视觉样式和布局。
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;!-- 基础用法 --&gt;
    /// &lt;ui:Card Header="标题"&gt;
    ///     &lt;TextBlock Text="内容" /&gt;
    /// &lt;/ui:Card&gt;
    ///
    /// &lt;!-- 带尾部 --&gt;
    /// &lt;ui:Card&gt;
    ///     &lt;ui:Card.Header&gt;自定义头部&lt;/ui:Card.Header&gt;
    ///     &lt;TextBlock Text="内容" /&gt;
    ///     &lt;ui:Card.Footer&gt;自定义尾部&lt;/ui:Card.Footer&gt;
    /// &lt;/ui:Card&gt;
    ///
    /// &lt;!-- 使用变体样式 --&gt;
    /// &lt;ui:Card Style="{StaticResource Card.Outlined}" Header="轮廓卡片"&gt;
    ///     &lt;TextBlock Text="内容" /&gt;
    /// &lt;/ui:Card&gt;
    ///
    /// &lt;ui:Card Style="{StaticResource Card.Elevated}" Header="抬升卡片"&gt;
    ///     &lt;TextBlock Text="内容" /&gt;
    /// &lt;/ui:Card&gt;
    /// </code>
    /// </example>
    public class Card : ContentControl
    {
        #region Constructor

        static Card()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Card),
                new FrameworkPropertyMetadata(typeof(Card)));
        }

        public Card()
        {
        }

        #endregion

        #region Header 依赖属性

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                nameof(Header),
                typeof(object),
                typeof(Card),
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
                typeof(Card),
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
                typeof(Card),
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
                typeof(Card),
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

        #region CornerRadius 依赖属性

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(Card),
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
                typeof(Card),
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
    }
}
