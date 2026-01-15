using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Lemoo.UI.Commands;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// Snackbar 通知控件，用于显示临时消息提示。
    /// </summary>
    /// <remarks>
    /// Snackbar 是一种非侵入式的通知方式，通常显示在屏幕底部，
    /// 用于显示操作结果、简短提示或确认消息。
    /// 它会自动消失或可以通过用户操作关闭。
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;!-- XAML 中定义 Snackbar --&gt;
    /// &lt;ui:Snackbar x:Name="MySnackbar"
    ///               VerticalAlignment="Bottom"
    ///               HorizontalAlignment="Center"
    ///               Margin="0,0,0,16"/&gt;
    ///
    /// // 代码中显示消息
    /// MySnackbar.Show("文件已保存成功");
    ///
    /// // 显示带操作按钮的消息
    /// MySnackbar.Show("文件已删除", "撤销", () => {
    ///     // 撤销操作
    /// });
    ///
    /// // 显示不同类型的通知
    /// MySnackbar.Show("操作成功", SnackbarSeverity.Success);
    /// MySnackbar.Show("操作失败", SnackbarSeverity.Error);
    /// MySnackbar.Show("警告信息", SnackbarSeverity.Warning);
    /// </code>
    /// </example>
    public class Snackbar : ContentControl
    {
        #region Constructor

        static Snackbar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Snackbar),
                new FrameworkPropertyMetadata(typeof(Snackbar)));
        }

        public Snackbar()
        {
            _dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(DefaultDuration)
            };
            _dispatcherTimer.Tick += OnTimerTick;
        }

        #endregion

        #region IsOpen 依赖属性

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(
                nameof(IsOpen),
                typeof(bool),
                typeof(Snackbar),
                new PropertyMetadata(false, OnIsOpenChanged));

        /// <summary>
        /// 获取或设置通知是否打开。
        /// </summary>
        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Snackbar snackbar)
            {
                if ((bool)e.NewValue)
                {
                    snackbar.OnOpened();
                }
                else
                {
                    snackbar.OnClosed();
                }
            }
        }

        #endregion

        #region Message 依赖属性

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                nameof(Message),
                typeof(string),
                typeof(Snackbar),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置通知消息文本。
        /// </summary>
        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        #endregion

        #region Severity 依赖属性

        public static readonly DependencyProperty SeverityProperty =
            DependencyProperty.Register(
                nameof(Severity),
                typeof(SnackbarSeverity),
                typeof(Snackbar),
                new PropertyMetadata(SnackbarSeverity.Info));

        /// <summary>
        /// 获取或设置通知严重程度类型。
        /// </summary>
        public SnackbarSeverity Severity
        {
            get => (SnackbarSeverity)GetValue(SeverityProperty);
            set => SetValue(SeverityProperty, value);
        }

        #endregion

        #region ActionButtonContent 依赖属性

        public static readonly DependencyProperty ActionButtonContentProperty =
            DependencyProperty.Register(
                nameof(ActionButtonContent),
                typeof(object),
                typeof(Snackbar),
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

        #region ShowIcon 依赖属性

        public static readonly DependencyProperty ShowIconProperty =
            DependencyProperty.Register(
                nameof(ShowIcon),
                typeof(bool),
                typeof(Snackbar),
                new PropertyMetadata(true));

        /// <summary>
        /// 获取或设置是否显示图标。
        /// </summary>
        public bool ShowIcon
        {
            get => (bool)GetValue(ShowIconProperty);
            set => SetValue(ShowIconProperty, value);
        }

        #endregion

        #region ShowCloseButton 依赖属性

        public static readonly DependencyProperty ShowCloseButtonProperty =
            DependencyProperty.Register(
                nameof(ShowCloseButton),
                typeof(bool),
                typeof(Snackbar),
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

        #region Duration 依赖属性

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register(
                nameof(Duration),
                typeof(int),
                typeof(Snackbar),
                new PropertyMetadata(DefaultDuration));

        /// <summary>
        /// 获取或设置通知显示时长（毫秒）。设置为 0 表示不自动关闭。
        /// </summary>
        public int Duration
        {
            get => (int)GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        private const int DefaultDuration = 4000;

        #endregion

        #region CornerRadius 依赖属性

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(Snackbar),
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

        #region 字段

        private readonly DispatcherTimer _dispatcherTimer;
        private ICommand? _actionCommand;

        #endregion

        #region 事件

        /// <summary>
        /// 操作按钮点击事件。
        /// </summary>
        public event EventHandler<SnackbarActionEventArgs>? ActionClick;

        /// <summary>
        /// 通知关闭事件。
        /// </summary>
        public event EventHandler<SnackbarClosedEventArgs>? Closed;

        #endregion

        #region 公共方法

        /// <summary>
        /// 显示通知消息。
        /// </summary>
        /// <param name="message">消息文本。</param>
        /// <param name="severity">通知类型。</param>
        /// <param name="duration">显示时长（毫秒），0 表示不自动关闭。</param>
        public void Show(string message, SnackbarSeverity severity = SnackbarSeverity.Info, int duration = 4000)
        {
            Message = message;
            Severity = severity;
            Duration = duration;
            ActionButtonContent = null;
            IsOpen = true;
        }

        /// <summary>
        /// 显示带操作按钮的通知消息。
        /// </summary>
        /// <param name="message">消息文本。</param>
        /// <param name="actionButtonText">操作按钮文本。</param>
        /// <param name="actionCallback">操作按钮回调。</param>
        /// <param name="severity">通知类型。</param>
        /// <param name="duration">显示时长（毫秒），0 表示不自动关闭。</param>
        public void Show(string message, string actionButtonText, Action actionCallback,
            SnackbarSeverity severity = SnackbarSeverity.Info, int duration = 4000)
        {
            Message = message;
            Severity = severity;
            Duration = duration;
            ActionButtonContent = actionButtonText;
            _actionCommand = new RelayCommand(_ => actionCallback());
            IsOpen = true;
        }

        /// <summary>
        /// 关闭通知。
        /// </summary>
        public void Close() => IsOpen = false;

        #endregion

        #region 模板部件绑定

        private Button? _actionButton;
        private Button? _closeButton;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // 解除旧的事件绑定
            if (_actionButton != null)
            {
                _actionButton.Click -= OnActionButtonClick;
            }
            if (_closeButton != null)
            {
                _closeButton.Click -= OnCloseButtonClick;
            }

            // 获取模板部件
            _actionButton = GetTemplateChild("ActionButton") as Button;
            _closeButton = GetTemplateChild("CloseButton") as Button;

            // 绑定新的事件
            if (_actionButton != null)
            {
                _actionButton.Click += OnActionButtonClick;
            }
            if (_closeButton != null)
            {
                _closeButton.Click += OnCloseButtonClick;
            }
        }

        private void OnActionButtonClick(object sender, RoutedEventArgs e)
        {
            if (_actionCommand != null && _actionCommand.CanExecute(null))
            {
                _actionCommand.Execute(null);
            }

            ActionClick?.Invoke(this, new SnackbarActionEventArgs(Message, Severity));
            Close();
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region 内部方法

        internal void HandleActionClick()
        {
            OnActionButtonClick(this, new RoutedEventArgs());
        }

        internal void HandleCloseClick()
        {
            OnCloseButtonClick(this, new RoutedEventArgs());
        }

        private void OnOpened()
        {
            if (Duration > 0)
            {
                _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(Duration);
                _dispatcherTimer.Start();
            }
        }

        private void OnClosed()
        {
            _dispatcherTimer.Stop();
            Closed?.Invoke(this, new SnackbarClosedEventArgs());
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            _dispatcherTimer.Stop();
            Close();
        }

        #endregion
    }

    /// <summary>
    /// Snackbar 通知严重程度类型。
    /// </summary>
    public enum SnackbarSeverity
    {
        /// <summary>
        /// 信息提示（默认）。
        /// </summary>
        Info,

        /// <summary>
        /// 成功提示。
        /// </summary>
        Success,

        /// <summary>
        /// 警告提示。
        /// </summary>
        Warning,

        /// <summary>
        /// 错误提示。
        /// </summary>
        Error
    }

    /// <summary>
    /// Snackbar 操作按钮事件参数。
    /// </summary>
    public class SnackbarActionEventArgs : EventArgs
    {
        /// <summary>
        /// 消息文本。
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// 通知类型。
        /// </summary>
        public SnackbarSeverity Severity { get; }

        public SnackbarActionEventArgs(string message, SnackbarSeverity severity)
        {
            Message = message;
            Severity = severity;
        }
    }

    /// <summary>
    /// Snackbar 关闭事件参数。
    /// </summary>
    public class SnackbarClosedEventArgs : EventArgs
    {
    }
}
