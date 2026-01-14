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

            if ((bool)e.NewValue)
            {
                element.Loaded += OnElementLoaded;
            }
            else
            {
                element.Loaded -= OnElementLoaded;
            }
        }

        private static void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                element.Loaded -= OnElementLoaded;
                element.Focus();
                if (element is TextBox textBox)
                {
                    textBox.SelectAll();
                }
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

            if ((bool)e.NewValue)
            {
                element.Loaded += OnElementFocusOnLoad;
            }
            else
            {
                element.Loaded -= OnElementFocusOnLoad;
            }
        }

        private static void OnElementFocusOnLoad(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                element.Focus();
                if (element is TextBox textBox)
                {
                    textBox.SelectAll();
                }
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

            if ((bool)e.NewValue)
            {
                textBox.GotFocus += OnTextBoxGotFocus;
            }
            else
            {
                textBox.GotFocus -= OnTextBoxGotFocus;
            }
        }

        private static void OnTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        #endregion
    }
}
