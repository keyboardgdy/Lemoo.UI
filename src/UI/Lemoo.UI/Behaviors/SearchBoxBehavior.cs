using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Lemoo.UI.Behaviors
{
    /// <summary>
    /// 搜索框行为，提供清除功能
    /// </summary>
    public static class SearchBoxBehavior
    {
        #region ClearCommand 附加属性

        public static readonly DependencyProperty ClearCommandProperty =
            DependencyProperty.RegisterAttached(
                "ClearCommand",
                typeof(ICommand),
                typeof(SearchBoxBehavior),
                new PropertyMetadata(null, OnClearCommandChanged));

        public static ICommand GetClearCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ClearCommandProperty);
        }

        public static void SetClearCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ClearCommandProperty, value);
        }

        private static void OnClearCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if (e.OldValue is ICommand oldCommand)
                {
                    // 清理旧的事件处理器
                }

                if (e.NewValue is ICommand newCommand)
                {
                    // 可以在这里添加初始化逻辑
                }
            }
        }

        #endregion

        #region IsClearButtonVisible 附加属性

        public static readonly DependencyProperty IsClearButtonVisibleProperty =
            DependencyProperty.RegisterAttached(
                "IsClearButtonVisible",
                typeof(bool),
                typeof(SearchBoxBehavior),
                new PropertyMetadata(false));

        public static bool GetIsClearButtonVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsClearButtonVisibleProperty);
        }

        public static void SetIsClearButtonVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(IsClearButtonVisibleProperty, value);
        }

        #endregion

        #region ExecuteClear 方法

        /// <summary>
        /// 执行清除操作
        /// </summary>
        public static void ExecuteClear(TextBox textBox)
        {
            var command = GetClearCommand(textBox);
            if (command != null && command.CanExecute(null))
            {
                command.Execute(null);
            }
            else
            {
                // 如果没有命令，直接清除文本
                textBox.Text = string.Empty;
            }
        }

        #endregion
    }
}
