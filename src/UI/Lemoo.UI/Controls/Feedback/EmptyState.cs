using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Lemoo.UI.Controls.Feedback
{
    /// <summary>
    /// 空状态预设类型
    /// </summary>
    public enum EmptyStatePreset
    {
        /// <summary>
        /// 无数据
        /// </summary>
        NoData,

        /// <summary>
        /// 无搜索结果
        /// </summary>
        NoSearchResults,

        /// <summary>
        /// 无网络
        /// </summary>
        NoNetwork,

        /// <summary>
        /// 错误
        /// </summary>
        Error,

        /// <summary>
        /// 加载中
        /// </summary>
        Loading,

        /// <summary>
        /// 维护中
        /// </summary>
        Maintenance,

        /// <summary>
        /// 未授权
        /// </summary>
        Unauthorized,

        /// <summary>
        /// 自定义
        /// </summary>
        Custom
    }

    /// <summary>
    /// 空状态控件，优雅展示空数据场景
    /// </summary>
    public class EmptyState : Control
    {
        #region Constructor

        static EmptyState()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EmptyState),
                new FrameworkPropertyMetadata(typeof(EmptyState)));
        }

        public EmptyState()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 获取或设置预设类型
        /// </summary>
        public EmptyStatePreset Preset
        {
            get => (EmptyStatePreset)GetValue(PresetProperty);
            set => SetValue(PresetProperty, value);
        }

        public static readonly DependencyProperty PresetProperty =
            DependencyProperty.Register(nameof(Preset), typeof(EmptyStatePreset), typeof(EmptyState),
                new PropertyMetadata(EmptyStatePreset.NoData, OnPresetChanged));

        /// <summary>
        /// 获取或设置图标
        /// </summary>
        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(object), typeof(EmptyState),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置标题
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(EmptyState),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置描述文本
        /// </summary>
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(EmptyState),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置主操作按钮文本
        /// </summary>
        public string ActionText
        {
            get => (string)GetValue(ActionTextProperty);
            set => SetValue(ActionTextProperty, value);
        }

        public static readonly DependencyProperty ActionTextProperty =
            DependencyProperty.Register(nameof(ActionText), typeof(string), typeof(EmptyState),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置次要操作文本
        /// </summary>
        public string SecondaryActionText
        {
            get => (string)GetValue(SecondaryActionTextProperty);
            set => SetValue(SecondaryActionTextProperty, value);
        }

        public static readonly DependencyProperty SecondaryActionTextProperty =
            DependencyProperty.Register(nameof(SecondaryActionText), typeof(string), typeof(EmptyState),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置图标大小
        /// </summary>
        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(EmptyState),
                new PropertyMetadata(128.0));

        /// <summary>
        /// 获取或设置图标颜色
        /// </summary>
        public Brush IconBrush
        {
            get => (Brush)GetValue(IconBrushProperty);
            set => SetValue(IconBrushProperty, value);
        }

        public static readonly DependencyProperty IconBrushProperty =
            DependencyProperty.Register(nameof(IconBrush), typeof(Brush), typeof(EmptyState),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置主操作命令
        /// </summary>
        public ICommand ActionCommand
        {
            get => (ICommand)GetValue(ActionCommandProperty);
            set => SetValue(ActionCommandProperty, value);
        }

        public static readonly DependencyProperty ActionCommandProperty =
            DependencyProperty.Register(nameof(ActionCommand), typeof(ICommand), typeof(EmptyState),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置次要操作命令
        /// </summary>
        public ICommand SecondaryActionCommand
        {
            get => (ICommand)GetValue(SecondaryActionCommandProperty);
            set => SetValue(SecondaryActionCommandProperty, value);
        }

        public static readonly DependencyProperty SecondaryActionCommandProperty =
            DependencyProperty.Register(nameof(SecondaryActionCommand), typeof(ICommand), typeof(EmptyState),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置是否显示动画
        /// </summary>
        public bool EnableAnimation
        {
            get => (bool)GetValue(EnableAnimationProperty);
            set => SetValue(EnableAnimationProperty, value);
        }

        public static readonly DependencyProperty EnableAnimationProperty =
            DependencyProperty.Register(nameof(EnableAnimation), typeof(bool), typeof(EmptyState),
                new PropertyMetadata(true));

        /// <summary>
        /// 获取或设置自定义内容
        /// </summary>
        public object CustomContent
        {
            get => GetValue(CustomContentProperty);
            set => SetValue(CustomContentProperty, value);
        }

        public static readonly DependencyProperty CustomContentProperty =
            DependencyProperty.Register(nameof(CustomContent), typeof(object), typeof(EmptyState),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置最大宽度
        /// </summary>
        public double MaxContentWidth
        {
            get => (double)GetValue(MaxContentWidthProperty);
            set => SetValue(MaxContentWidthProperty, value);
        }

        public static readonly DependencyProperty MaxContentWidthProperty =
            DependencyProperty.Register(nameof(MaxContentWidth), typeof(double), typeof(EmptyState),
                new PropertyMetadata(400.0));

        #endregion

        #region Event Handlers

        private static void OnPresetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EmptyState control)
            {
                control.ApplyPreset((EmptyStatePreset)e.NewValue);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 应用预设
        /// </summary>
        public void ApplyPreset(EmptyStatePreset preset)
        {
            switch (preset)
            {
                case EmptyStatePreset.NoData:
                    ApplyNoDataPreset();
                    break;
                case EmptyStatePreset.NoSearchResults:
                    ApplyNoSearchResultsPreset();
                    break;
                case EmptyStatePreset.NoNetwork:
                    ApplyNoNetworkPreset();
                    break;
                case EmptyStatePreset.Error:
                    ApplyErrorPreset();
                    break;
                case EmptyStatePreset.Loading:
                    ApplyLoadingPreset();
                    break;
                case EmptyStatePreset.Maintenance:
                    ApplyMaintenancePreset();
                    break;
                case EmptyStatePreset.Unauthorized:
                    ApplyUnauthorizedPreset();
                    break;
                case EmptyStatePreset.Custom:
                    // 自定义类型不应用任何预设
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void ApplyNoDataPreset()
        {
            Title = "暂无数据";
            Description = "这里还没有任何内容，快来添加第一条数据吧！";
            ActionText = "添加数据";
        }

        private void ApplyNoSearchResultsPreset()
        {
            Title = "未找到相关内容";
            Description = "没有找到与您搜索相关的内容，请尝试其他关键词。";
            ActionText = "清除搜索";
        }

        private void ApplyNoNetworkPreset()
        {
            Title = "网络连接失败";
            Description = "无法连接到服务器，请检查您的网络连接后重试。";
            ActionText = "重试";
        }

        private void ApplyErrorPreset()
        {
            Title = "出错了";
            Description = "抱歉，加载内容时出现了问题，请稍后再试。";
            ActionText = "刷新";
        }

        private void ApplyLoadingPreset()
        {
            Title = "加载中...";
            Description = "正在努力加载内容，请稍候。";
        }

        private void ApplyMaintenancePreset()
        {
            Title = "系统维护中";
            Description = "系统正在进行维护，暂时无法提供服务，请稍后再试。";
        }

        private void ApplyUnauthorizedPreset()
        {
            Title = "未授权访问";
            Description = "您没有权限访问此内容，请联系管理员获取权限。";
            ActionText = "重新登录";
        }

        #endregion
    }
}
