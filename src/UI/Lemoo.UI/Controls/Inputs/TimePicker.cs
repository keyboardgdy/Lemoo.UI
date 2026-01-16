using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 时间选择器控件。
    /// </summary>
    /// <remarks>
    /// TimePicker 是一个专门用于选择时间的控件，支持小时和分钟的选择。
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;!-- 基础用法 --&gt;
    /// &lt;ui:TimePicker
    ///     SelectedTime="{Binding StartTime}"
    ///     IsSecondsEnabled="False" /&gt;
    ///
    /// &lt;!-- 带秒选择 --&gt;
    /// &lt;ui:TimePicker
    ///     SelectedTime="{Binding StartTime}"
    ///     IsSecondsEnabled="True" /&gt;
    /// </code>
    /// </example>
    public class TimePicker : Control
    {
        #region Fields

        private Popup? _popup;
        private Button? _hourUpButton;
        private Button? _hourDownButton;
        private Button? _minuteUpButton;
        private Button? _minuteDownButton;
        private Button? _secondUpButton;
        private Button? _secondDownButton;
        private TextBlock? _hourTextBlock;
        private TextBlock? _minuteTextBlock;
        private TextBlock? _secondTextBlock;

        #endregion

        #region Constructor

        static TimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePicker),
                new FrameworkPropertyMetadata(typeof(TimePicker)));
        }

        public TimePicker()
        {
            SelectedTime = DateTime.Now;
        }

        #endregion

        #region SelectedTime 依赖属性

        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register(
                nameof(SelectedTime),
                typeof(DateTime?),
                typeof(TimePicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedTimeChanged));

        /// <summary>
        /// 获取或设置选中的时间。
        /// </summary>
        public DateTime? SelectedTime
        {
            get => (DateTime?)GetValue(SelectedTimeProperty);
            set => SetValue(SelectedTimeProperty, value);
        }

        private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timePicker = (TimePicker)d;
            timePicker.UpdateDisplay();
            timePicker.OnSelectedTimeChanged((DateTime?)e.OldValue, (DateTime?)e.NewValue);
        }

        #endregion

        #region IsSecondsEnabled 依赖属性

        public static readonly DependencyProperty IsSecondsEnabledProperty =
            DependencyProperty.Register(
                nameof(IsSecondsEnabled),
                typeof(bool),
                typeof(TimePicker),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否启用秒选择。
        /// </summary>
        public bool IsSecondsEnabled
        {
            get => (bool)GetValue(IsSecondsEnabledProperty);
            set => SetValue(IsSecondsEnabledProperty, value);
        }

        #endregion

        #region IsDropDownOpen 依赖属性

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(
                nameof(IsDropDownOpen),
                typeof(bool),
                typeof(TimePicker),
                new PropertyMetadata(false, OnIsDropDownOpenChanged));

        /// <summary>
        /// 获取或设置下拉面板是否打开。
        /// </summary>
        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timePicker = (TimePicker)d;
            if (timePicker._popup != null)
            {
                timePicker._popup.IsOpen = (bool)e.NewValue;
            }
        }

        #endregion

        #region Interval 依赖属性

        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(
                nameof(Interval),
                typeof(int),
                typeof(TimePicker),
                new PropertyMetadata(1));

        /// <summary>
        /// 获取或设置时间调整的间隔（分钟）。
        /// </summary>
        public int Interval
        {
            get => (int)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, Math.Max(1, value));
        }

        #endregion

        #region 事件

        public static readonly RoutedEvent SelectedTimeChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(SelectedTimeChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TimePicker));

        /// <summary>
        /// 在选中时间改变时发生。
        /// </summary>
        public event RoutedEventHandler SelectedTimeChanged
        {
            add => AddHandler(SelectedTimeChangedEvent, value);
            remove => RemoveHandler(SelectedTimeChangedEvent, value);
        }

        #endregion

        #region 方法

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // 移除旧的事件处理器
            if (_hourUpButton != null)
            {
                _hourUpButton.Click -= HourUpButton_Click;
            }
            if (_hourDownButton != null)
            {
                _hourDownButton.Click -= HourDownButton_Click;
            }
            if (_minuteUpButton != null)
            {
                _minuteUpButton.Click -= MinuteUpButton_Click;
            }
            if (_minuteDownButton != null)
            {
                _minuteDownButton.Click -= MinuteDownButton_Click;
            }
            if (_secondUpButton != null)
            {
                _secondUpButton.Click -= SecondUpButton_Click;
            }
            if (_secondDownButton != null)
            {
                _secondDownButton.Click -= SecondDownButton_Click;
            }

            // 获取模板部件
            _popup = GetTemplateChild("PART_Popup") as Popup;
            _hourUpButton = GetTemplateChild("PART_HourUpButton") as Button;
            _hourDownButton = GetTemplateChild("PART_HourDownButton") as Button;
            _minuteUpButton = GetTemplateChild("PART_MinuteUpButton") as Button;
            _minuteDownButton = GetTemplateChild("PART_MinuteDownButton") as Button;
            _secondUpButton = GetTemplateChild("PART_SecondUpButton") as Button;
            _secondDownButton = GetTemplateChild("PART_SecondDownButton") as Button;
            _hourTextBlock = GetTemplateChild("PART_HourTextBlock") as TextBlock;
            _minuteTextBlock = GetTemplateChild("PART_MinuteTextBlock") as TextBlock;
            _secondTextBlock = GetTemplateChild("PART_SecondTextBlock") as TextBlock;

            // 获取主按钮并设置点击事件
            var mainButton = GetTemplateChild("PART_Button") as Button;
            if (mainButton != null)
            {
                mainButton.Click += (s, e) => IsDropDownOpen = !IsDropDownOpen;
            }

            // 添加新的事件处理器
            if (_hourUpButton != null)
            {
                _hourUpButton.Click += HourUpButton_Click;
            }
            if (_hourDownButton != null)
            {
                _hourDownButton.Click += HourDownButton_Click;
            }
            if (_minuteUpButton != null)
            {
                _minuteUpButton.Click += MinuteUpButton_Click;
            }
            if (_minuteDownButton != null)
            {
                _minuteDownButton.Click += MinuteDownButton_Click;
            }
            if (_secondUpButton != null)
            {
                _secondUpButton.Click += SecondUpButton_Click;
            }
            if (_secondDownButton != null)
            {
                _secondDownButton.Click += SecondDownButton_Click;
            }

            UpdateDisplay();
        }

        protected virtual void OnSelectedTimeChanged(DateTime? oldTime, DateTime? newTime)
        {
            RaiseEvent(new RoutedEventArgs(SelectedTimeChangedEvent, this));
        }

        private void HourUpButton_Click(object? sender, RoutedEventArgs e)
        {
            if (!SelectedTime.HasValue) return;
            SelectedTime = SelectedTime.Value.AddHours(1);
        }

        private void HourDownButton_Click(object? sender, RoutedEventArgs e)
        {
            if (!SelectedTime.HasValue) return;
            SelectedTime = SelectedTime.Value.AddHours(-1);
        }

        private void MinuteUpButton_Click(object? sender, RoutedEventArgs e)
        {
            if (!SelectedTime.HasValue) return;
            SelectedTime = SelectedTime.Value.AddMinutes(Interval);
        }

        private void MinuteDownButton_Click(object? sender, RoutedEventArgs e)
        {
            if (!SelectedTime.HasValue) return;
            SelectedTime = SelectedTime.Value.AddMinutes(-Interval);
        }

        private void SecondUpButton_Click(object? sender, RoutedEventArgs e)
        {
            if (!SelectedTime.HasValue) return;
            SelectedTime = SelectedTime.Value.AddSeconds(1);
        }

        private void SecondDownButton_Click(object? sender, RoutedEventArgs e)
        {
            if (!SelectedTime.HasValue) return;
            SelectedTime = SelectedTime.Value.AddSeconds(-1);
        }

        private void UpdateDisplay()
        {
            if (_hourTextBlock != null && SelectedTime.HasValue)
            {
                _hourTextBlock.Text = SelectedTime.Value.ToString("HH");
            }
            if (_minuteTextBlock != null && SelectedTime.HasValue)
            {
                _minuteTextBlock.Text = SelectedTime.Value.ToString("mm");
            }
            if (_secondTextBlock != null && SelectedTime.HasValue)
            {
                _secondTextBlock.Text = SelectedTime.Value.ToString("ss");
            }
        }

        #endregion
    }
}
