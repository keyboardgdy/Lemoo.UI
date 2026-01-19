using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Lemoo.UI.Interactivity;

namespace Lemoo.UI.Behaviors
{
    /// <summary>
    /// 管理焦点控制的行为。
    /// </summary>
    /// <remarks>
    /// 线程安全性: 静态方法中使用了try-catch来确保异常不会导致应用程序崩溃。
    /// 资源管理: 在元素Unloaded时会自动清理所有事件订阅，防止内存泄漏。
    /// </remarks>
    public class FocusBehavior : Behavior<FrameworkElement>
    {
        #region IsFocused 附加属性

        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached(
                "IsFocused",
                typeof(bool),
                typeof(FocusBehavior),
                new PropertyMetadata(false, OnIsFocusedChanged));

        /// <summary>
        /// 获取元素是否获得焦点。
        /// </summary>
        public static bool GetIsFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusedProperty);
        }

        /// <summary>
        /// 设置元素是否获得焦点。
        /// </summary>
        public static void SetIsFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }

        private static void OnIsFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element)
                return;

            try
            {
                // 先取消订阅，防止重复订阅
                DetachIsFocused(element);

                if ((bool)e.NewValue)
                {
                    element.Loaded += OnElementLoaded;
                    element.Unloaded += OnElementUnloaded;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FocusBehavior: OnIsFocusedChanged failed: {ex.Message}");
            }
        }

        private static void DetachIsFocused(FrameworkElement element)
        {
            try
            {
                element.Loaded -= OnElementLoaded;
                element.Unloaded -= OnElementUnloaded;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FocusBehavior: DetachIsFocused failed: {ex.Message}");
            }
        }

        private static void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                try
                {
                    element.Focus();
                    if (element is TextBox textBox)
                    {
                        textBox.SelectAll();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"FocusBehavior: OnElementLoaded failed: {ex.Message}");
                }
            }
        }

        private static void OnElementUnloaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                // 清理事件订阅，防止内存泄漏
                DetachIsFocused(element);
            }
        }

        #endregion

        #region FocusOnLoad 附加属性

        public static readonly DependencyProperty FocusOnLoadProperty =
            DependencyProperty.RegisterAttached(
                "FocusOnLoad",
                typeof(bool),
                typeof(FocusBehavior),
                new PropertyMetadata(false, OnFocusOnLoadChanged));

        /// <summary>
        /// 获取元素是否在加载时获得焦点。
        /// </summary>
        public static bool GetFocusOnLoad(DependencyObject obj)
        {
            return (bool)obj.GetValue(FocusOnLoadProperty);
        }

        /// <summary>
        /// 设置元素是否在加载时获得焦点。
        /// </summary>
        public static void SetFocusOnLoad(DependencyObject obj, bool value)
        {
            obj.SetValue(FocusOnLoadProperty, value);
        }

        private static void OnFocusOnLoadChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element)
                return;

            try
            {
                // 先取消订阅，防止重复订阅
                DetachFocusOnLoad(element);

                if ((bool)e.NewValue)
                {
                    element.Loaded += OnElementFocusOnLoad;
                    element.Unloaded += OnElementFocusOnLoadUnloaded;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FocusBehavior: OnFocusOnLoadChanged failed: {ex.Message}");
            }
        }

        private static void DetachFocusOnLoad(FrameworkElement element)
        {
            try
            {
                element.Loaded -= OnElementFocusOnLoad;
                element.Unloaded -= OnElementFocusOnLoadUnloaded;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FocusBehavior: DetachFocusOnLoad failed: {ex.Message}");
            }
        }

        private static void OnElementFocusOnLoad(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                try
                {
                    element.Focus();
                    if (element is TextBox textBox)
                    {
                        textBox.SelectAll();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"FocusBehavior: OnElementFocusOnLoad failed: {ex.Message}");
                }
            }
        }

        private static void OnElementFocusOnLoadUnloaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                // 清理事件订阅，防止内存泄漏
                DetachFocusOnLoad(element);
            }
        }

        #endregion

        #region SelectAllOnFocus 附加属性

        public static readonly DependencyProperty SelectAllOnFocusProperty =
            DependencyProperty.RegisterAttached(
                "SelectAllOnFocus",
                typeof(bool),
                typeof(FocusBehavior),
                new PropertyMetadata(false, OnSelectAllOnFocusChanged));

        /// <summary>
        /// 获取文本框在获得焦点时是否选中全部文本。
        /// </summary>
        public static bool GetSelectAllOnFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectAllOnFocusProperty);
        }

        /// <summary>
        /// 设置文本框在获得焦点时是否选中全部文本。
        /// </summary>
        public static void SetSelectAllOnFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectAllOnFocusProperty, value);
        }

        private static void OnSelectAllOnFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox textBox)
                return;

            try
            {
                // 先取消订阅，防止重复订阅
                DetachSelectAllOnFocus(textBox);

                if ((bool)e.NewValue)
                {
                    textBox.GotFocus += OnTextBoxGotFocus;
                    textBox.Unloaded += OnTextBoxUnloaded;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FocusBehavior: OnSelectAllOnFocusChanged failed: {ex.Message}");
            }
        }

        private static void DetachSelectAllOnFocus(TextBox textBox)
        {
            try
            {
                textBox.GotFocus -= OnTextBoxGotFocus;
                textBox.Unloaded -= OnTextBoxUnloaded;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FocusBehavior: DetachSelectAllOnFocus failed: {ex.Message}");
            }
        }

        private static void OnTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                try
                {
                    textBox.SelectAll();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"FocusBehavior: OnTextBoxGotFocus failed: {ex.Message}");
                }
            }
        }

        private static void OnTextBoxUnloaded(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // 清理事件订阅，防止内存泄漏
                DetachSelectAllOnFocus(textBox);
            }
        }

        #endregion
    }
}
