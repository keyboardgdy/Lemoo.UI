using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Lemoo.UI.Design;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 徽章控件，用于显示通知计数或状态指示。
    /// </summary>
    /// <remarks>
    /// 徽章通常附加在其他控件上，用于显示数字、状态或通知。
    /// </remarks>
    /// <example>
    /// <code>
    /// <!-- 基础用法 -->
    /// <ui:Badge Content="5" />
    ///
    /// <!-- 附加到按钮 -->
    /// <Grid>
    ///     <Button Content="消息" />
    ///     <ui:Badge Content="99" BadgePlacement="TopRight" />
    /// </Grid>
    ///
    /// <!-- 点状徽章 -->
    /// <ui:Badge BadgeShape="Dot" />
    ///
    /// <!-- 自定义颜色 -->
    /// <ui:Badge Content="New" Background="{DynamicResource Brush.Semantic.Success}" />
    /// </code>
    /// </example>
    [ToolboxItem(true)]
    [Category(ToolboxCategories.Buttons)]
    [Description("用于显示通知计数或状态指示的徽章控件")]
    public class Badge : ContentControl
    {
        #region Constructor

        static Badge()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Badge),
                new FrameworkPropertyMetadata(typeof(Badge)));
        }

        public Badge()
        {
        }

        #endregion

        #region BadgeShape 依赖属性

        public static readonly DependencyProperty BadgeShapeProperty =
            DependencyProperty.Register(
                nameof(BadgeShape),
                typeof(BadgeShape),
                typeof(Badge),
                new PropertyMetadata(BadgeShape.Pill));

        /// <summary>
        /// 获取或设置徽章形状。
        /// </summary>
        public BadgeShape BadgeShape
        {
            get => (BadgeShape)GetValue(BadgeShapeProperty);
            set => SetValue(BadgeShapeProperty, value);
        }

        #endregion

        #region BadgePlacement 依赖属性

        public static readonly DependencyProperty BadgePlacementProperty =
            DependencyProperty.Register(
                nameof(BadgePlacement),
                typeof(BadgePlacement),
                typeof(Badge),
                new PropertyMetadata(BadgePlacement.TopRight));

        /// <summary>
        /// 获取或设置徽章位置。
        /// </summary>
        public BadgePlacement BadgePlacement
        {
            get => (BadgePlacement)GetValue(BadgePlacementProperty);
            set => SetValue(BadgePlacementProperty, value);
        }

        #endregion

        #region CornerRadius 依赖属性

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(Badge),
                new PropertyMetadata(new CornerRadius(12)));

        /// <summary>
        /// 获取或设置徽章的圆角半径。
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        #endregion

        #region ShowZero 依赖属性

        public static readonly DependencyProperty ShowZeroProperty =
            DependencyProperty.Register(
                nameof(ShowZero),
                typeof(bool),
                typeof(Badge),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否显示零值。
        /// </summary>
        public bool ShowZero
        {
            get => (bool)GetValue(ShowZeroProperty);
            set => SetValue(ShowZeroProperty, value);
        }

        #endregion

        #region MaxValue 依赖属性

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(
                nameof(MaxValue),
                typeof(int),
                typeof(Badge),
                new PropertyMetadata(99));

        /// <summary>
        /// 获取或设置显示的最大值，超过则显示 99+。
        /// </summary>
        public int MaxValue
        {
            get => (int)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        #endregion
    }

    /// <summary>
    /// 徽章形状。
    /// </summary>
    public enum BadgeShape
    {
        /// <summary>
        /// 胶囊形状（默认）。
        /// </summary>
        Pill,

        /// <summary>
        /// 圆形。
        /// </summary>
        Circle,

        /// <summary>
        /// 圆角矩形。
        /// </summary>
        Rounded,

        /// <summary>
        /// 点状（不显示内容）。
        /// </summary>
        Dot
    }

    /// <summary>
    /// 徽章位置。
    /// </summary>
    public enum BadgePlacement
    {
        /// <summary>
        /// 左上角。
        /// </summary>
        TopLeft,

        /// <summary>
        /// 右上角（默认）。
        /// </summary>
        TopRight,

        /// <summary>
        /// 左下角。
        /// </summary>
        BottomLeft,

        /// <summary>
        /// 右下角。
        /// </summary>
        BottomRight,

        /// <summary>
        /// 顶部居中。
        /// </summary>
        TopCenter,

        /// <summary>
        /// 底部居中。
        /// </summary>
        BottomCenter
    }
}
