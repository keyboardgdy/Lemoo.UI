using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Lemoo.UI.Commands;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// Toast 类型枚举。
    /// </summary>
    public enum ToastType
    {
        /// <summary>
        /// 信息
        /// </summary>
        Info,

        /// <summary>
        /// 成功
        /// </summary>
        Success,

        /// <summary>
        /// 警告
        /// </summary>
        Warning,

        /// <summary>
        /// 错误
        /// </summary>
        Error
    }

    /// <summary>
    /// Toast 位置枚举。
    /// </summary>
    public enum ToastPosition
    {
        /// <summary>
        /// 左上角
        /// </summary>
        TopLeft,

        /// <summary>
        /// 中上
        /// </summary>
        TopCenter,

        /// <summary>
        /// 右上角
        /// </summary>
        TopRight,

        /// <summary>
        /// 左下角
        /// </summary>
        BottomLeft,

        /// <summary>
        /// 中下
        /// </summary>
        BottomCenter,

        /// <summary>
        /// 右下角
        /// </summary>
        BottomRight
    }

    /// <summary>
    /// ToastItem 表示单个 Toast 通知项。
    /// </summary>
    public class ToastItem : Control
    {
        #region Constructor

        static ToastItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToastItem),
                new FrameworkPropertyMetadata(typeof(ToastItem)));
        }

        public ToastItem()
        {
        }

        #endregion

        #region Message 依赖属性

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                nameof(Message),
                typeof(string),
                typeof(ToastItem),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置通知消息。
        /// </summary>
        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        #endregion

        #region Type 依赖属性

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register(
                nameof(Type),
                typeof(ToastType),
                typeof(ToastItem),
                new PropertyMetadata(ToastType.Info));

        /// <summary>
        /// 获取或设置通知类型。
        /// </summary>
        public ToastType Type
        {
            get => (ToastType)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        #endregion

        #region CornerRadius 依赖属性

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(ToastItem),
                new PropertyMetadata(new CornerRadius(8)));

        /// <summary>
        /// 获取或设置通知的圆角半径。
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        #endregion

        #region ActionButtonContent 依赖属性

        public static readonly DependencyProperty ActionButtonContentProperty =
            DependencyProperty.Register(
                nameof(ActionButtonContent),
                typeof(object),
                typeof(ToastItem),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置操作按钮内容。
        /// </summary>
        public object? ActionButtonContent
        {
            get => GetValue(ActionButtonContentProperty);
            set => SetValue(ActionButtonContentProperty, value);
        }

        #endregion

        #region ActionButtonCommand 依赖属性

        public static readonly DependencyProperty ActionButtonCommandProperty =
            DependencyProperty.Register(
                nameof(ActionButtonCommand),
                typeof(ICommand),
                typeof(ToastItem),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置操作按钮命令。
        /// </summary>
        public ICommand? ActionButtonCommand
        {
            get => (ICommand?)GetValue(ActionButtonCommandProperty);
            set => SetValue(ActionButtonCommandProperty, value);
        }

        #endregion

        #region ShowCloseButton 依赖属性

        public static readonly DependencyProperty ShowCloseButtonProperty =
            DependencyProperty.Register(
                nameof(ShowCloseButton),
                typeof(bool),
                typeof(ToastItem),
                new PropertyMetadata(true));

        /// <summary>
        /// 获取或设置是否显示关闭按钮。
        /// </summary>
        public bool ShowCloseButton
        {
            get => (bool)GetValue(ShowCloseButtonProperty);
            set => SetValue(ShowCloseButtonProperty, value);
        }

        #endregion

        #region ShowProgress 依赖属性

        public static readonly DependencyProperty ShowProgressProperty =
            DependencyProperty.Register(
                nameof(ShowProgress),
                typeof(bool),
                typeof(ToastItem),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否显示进度条。
        /// </summary>
        public bool ShowProgress
        {
            get => (bool)GetValue(ShowProgressProperty);
            set => SetValue(ShowProgressProperty, value);
        }

        #endregion

        #region Duration 依赖属性

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register(
                nameof(Duration),
                typeof(TimeSpan),
                typeof(ToastItem),
                new PropertyMetadata(TimeSpan.FromSeconds(3)));

        /// <summary>
        /// 获取或设置显示持续时间。
        /// </summary>
        public TimeSpan Duration
        {
            get => (TimeSpan)GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        #endregion

        #region CloseCommand 依赖属性

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register(
                nameof(CloseCommand),
                typeof(ICommand),
                typeof(ToastItem),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置关闭命令。
        /// </summary>
        public ICommand? CloseCommand
        {
            get => (ICommand?)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        #endregion
    }

    /// <summary>
    /// ToastNotification 是非侵入式通知控件,支持多种类型、队列管理、位置配置。
    /// </summary>
    /// <remarks>
    /// ToastNotification 提供以下功能:
    /// - 类型:Info、Success、Warning、Error
    /// - 位置:Top/Bottom + Left/Center/Right
    /// - 进度条(自动关闭倒计时)
    /// - 操作按钮(撤销、重试、查看详情)
    /// - 通知队列(最多显示 N 条)
    /// - 滑入/滑出动画
    /// - 点击关闭
    /// - 持续时间配置
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;!-- XAML 中定义 ToastNotification --&gt;
    /// &lt;ui:ToastNotification x:Name="MyToast" Position="TopRight" /&gt;
    ///
    /// // 代码中显示通知
    /// MyToast.Show("操作成功", ToastType.Success);
    /// MyToast.Show("发生错误", ToastType.Error);
    /// </code>
    /// </example>
    public class ToastNotification : Control
    {
        #region Constructor

        static ToastNotification()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToastNotification),
                new FrameworkPropertyMetadata(typeof(ToastNotification)));
        }

        public ToastNotification()
        {
            _toasts = new List<ToastItem>();
            Toasts = new List<ToastItem>();
            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;
        }

        #endregion

        #region 字段

        private readonly List<ToastItem> _toasts;
        private readonly DispatcherTimer _timer;

        #endregion

        #region Position 依赖属性

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(
                nameof(Position),
                typeof(ToastPosition),
                typeof(ToastNotification),
                new PropertyMetadata(ToastPosition.TopRight));

        /// <summary>
        /// 获取或设置通知位置。
        /// </summary>
        public ToastPosition Position
        {
            get => (ToastPosition)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        #endregion

        #region MaxToasts 依赖属性

        public static readonly DependencyProperty MaxToastsProperty =
            DependencyProperty.Register(
                nameof(MaxToasts),
                typeof(int),
                typeof(ToastNotification),
                new PropertyMetadata(5));

        /// <summary>
        /// 获取或设置最大通知数量。
        /// </summary>
        public int MaxToasts
        {
            get => (int)GetValue(MaxToastsProperty);
            set => SetValue(MaxToastsProperty, value);
        }

        #endregion

        #region Toasts 内部属性

        public static readonly DependencyProperty ToastsProperty =
            DependencyProperty.Register(
                nameof(Toasts),
                typeof(IList<ToastItem>),
                typeof(ToastNotification),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取通知集合(用于内部绑定)。
        /// </summary>
        public IList<ToastItem> Toasts
        {
            get => (IList<ToastItem>)GetValue(ToastsProperty);
            set => SetValue(ToastsProperty, value);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 显示通知。
        /// </summary>
        /// <param name="message">通知消息。</param>
        /// <param name="type">通知类型。</param>
        /// <param name="duration">持续时间。</param>
        public void Show(string message, ToastType type = ToastType.Info, TimeSpan? duration = null)
        {
            var toast = new ToastItem
            {
                Message = message,
                Type = type,
                Duration = duration ?? TimeSpan.FromSeconds(3)
            };
            toast.CloseCommand = new RelayCommand(_ => CloseToast(toast));

            AddToast(toast);
        }

        /// <summary>
        /// 显示带操作按钮的通知。
        /// </summary>
        /// <param name="message">通知消息。</param>
        /// <param name="type">通知类型。</param>
        /// <param name="actionButtonContent">操作按钮内容。</param>
        /// <param name="actionButtonCommand">操作按钮命令。</param>
        /// <param name="duration">持续时间。</param>
        public void Show(
            string message,
            ToastType type,
            object actionButtonContent,
            ICommand actionButtonCommand,
            TimeSpan? duration = null)
        {
            var toast = new ToastItem
            {
                Message = message,
                Type = type,
                ActionButtonContent = actionButtonContent,
                ActionButtonCommand = actionButtonCommand,
                Duration = duration ?? TimeSpan.FromSeconds(3)
            };
            toast.CloseCommand = new RelayCommand(_ => CloseToast(toast));

            AddToast(toast);
        }

        /// <summary>
        /// 显示信息通知。
        /// </summary>
        public void ShowInfo(string message, TimeSpan? duration = null)
        {
            Show(message, ToastType.Info, duration);
        }

        /// <summary>
        /// 显示成功通知。
        /// </summary>
        public void ShowSuccess(string message, TimeSpan? duration = null)
        {
            Show(message, ToastType.Success, duration);
        }

        /// <summary>
        /// 显示警告通知。
        /// </summary>
        public void ShowWarning(string message, TimeSpan? duration = null)
        {
            Show(message, ToastType.Warning, duration);
        }

        /// <summary>
        /// 显示错误通知。
        /// </summary>
        public void ShowError(string message, TimeSpan? duration = null)
        {
            Show(message, ToastType.Error, duration);
        }

        /// <summary>
        /// 关闭所有通知。
        /// </summary>
        public void CloseAll()
        {
            var toasts = Toasts.ToList();
            foreach (var toast in toasts)
            {
                CloseToast(toast);
            }
        }

        #endregion

        #region 私有方法

        private void AddToast(ToastItem toast)
        {
            // 检查最大通知数
            if (Toasts.Count >= MaxToasts)
            {
                // 移除最旧的通知
                CloseToast(Toasts[0]);
            }

            Toasts.Add(toast);
            _toasts.Add(toast);

            // 自动关闭
            if (toast.Duration > TimeSpan.Zero)
            {
                StartAutoClose(toast);
            }
        }

        private void CloseToast(ToastItem toast)
        {
            if (Toasts.Contains(toast))
            {
                Toasts.Remove(toast);
                _toasts.Remove(toast);
            }
        }

        private void StartAutoClose(ToastItem toast)
        {
            _timer.Tag = toast;
            _timer.Interval = toast.Duration;
            _timer.Start();
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            _timer.Stop();
            if (_timer.Tag is ToastItem toast)
            {
                CloseToast(toast);
            }
        }

        #endregion
    }
}
