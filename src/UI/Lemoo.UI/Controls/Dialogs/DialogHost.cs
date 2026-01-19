using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 对话框宿主控件，用于在当前窗口内显示模态对话框。
    /// </summary>
    /// <remarks>
    /// DialogHost 提供了一种在窗口内显示对话框的方式，而不是打开新的窗口。
    /// 它支持自定义对话框内容、遮罩层、动画效果和多种显示方式。
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;!-- XAML 中定义 DialogHost --&gt;
    /// &lt;ui:DialogHost x:Name="MyDialogHost"&gt;
    ///     &lt;ui:DialogHost.DialogContent&gt;
    ///         &lt;ui:Card Width="400" Padding="24"&gt;
    ///             &lt;StackPanel&gt;
    ///                 &lt;TextBlock Text="确认删除" FontSize="18" FontWeight="SemiBold"/&gt;
    ///                 &lt;TextBlock Text="确定要删除这个项目吗？" Margin="0,12,0,0"/&gt;
    ///                 &lt;StackPanel Orientation="Horizontal" HorizontalAlignment="Right" Margin="0,16,0,0"&gt;
    ///                     &lt;Button Content="取消" Click="CancelClick"/&gt;
    ///                     &lt;Button Content="删除" Click="ConfirmClick" Margin="8,0,0,0"/&gt;
    ///                 &lt;/StackPanel&gt;
    ///             &lt;/StackPanel&gt;
    ///         &lt;/ui:Card&gt;
    ///     &lt;/ui:DialogHost.DialogContent&gt;
    ///
    ///     &lt;!-- 主内容 --&gt;
    ///     &lt;Grid&gt;
    ///         &lt;Button Content="显示对话框" Click="ShowDialogClick"/&gt;
    ///     &lt;/Grid&gt;
    /// &lt;/ui:DialogHost&gt;
    ///
    /// // 代码中显示对话框
    /// private void ShowDialogClick(object sender, RoutedEventArgs e)
    /// {
    ///     MyDialogHost.IsOpen = true;
    /// }
    ///
    /// // 代码中关闭对话框
    /// private void CancelClick(object sender, RoutedEventArgs e)
    /// {
    ///     MyDialogHost.IsOpen = false;
    /// }
    /// </code>
    /// </example>
    public class DialogHost : ContentControl, IDisposable
    {
        #region Constructor

        private bool _disposed;

        static DialogHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogHost),
                new FrameworkPropertyMetadata(typeof(DialogHost)));
        }

        public DialogHost()
        {
        }

        #endregion

        #region IsOpen 依赖属性

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(
                nameof(IsOpen),
                typeof(bool),
                typeof(DialogHost),
                new PropertyMetadata(false, OnIsOpenChanged));

        /// <summary>
        /// 获取或设置对话框是否打开。
        /// </summary>
        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DialogHost dialogHost)
            {
                if ((bool)e.NewValue)
                {
                    dialogHost.OnOpened();
                }
                else
                {
                    dialogHost.OnClosed();
                }
            }
        }

        #endregion

        #region DialogContent 依赖属性

        public static readonly DependencyProperty DialogContentProperty =
            DependencyProperty.Register(
                nameof(DialogContent),
                typeof(object),
                typeof(DialogHost),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置对话框内容。
        /// </summary>
        public object? DialogContent
        {
            get => GetValue(DialogContentProperty);
            set => SetValue(DialogContentProperty, value);
        }

        #endregion

        #region DialogContentTemplate 依赖属性

        public static readonly DependencyProperty DialogContentTemplateProperty =
            DependencyProperty.Register(
                nameof(DialogContentTemplate),
                typeof(DataTemplate),
                typeof(DialogHost),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置对话框内容模板。
        /// </summary>
        public DataTemplate? DialogContentTemplate
        {
            get => (DataTemplate?)GetValue(DialogContentTemplateProperty);
            set => SetValue(DialogContentTemplateProperty, value);
        }

        #endregion

        #region ShowOverlay 依赖属性

        public static readonly DependencyProperty ShowOverlayProperty =
            DependencyProperty.Register(
                nameof(ShowOverlay),
                typeof(bool),
                typeof(DialogHost),
                new PropertyMetadata(true));

        /// <summary>
        /// 获取或设置是否显示遮罩层。
        /// </summary>
        public bool ShowOverlay
        {
            get => (bool)GetValue(ShowOverlayProperty);
            set => SetValue(ShowOverlayProperty, value);
        }

        #endregion

        #region OverlayBackground 依赖属性

        public static readonly DependencyProperty OverlayBackgroundProperty =
            DependencyProperty.Register(
                nameof(OverlayBackground),
                typeof(Brush),
                typeof(DialogHost),
                new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// 获取或设置遮罩层背景。
        /// </summary>
        public Brush OverlayBackground
        {
            get => (Brush)GetValue(OverlayBackgroundProperty);
            set => SetValue(OverlayBackgroundProperty, value);
        }

        #endregion

        #region OverlayOpacity 依赖属性

        public static readonly DependencyProperty OverlayOpacityProperty =
            DependencyProperty.Register(
                nameof(OverlayOpacity),
                typeof(double),
                typeof(DialogHost),
                new PropertyMetadata(0.5));

        /// <summary>
        /// 获取或设置遮罩层透明度（0.0-1.0）。
        /// </summary>
        public double OverlayOpacity
        {
            get => (double)GetValue(OverlayOpacityProperty);
            set => SetValue(OverlayOpacityProperty, value);
        }

        #endregion

        #region CloseOnClickOutside 依赖属性

        public static readonly DependencyProperty CloseOnClickOutsideProperty =
            DependencyProperty.Register(
                nameof(CloseOnClickOutside),
                typeof(bool),
                typeof(DialogHost),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否点击遮罩层关闭对话框。
        /// </summary>
        public bool CloseOnClickOutside
        {
            get => (bool)GetValue(CloseOnClickOutsideProperty);
            set => SetValue(CloseOnClickOutsideProperty, value);
        }

        #endregion

        #region DialogHorizontalAlignment 依赖属性

        public static readonly DependencyProperty DialogHorizontalAlignmentProperty =
            DependencyProperty.Register(
                nameof(DialogHorizontalAlignment),
                typeof(HorizontalAlignment),
                typeof(DialogHost),
                new PropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// 获取或设置对话框的水平对齐方式。
        /// </summary>
        public HorizontalAlignment DialogHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(DialogHorizontalAlignmentProperty);
            set => SetValue(DialogHorizontalAlignmentProperty, value);
        }

        #endregion

        #region DialogVerticalAlignment 依赖属性

        public static readonly DependencyProperty DialogVerticalAlignmentProperty =
            DependencyProperty.Register(
                nameof(DialogVerticalAlignment),
                typeof(VerticalAlignment),
                typeof(DialogHost),
                new PropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// 获取或设置对话框的垂直对齐方式。
        /// </summary>
        public VerticalAlignment DialogVerticalAlignment
        {
            get => (VerticalAlignment)GetValue(DialogVerticalAlignmentProperty);
            set => SetValue(DialogVerticalAlignmentProperty, value);
        }

        #endregion

        #region OpenAnimation 依赖属性

        public static readonly DependencyProperty OpenAnimationProperty =
            DependencyProperty.Register(
                nameof(OpenAnimation),
                typeof(DialogAnimation),
                typeof(DialogHost),
                new PropertyMetadata(DialogAnimation.FadeIn));

        /// <summary>
        /// 获取或设置打开对话框的动画类型。
        /// </summary>
        public DialogAnimation OpenAnimation
        {
            get => (DialogAnimation)GetValue(OpenAnimationProperty);
            set => SetValue(OpenAnimationProperty, value);
        }

        #endregion

        #region CloseAnimation 依赖属性

        public static readonly DependencyProperty CloseAnimationProperty =
            DependencyProperty.Register(
                nameof(CloseAnimation),
                typeof(DialogAnimation),
                typeof(DialogHost),
                new PropertyMetadata(DialogAnimation.FadeOut));

        /// <summary>
        /// 获取或设置关闭对话框的动画类型。
        /// </summary>
        public DialogAnimation CloseAnimation
        {
            get => (DialogAnimation)GetValue(CloseAnimationProperty);
            set => SetValue(CloseAnimationProperty, value);
        }

        #endregion

        #region 事件

        /// <summary>
        /// 对话框打开事件。
        /// </summary>
        public event EventHandler<DialogOpenedEventArgs>? DialogOpened;

        /// <summary>
        /// 对话框关闭事件。
        /// </summary>
        public event EventHandler<DialogClosedEventArgs>? DialogClosed;

        /// <summary>
        /// 对话框打开时触发。
        /// </summary>
        protected virtual void OnOpened()
        {
            Focus();
            DialogOpened?.Invoke(this, new DialogOpenedEventArgs());
        }

        /// <summary>
        /// 对话框关闭时触发。
        /// </summary>
        protected virtual void OnClosed()
        {
            DialogClosed?.Invoke(this, new DialogClosedEventArgs());
        }

        #endregion

        #region 内部模板部件

        private UIElement? _dialogContainer;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _dialogContainer = GetTemplateChild("PART_DialogContainer") as UIElement;
        }

        internal void HandleOverlayClick()
        {
            if (CloseOnClickOutside)
            {
                IsOpen = false;
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 显示对话框。
        /// </summary>
        public void ShowDialog() => IsOpen = true;

        /// <summary>
        /// 关闭对话框。
        /// </summary>
        public void CloseDialog() => IsOpen = false;

        #endregion

        #region IDisposable 实现

        /// <summary>
        /// 释放对话框占用的资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放对话框占用的资源。
        /// </summary>
        /// <param name="disposing">是否正在释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // 清理托管资源 - 取消事件订阅
                if (DialogOpened != null)
                {
                    // 获取事件订阅字段并清空
                    var eventField = typeof(DialogHost).GetEvent(nameof(DialogOpened));
                    if (eventField != null)
                    {
                        // 移除所有事件订阅者
                        foreach (var @delegate in DialogOpened.GetInvocationList())
                        {
                            DialogOpened -= (EventHandler<DialogOpenedEventArgs>)@delegate;
                        }
                    }
                }

                if (DialogClosed != null)
                {
                    var eventField = typeof(DialogHost).GetEvent(nameof(DialogClosed));
                    if (eventField != null)
                    {
                        foreach (var @delegate in DialogClosed.GetInvocationList())
                        {
                            DialogClosed -= (EventHandler<DialogClosedEventArgs>)@delegate;
                        }
                    }
                }

                // 释放模板部件引用
                _dialogContainer = null;
            }

            _disposed = true;
        }

        /// <summary>
        /// 析构函数。
        /// </summary>
        ~DialogHost()
        {
            Dispose(false);
        }

        #endregion
    }

    /// <summary>
    /// 对话框动画类型。
    /// </summary>
    public enum DialogAnimation
    {
        /// <summary>
        /// 无动画。
        /// </summary>
        None,

        /// <summary>
        /// 淡入。
        /// </summary>
        FadeIn,

        /// <summary>
        /// 淡出。
        /// </summary>
        FadeOut,

        /// <summary>
        /// 从上滑入。
        /// </summary>
        SlideFromTop,

        /// <summary>
        /// 从下滑入。
        /// </summary>
        SlideFromBottom,

        /// <summary>
        /// 从左滑入。
        /// </summary>
        SlideFromLeft,

        /// <summary>
        /// 从右滑入。
        /// </summary>
        SlideFromRight,

        /// <summary>
        /// 缩放进入。
        /// </summary>
        Zoom,

        /// <summary>
        /// 缩放并淡入。
        /// </summary>
        ZoomFade
    }

    /// <summary>
    /// 对话框打开事件参数。
    /// </summary>
    public class DialogOpenedEventArgs : EventArgs
    {
    }

    /// <summary>
    /// 对话框关闭事件参数。
    /// </summary>
    public class DialogClosedEventArgs : EventArgs
    {
    }
}
