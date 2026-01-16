using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 分割视图控件。
    /// </summary>
    /// <remarks>
    /// SplitView 是一个可以显示/隐藏侧边面板的布局控件，常用于创建响应式导航界面。
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;!-- 基础用法 --&gt;
    /// &lt;ui:SplitView IsPaneOpen="True"&gt;
    ///     &lt;ui:SplitView.Pane&gt;
    ///         &lt;StackPanel&gt;
    ///             &lt;Button Content="选项 1"/&gt;
    ///             &lt;Button Content="选项 2"/&gt;
    ///         &lt;/StackPanel&gt;
    ///     &lt;/ui:SplitView.Pane&gt;
    ///     &lt;ui:SplitView.Content&gt;
    ///         &lt;TextBlock Text="主内容区域"/&gt;
    ///     &lt;/ui:SplitView.Content&gt;
    /// &lt;/ui:SplitView&gt;
    /// </code>
    /// </example>
    public class SplitView : ContentControl
    {
        #region Constructor

        static SplitView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitView),
                new FrameworkPropertyMetadata(typeof(SplitView)));
        }

        public SplitView()
        {
        }

        #endregion

        #region Pane 依赖属性

        public static readonly DependencyProperty PaneProperty =
            DependencyProperty.Register(
                nameof(Pane),
                typeof(object),
                typeof(SplitView),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置侧边面板的内容。
        /// </summary>
        public object? Pane
        {
            get => GetValue(PaneProperty);
            set => SetValue(PaneProperty, value);
        }

        #endregion

        #region PanePlacement 依赖属性

        public static readonly DependencyProperty PanePlacementProperty =
            DependencyProperty.Register(
                nameof(PanePlacement),
                typeof(SplitViewPanePlacement),
                typeof(SplitView),
                new PropertyMetadata(SplitViewPanePlacement.Left));

        /// <summary>
        /// 获取或设置侧边面板的位置。
        /// </summary>
        public SplitViewPanePlacement PanePlacement
        {
            get => (SplitViewPanePlacement)GetValue(PanePlacementProperty);
            set => SetValue(PanePlacementProperty, value);
        }

        #endregion

        #region IsPaneOpen 依赖属性

        public static readonly DependencyProperty IsPaneOpenProperty =
            DependencyProperty.Register(
                nameof(IsPaneOpen),
                typeof(bool),
                typeof(SplitView),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置侧边面板是否打开。
        /// </summary>
        public bool IsPaneOpen
        {
            get => (bool)GetValue(IsPaneOpenProperty);
            set => SetValue(IsPaneOpenProperty, value);
        }

        #endregion

        #region PaneBackground 依赖属性

        public static readonly DependencyProperty PaneBackgroundProperty =
            DependencyProperty.Register(
                nameof(PaneBackground),
                typeof(Brush),
                typeof(SplitView),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置侧边面板的背景。
        /// </summary>
        public Brush? PaneBackground
        {
            get => (Brush?)GetValue(PaneBackgroundProperty);
            set => SetValue(PaneBackgroundProperty, value);
        }

        #endregion

        #region DisplayMode 依赖属性

        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register(
                nameof(DisplayMode),
                typeof(SplitViewDisplayMode),
                typeof(SplitView),
                new PropertyMetadata(SplitViewDisplayMode.Overlay));

        /// <summary>
        /// 获取或设置显示模式。
        /// </summary>
        public SplitViewDisplayMode DisplayMode
        {
            get => (SplitViewDisplayMode)GetValue(DisplayModeProperty);
            set => SetValue(DisplayModeProperty, value);
        }

        #endregion

        #region OpenPaneLength 依赖属性

        public static readonly DependencyProperty OpenPaneLengthProperty =
            DependencyProperty.Register(
                nameof(OpenPaneLength),
                typeof(double),
                typeof(SplitView),
                new PropertyMetadata(250.0));

        /// <summary>
        /// 获取或设置打开的侧边面板宽度。
        /// </summary>
        public double OpenPaneLength
        {
            get => (double)GetValue(OpenPaneLengthProperty);
            set => SetValue(OpenPaneLengthProperty, value);
        }

        #endregion

        #region CompactPaneLength 依赖属性

        public static readonly DependencyProperty CompactPaneLengthProperty =
            DependencyProperty.Register(
                nameof(CompactPaneLength),
                typeof(double),
                typeof(SplitView),
                new PropertyMetadata(48.0));

        /// <summary>
        /// 获取或设置紧凑模式下侧边面板的宽度。
        /// </summary>
        public double CompactPaneLength
        {
            get => (double)GetValue(CompactPaneLengthProperty);
            set => SetValue(CompactPaneLengthProperty, value);
        }

        #endregion

    }

    /// <summary>
    /// 分割视图面板位置枚举。
    /// </summary>
    public enum SplitViewPanePlacement
    {
        /// <summary>
        /// 面板位于左侧。
        /// </summary>
        Left,

        /// <summary>
        /// 面板位于右侧。
        /// </summary>
        Right
    }

    /// <summary>
    /// 分割视图显示模式枚举。
    /// </summary>
    public enum SplitViewDisplayMode
    {
        /// <summary>
        /// 覆盖模式：面板打开时覆盖内容。
        /// </summary>
        Overlay,

        /// <summary>
        /// 内联模式：面板打开时推开内容。
        /// </summary>
        Inline,

        /// <summary>
        /// 紧凑覆盖模式：面板紧凑显示时覆盖内容。
        /// </summary>
        CompactOverlay,

        /// <summary>
        /// 紧凑内联模式：面板紧凑显示时推开内容。
        /// </summary>
        CompactInline
    }
}
