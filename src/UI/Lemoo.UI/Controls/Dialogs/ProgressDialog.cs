using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 进度条类型枚举。
    /// </summary>
    public enum ProgressBarType
    {
        /// <summary>
        /// 线性进度条
        /// </summary>
        Linear,

        /// <summary>
        /// 圆形进度条
        /// </summary>
        Circular
    }

    /// <summary>
    /// ProgressDialog 是美观的进度对话框控件,支持确定/不确定进度、取消操作。
    /// </summary>
    /// <remarks>
    /// ProgressDialog 提供以下功能:
    /// - 确定进度(百分比、进度条)
    /// - 不确定进度(旋转动画、波纹动画)
    /// - 子状态文本(如"正在下载文件 X/Y")
    /// - 取消按钮(可禁用)
    /// - 后台模式(不阻塞界面)
    /// - 预计剩余时间
    /// </remarks>
    /// <example>
    /// <code>
    /// // 代码中显示进度对话框
    /// var dialog = new ProgressDialog
    /// {
    ///     Title = "下载文件",
    ///     Message = "正在下载...",
    ///     IsCancellable = true
    /// };
    /// dialog.Show();
    /// </code>
    /// </example>
    public class ProgressDialog : DialogHost
    {
        #region Constructor

        static ProgressDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressDialog),
                new FrameworkPropertyMetadata(typeof(ProgressDialog)));
        }

        public ProgressDialog()
        {
            DefaultStyleKey = typeof(ProgressDialog);
            ShowOverlay = true;
            CloseOnClickOutside = false;
        }

        #endregion

        #region Title 依赖属性

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(ProgressDialog),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置对话框标题。
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion

        #region CornerRadius 依赖属性

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(ProgressDialog),
                new PropertyMetadata(new CornerRadius(8)));

        /// <summary>
        /// 获取或设置对话框的圆角半径。
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        #endregion

        #region Message 依赖属性

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                nameof(Message),
                typeof(string),
                typeof(ProgressDialog),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置主要消息文本。
        /// </summary>
        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        #endregion

        #region DetailMessage 依赖属性

        public static readonly DependencyProperty DetailMessageProperty =
            DependencyProperty.Register(
                nameof(DetailMessage),
                typeof(string),
                typeof(ProgressDialog),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置详细信息文本。
        /// </summary>
        public string DetailMessage
        {
            get => (string)GetValue(DetailMessageProperty);
            set => SetValue(DetailMessageProperty, value);
        }

        #endregion

        #region ProgressBarType 依赖属性

        public static readonly DependencyProperty ProgressBarTypeProperty =
            DependencyProperty.Register(
                nameof(ProgressBarType),
                typeof(ProgressBarType),
                typeof(ProgressDialog),
                new PropertyMetadata(ProgressBarType.Linear));

        /// <summary>
        /// 获取或设置进度条类型。
        /// </summary>
        public ProgressBarType ProgressBarType
        {
            get => (ProgressBarType)GetValue(ProgressBarTypeProperty);
            set => SetValue(ProgressBarTypeProperty, value);
        }

        #endregion

        #region Progress 依赖属性

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register(
                nameof(Progress),
                typeof(double),
                typeof(ProgressDialog),
                new PropertyMetadata(0.0));

        /// <summary>
        /// 获取或设置当前进度值(0-100)。
        /// </summary>
        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        #endregion

        #region IsIndeterminate 依赖属性

        public static readonly DependencyProperty IsIndeterminateProperty =
            DependencyProperty.Register(
                nameof(IsIndeterminate),
                typeof(bool),
                typeof(ProgressDialog),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否为不确定进度。
        /// </summary>
        public bool IsIndeterminate
        {
            get => (bool)GetValue(IsIndeterminateProperty);
            set => SetValue(IsIndeterminateProperty, value);
        }

        #endregion

        #region IsCancellable 依赖属性

        public static readonly DependencyProperty IsCancellableProperty =
            DependencyProperty.Register(
                nameof(IsCancellable),
                typeof(bool),
                typeof(ProgressDialog),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否允许取消。
        /// </summary>
        public bool IsCancellable
        {
            get => (bool)GetValue(IsCancellableProperty);
            set => SetValue(IsCancellableProperty, value);
        }

        #endregion

        #region CancelButtonText 依赖属性

        public static readonly DependencyProperty CancelButtonTextProperty =
            DependencyProperty.Register(
                nameof(CancelButtonText),
                typeof(string),
                typeof(ProgressDialog),
                new PropertyMetadata("取消"));

        /// <summary>
        /// 获取或设置取消按钮文本。
        /// </summary>
        public string CancelButtonText
        {
            get => (string)GetValue(CancelButtonTextProperty);
            set => SetValue(CancelButtonTextProperty, value);
        }

        #endregion

        #region CancelCommand 依赖属性

        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register(
                nameof(CancelCommand),
                typeof(ICommand),
                typeof(ProgressDialog),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置取消命令。
        /// </summary>
        public ICommand? CancelCommand
        {
            get => (ICommand?)GetValue(CancelCommandProperty);
            set => SetValue(CancelCommandProperty, value);
        }

        #endregion

        #region ShowPercentage 依赖属性

        public static readonly DependencyProperty ShowPercentageProperty =
            DependencyProperty.Register(
                nameof(ShowPercentage),
                typeof(bool),
                typeof(ProgressDialog),
                new PropertyMetadata(true));

        /// <summary>
        /// 获取或设置是否显示百分比。
        /// </summary>
        public bool ShowPercentage
        {
            get => (bool)GetValue(ShowPercentageProperty);
            set => SetValue(ShowPercentageProperty, value);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 显示进度对话框。
        /// </summary>
        public void Show()
        {
            IsOpen = true;
        }

        /// <summary>
        /// 关闭进度对话框。
        /// </summary>
        public void Close()
        {
            IsOpen = false;
        }

        /// <summary>
        /// 更新进度。
        /// </summary>
        /// <param name="progress">进度值(0-100)。</param>
        /// <param name="message">可选的消息文本。</param>
        public void UpdateProgress(double progress, string? message = null)
        {
            Progress = Math.Max(0, Math.Min(100, progress));
            if (!string.IsNullOrEmpty(message))
            {
                Message = message;
            }
        }

        /// <summary>
        /// 设置不确定进度模式。
        /// </summary>
        /// <param name="message">可选的消息文本。</param>
        public void SetIndeterminate(string? message = null)
        {
            IsIndeterminate = true;
            if (!string.IsNullOrEmpty(message))
            {
                Message = message;
            }
        }

        /// <summary>
        /// 设置确定进度模式。
        /// </summary>
        /// <param name="progress">进度值(0-100)。</param>
        /// <param name="message">可选的消息文本。</param>
        public void SetDeterminate(double progress, string? message = null)
        {
            IsIndeterminate = false;
            UpdateProgress(progress, message);
        }

        #endregion
    }
}
