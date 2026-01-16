using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Lemoo.UI.Commands;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 数字输入控件。
    /// </summary>
    /// <remarks>
    /// NumericUpDown 是一个专门用于数字输入的控件，带有增加/减少按钮。
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;!-- 基础用法 --&gt;
    /// &lt;ui:NumericUpDown Value="{Binding Quantity}" /&gt;
    ///
    /// &lt;!-- 设置范围 --&gt;
    /// &lt;ui:NumericUpDown
    ///     Value="{Binding Quantity}"
    ///     Minimum="0"
    ///     Maximum="100"
    ///     Increment="5" /&gt;
    ///
    /// &lt;!-- 小数支持 --&gt;
    /// &lt;ui:NumericUpDown
    ///     Value="{Binding Price}"
    ///     DecimalPlaces="2"
    ///     Increment="0.01" /&gt;
    /// </code>
    /// </example>
    public class NumericUpDown : Control
    {
        #region Constructor

        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown),
                new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }

        public NumericUpDown()
        {
            // 默认命令
            IncrementValueCommand = new RelayCommand(_ => IncrementValue());
            DecrementValueCommand = new RelayCommand(_ => DecrementValue());
        }

        #endregion

        #region Value 依赖属性

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(double),
                typeof(NumericUpDown),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, CoerceValue));

        /// <summary>
        /// 获取或设置当前值。
        /// </summary>
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var nud = (NumericUpDown)d;
            nud.CoerceValue(MinimumProperty);
            nud.CoerceValue(MaximumProperty);
            nud.OnValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            var nud = (NumericUpDown)d;
            var value = (double)baseValue;
            return Math.Max(nud.Minimum, Math.Min(nud.Maximum, value));
        }

        #endregion

        #region Minimum 依赖属性

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                nameof(Minimum),
                typeof(double),
                typeof(NumericUpDown),
                new FrameworkPropertyMetadata(0.0, OnMinimumChanged, CoerceMinimum));

        /// <summary>
        /// 获取或设置最小值。
        /// </summary>
        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var nud = (NumericUpDown)d;
            nud.CoerceValue(MaximumProperty);
            nud.CoerceValue(ValueProperty);
        }

        private static object CoerceMinimum(DependencyObject d, object baseValue)
        {
            var nud = (NumericUpDown)d;
            var min = (double)baseValue;
            return min > nud.Maximum ? nud.Maximum : min;
        }

        #endregion

        #region Maximum 依赖属性

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                nameof(Maximum),
                typeof(double),
                typeof(NumericUpDown),
                new FrameworkPropertyMetadata(100.0, OnMaximumChanged, CoerceMaximum));

        /// <summary>
        /// 获取或设置最大值。
        /// </summary>
        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var nud = (NumericUpDown)d;
            nud.CoerceValue(MinimumProperty);
            nud.CoerceValue(ValueProperty);
        }

        private static object CoerceMaximum(DependencyObject d, object baseValue)
        {
            var nud = (NumericUpDown)d;
            var max = (double)baseValue;
            return max < nud.Minimum ? nud.Minimum : max;
        }

        #endregion

        #region Increment 依赖属性

        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register(
                nameof(Increment),
                typeof(double),
                typeof(NumericUpDown),
                new PropertyMetadata(1.0));

        /// <summary>
        /// 获取或设置增量。
        /// </summary>
        public double Increment
        {
            get => (double)GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }

        #endregion

        #region DecimalPlaces 依赖属性

        public static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register(
                nameof(DecimalPlaces),
                typeof(int),
                typeof(NumericUpDown),
                new PropertyMetadata(0, OnDecimalPlacesChanged));

        /// <summary>
        /// 获取或设置小数位数。
        /// </summary>
        public int DecimalPlaces
        {
            get => (int)GetValue(DecimalPlacesProperty);
            set => SetValue(DecimalPlacesProperty, value);
        }

        private static void OnDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var nud = (NumericUpDown)d;
            nud.Value = Math.Round(nud.Value, (int)e.NewValue);
        }

        #endregion

        #region IsReadOnly 依赖属性

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                nameof(IsReadOnly),
                typeof(bool),
                typeof(NumericUpDown),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否只读。
        /// </summary>
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        #endregion

        #region Commands

        public static readonly DependencyProperty IncrementValueCommandProperty =
            DependencyProperty.Register(
                nameof(IncrementValueCommand),
                typeof(ICommand),
                typeof(NumericUpDown),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置增加值的命令。
        /// </summary>
        public ICommand? IncrementValueCommand
        {
            get => (ICommand?)GetValue(IncrementValueCommandProperty);
            set => SetValue(IncrementValueCommandProperty, value);
        }

        public static readonly DependencyProperty DecrementValueCommandProperty =
            DependencyProperty.Register(
                nameof(DecrementValueCommand),
                typeof(ICommand),
                typeof(NumericUpDown),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置减少值的命令。
        /// </summary>
        public ICommand? DecrementValueCommand
        {
            get => (ICommand?)GetValue(DecrementValueCommandProperty);
            set => SetValue(DecrementValueCommandProperty, value);
        }

        #endregion

        #region 事件

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(ValueChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(NumericUpDown));

        /// <summary>
        /// 在值改变时发生。
        /// </summary>
        public event RoutedEventHandler ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }

        #endregion

        #region 方法

        protected virtual void OnValueChanged(double oldValue, double newValue)
        {
            RaiseEvent(new RoutedEventArgs(ValueChangedEvent, this));
        }

        /// <summary>
        /// 增加值。
        /// </summary>
        public void IncrementValue()
        {
            if (IsReadOnly) return;
            var newValue = Math.Min(Value + Increment, Maximum);
            Value = Math.Round(newValue, DecimalPlaces);
        }

        /// <summary>
        /// 减少值。
        /// </summary>
        public void DecrementValue()
        {
            if (IsReadOnly) return;
            var newValue = Math.Max(Value - Increment, Minimum);
            Value = Math.Round(newValue, DecimalPlaces);
        }

        #endregion
    }
}
