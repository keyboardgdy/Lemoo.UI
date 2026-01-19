using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Lemoo.UI.Behaviors
{
    /// <summary>
    /// TextBox 辅助类，提供搜索框相关的附加属性
    /// </summary>
    public static class TextBoxHelper
    {
        #region PlaceholderText 附加属性

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.RegisterAttached(
                "PlaceholderText",
                typeof(string),
                typeof(TextBoxHelper),
                new PropertyMetadata(string.Empty, OnPlaceholderTextChanged));

        /// <summary>
        /// 获取占位符文本
        /// </summary>
        public static string GetPlaceholderText(DependencyObject obj)
        {
            return (string)obj.GetValue(PlaceholderTextProperty);
        }

        /// <summary>
        /// 设置占位符文本
        /// </summary>
        public static void SetPlaceholderText(DependencyObject obj, string value)
        {
            obj.SetValue(PlaceholderTextProperty, value);
        }

        private static void OnPlaceholderTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                // 更新 Tag 属性，用于模板绑定
                textBox.Tag = e.NewValue;
            }
        }

        #endregion

        #region ClearCommand 附加属性

        public static readonly DependencyProperty ClearCommandProperty =
            DependencyProperty.RegisterAttached(
                "ClearCommand",
                typeof(ICommand),
                typeof(TextBoxHelper),
                new PropertyMetadata(null, OnClearCommandChanged));

        /// <summary>
        /// 获取清除命令
        /// </summary>
        public static ICommand GetClearCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ClearCommandProperty);
        }

        /// <summary>
        /// 设置清除命令
        /// </summary>
        public static void SetClearCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ClearCommandProperty, value);
        }

        private static void OnClearCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                // 存储命令以备后用
                textBox.SetValue(ClearCommandProperty, e.NewValue);
            }
        }

        #endregion

        #region HasText 附加属性（只读）

        private static readonly DependencyPropertyKey HasTextPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "HasText",
                typeof(bool),
                typeof(TextBoxHelper),
                new PropertyMetadata(false));

        public static readonly DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;

        /// <summary>
        /// 获取文本框是否有文本
        /// </summary>
        public static bool GetHasText(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasTextProperty);
        }

        private static void SetHasText(DependencyObject obj, bool value)
        {
            obj.SetValue(HasTextPropertyKey, value);
        }

        #endregion

        #region TrackHasText 附加属性

        public static readonly DependencyProperty TrackHasTextProperty =
            DependencyProperty.RegisterAttached(
                "TrackHasText",
                typeof(bool),
                typeof(TextBoxHelper),
                new PropertyMetadata(false, OnTrackHasTextChanged));

        /// <summary>
        /// 获取是否跟踪文本状态
        /// </summary>
        public static bool GetTrackHasText(DependencyObject obj)
        {
            return (bool)obj.GetValue(TrackHasTextProperty);
        }

        /// <summary>
        /// 设置是否跟踪文本状态
        /// </summary>
        public static void SetTrackHasText(DependencyObject obj, bool value)
        {
            obj.SetValue(TrackHasTextProperty, value);
        }

        private static void OnTrackHasTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool)e.NewValue)
                {
                    textBox.TextChanged += OnTextBoxTextChanged;
                    // 初始化状态
                    UpdateHasText(textBox);
                }
                else
                {
                    textBox.TextChanged -= OnTextBoxTextChanged;
                }
            }
        }

        private static void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                UpdateHasText(textBox);
            }
        }

        private static void UpdateHasText(TextBox textBox)
        {
            SetHasText(textBox, !string.IsNullOrWhiteSpace(textBox.Text));
        }

        #endregion

        #region ExecuteClearCommand 辅助方法

        /// <summary>
        /// 执行清除命令（供模板中的清除按钮调用）
        /// </summary>
        public static void ExecuteClearCommand(TextBox textBox)
        {
            var command = GetClearCommand(textBox);
            if (command != null && command.CanExecute(textBox.Text))
            {
                command.Execute(textBox.Text);
            }
        }

        #endregion

        #region SelectAllOnFocus 附加属性

        public static readonly DependencyProperty SelectAllOnFocusProperty =
            DependencyProperty.RegisterAttached(
                "SelectAllOnFocus",
                typeof(bool),
                typeof(TextBoxHelper),
                new PropertyMetadata(false, OnSelectAllOnFocusChanged));

        /// <summary>
        /// 获取是否在获得焦点时选中所有文本
        /// </summary>
        public static bool GetSelectAllOnFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectAllOnFocusProperty);
        }

        /// <summary>
        /// 设置是否在获得焦点时选中所有文本
        /// </summary>
        public static void SetSelectAllOnFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectAllOnFocusProperty, value);
        }

        private static void OnSelectAllOnFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool)e.NewValue)
                {
                    textBox.GotFocus += TextBox_GotFocus;
                }
                else
                {
                    textBox.GotFocus -= TextBox_GotFocus;
                }
            }
        }

        private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && !string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.SelectAll();
            }
        }

        #endregion
    }
}
