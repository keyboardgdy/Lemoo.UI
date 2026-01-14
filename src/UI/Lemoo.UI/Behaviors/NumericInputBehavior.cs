using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Lemoo.UI.Interactivity;

namespace Lemoo.UI.Behaviors
{
    /// <summary>
    /// 限制 TextBox 只能输入数字的行为。
    /// </summary>
    public class NumericInputBehavior : Behavior<TextBox>
    {
        #region MinValue 附加属性

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.RegisterAttached(
                "MinValue",
                typeof(double?),
                typeof(NumericInputBehavior),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取最小值。
        /// </summary>
        public static double? GetMinValue(DependencyObject obj)
        {
            return (double?)obj.GetValue(MinValueProperty);
        }

        /// <summary>
        /// 设置最小值。
        /// </summary>
        public static void SetMinValue(DependencyObject obj, double? value)
        {
            obj.SetValue(MinValueProperty, value);
        }

        #endregion

        #region MaxValue 附加属性

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.RegisterAttached(
                "MaxValue",
                typeof(double?),
                typeof(NumericInputBehavior),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取最大值。
        /// </summary>
        public static double? GetMaxValue(DependencyObject obj)
        {
            return (double?)obj.GetValue(MaxValueProperty);
        }

        /// <summary>
        /// 设置最大值。
        /// </summary>
        public static void SetMaxValue(DependencyObject obj, double? value)
        {
            obj.SetValue(MaxValueProperty, value);
        }

        #endregion

        #region DecimalPlaces 附加属性

        public static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.RegisterAttached(
                "DecimalPlaces",
                typeof(int),
                typeof(NumericInputBehavior),
                new PropertyMetadata(2));

        /// <summary>
        /// 获取小数位数。
        /// </summary>
        public static int GetDecimalPlaces(DependencyObject obj)
        {
            return (int)obj.GetValue(DecimalPlacesProperty);
        }

        /// <summary>
        /// 设置小数位数。
        /// </summary>
        public static void SetDecimalPlaces(DependencyObject obj, int value)
        {
            obj.SetValue(DecimalPlacesProperty, value);
        }

        #endregion

        #region AllowNegative 附加属性

        public static readonly DependencyProperty AllowNegativeProperty =
            DependencyProperty.RegisterAttached(
                "AllowNegative",
                typeof(bool),
                typeof(NumericInputBehavior),
                new PropertyMetadata(true));

        /// <summary>
        /// 获取是否允许负数。
        /// </summary>
        public static bool GetAllowNegative(DependencyObject obj)
        {
            return (bool)obj.GetValue(AllowNegativeProperty);
        }

        /// <summary>
        /// 设置是否允许负数。
        /// </summary>
        public static void SetAllowNegative(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowNegativeProperty, value);
        }

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
            {
                AssociatedObject.PreviewTextInput += OnPreviewTextInput;
                AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
                DataObject.AddPastingHandler(AssociatedObject, OnPaste);
            }
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
                AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
                DataObject.RemovePastingHandler(AssociatedObject, OnPaste);
            }
            base.OnDetaching();
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (AssociatedObject == null)
                return;

            var textBox = AssociatedObject;
            var fullText = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength)
                                        .Insert(textBox.SelectionStart, e.Text);

            e.Handled = !IsValidInput(fullText);
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // 允许控制键
            if (e.Key == Key.Back ||
                e.Key == Key.Delete ||
                e.Key == Key.Tab ||
                e.Key == Key.Enter ||
                e.Key == Key.Escape ||
                e.Key == Key.Home ||
                e.Key == Key.End ||
                (e.Key == Key.Left && Keyboard.Modifiers == ModifierKeys.None) ||
                (e.Key == Key.Right && Keyboard.Modifiers == ModifierKeys.None))
            {
                return;
            }

            // 处理数字小键盘
            if ((Key.D0 <= e.Key && e.Key <= Key.D9) ||
                (Key.NumPad0 <= e.Key && e.Key <= Key.NumPad9))
            {
                return;
            }

            // 处理小数点和负号
            if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
            {
                if (AssociatedObject != null && AssociatedObject.Text.Contains('.'))
                    e.Handled = true;
                return;
            }

            if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
            {
                if (!GetAllowNegative(AssociatedObject!) ||
                    (AssociatedObject != null && AssociatedObject.SelectionStart != 0))
                    e.Handled = true;
                return;
            }

            e.Handled = true;
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                var text = e.DataObject.GetData(DataFormats.Text) as string;
                if (!string.IsNullOrEmpty(text) && AssociatedObject != null)
                {
                    var fullText = AssociatedObject.Text.Remove(AssociatedObject.SelectionStart, AssociatedObject.SelectionLength)
                                                        .Insert(AssociatedObject.SelectionStart, text);

                    if (!IsValidInput(fullText))
                    {
                        e.CancelCommand();
                    }
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private bool IsValidInput(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return true;

            if (!double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
                return false;

            var min = GetMinValue(AssociatedObject!);
            var max = GetMaxValue(AssociatedObject!);

            if (min.HasValue && value < min.Value)
                return false;

            if (max.HasValue && value > max.Value)
                return false;

            return true;
        }
    }
}
