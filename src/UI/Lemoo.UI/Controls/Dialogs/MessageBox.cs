using System;
using System.Windows;
using System.Windows.Media;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// MessageBox 消息框静态类，用于显示模态对话框。
    /// </summary>
    /// <remarks>
    /// MessageBox 提供了一种显示消息、获取用户输入的方式。
    /// 支持多种按钮组合、图标类型和自定义选项。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 显示简单的信息提示
    /// MessageBox.Show("操作成功完成！");
    ///
    /// // 显示带标题的信息提示
    /// MessageBox.Show("文件已保存", "提示");
    ///
    /// // 显示确认对话框
    /// var result = MessageBox.Show("确定要删除吗？", "确认", MessageBoxButton.YesNo);
    /// if (result == MessageBoxResult.Yes)
    /// {
    ///     // 用户点击了"是"
    /// }
    ///
    /// // 显示错误消息
    /// MessageBox.Show("操作失败，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
    ///
    /// // 显示带复选框的消息框
    /// var result = MessageBox.Show("不再提示此消息", "提示",
    ///     MessageBoxButton.OK, MessageBoxImage.Information, "不再提示");
    /// if (result.WasOptionChecked)
    /// {
    ///     // 用户勾选了"不再提示"
    /// }
    /// </code>
    /// </example>
    public static class MessageBox
    {
        #region 显示方法

        /// <summary>
        /// 显示消息框。
        /// </summary>
        /// <param name="messageText">消息文本。</param>
        /// <returns>消息框结果。</returns>
        public static MessageBoxResult Show(string messageText)
        {
            return Show(messageText, string.Empty, MessageBoxButton.OK, MessageBoxImage.None);
        }

        /// <summary>
        /// 显示消息框。
        /// </summary>
        /// <param name="messageText">消息文本。</param>
        /// <param name="caption">标题。</param>
        /// <returns>消息框结果。</returns>
        public static MessageBoxResult Show(string messageText, string caption)
        {
            return Show(messageText, caption, MessageBoxButton.OK, MessageBoxImage.None);
        }

        /// <summary>
        /// 显示消息框。
        /// </summary>
        /// <param name="messageText">消息文本。</param>
        /// <param name="caption">标题。</param>
        /// <param name="button">按钮类型。</param>
        /// <returns>消息框结果。</returns>
        public static MessageBoxResult Show(string messageText, string caption, MessageBoxButton button)
        {
            return Show(messageText, caption, button, MessageBoxImage.None);
        }

        /// <summary>
        /// 显示消息框。
        /// </summary>
        /// <param name="messageText">消息文本。</param>
        /// <param name="caption">标题。</param>
        /// <param name="button">按钮类型。</param>
        /// <param name="icon">图标类型。</param>
        /// <returns>消息框结果。</returns>
        public static MessageBoxResult Show(string messageText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return Show(messageText, caption, button, icon, null);
        }

        /// <summary>
        /// 显示消息框。
        /// </summary>
        /// <param name="messageText">消息文本。</param>
        /// <param name="caption">标题。</param>
        /// <param name="button">按钮类型。</param>
        /// <param name="icon">图标类型。</param>
        /// <param name="optionText">复选框文本（可选）。</param>
        /// <returns>消息框结果。</returns>
        public static MessageBoxResultEx Show(string messageText, string caption, MessageBoxButton button, MessageBoxImage icon, string? optionText)
        {
            var window = new Dialogs.MessageBoxWindow
            {
                Owner = Application.Current.MainWindow
            };

            window.Initialize(messageText, caption, button, icon, optionText);
            window.ShowDialog();

            return new MessageBoxResultEx(window.Result, window.IsOptionChecked);
        }

        #endregion

        #region 便捷方法

        /// <summary>
        /// 显示信息提示框。
        /// </summary>
        /// <param name="message">消息文本。</param>
        /// <param name="caption">标题。</param>
        /// <returns>消息框结果。</returns>
        public static MessageBoxResult Information(string message, string caption = "提示")
        {
            return Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// 显示成功提示框。
        /// </summary>
        /// <param name="message">消息文本。</param>
        /// <param name="caption">标题。</param>
        /// <returns>消息框结果。</returns>
        public static MessageBoxResult Success(string message, string caption = "成功")
        {
            return Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// 显示警告提示框。
        /// </summary>
        /// <param name="message">消息文本。</param>
        /// <param name="caption">标题。</param>
        /// <returns>消息框结果。</returns>
        public static MessageBoxResult Warning(string message, string caption = "警告")
        {
            return Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// 显示错误提示框。
        /// </summary>
        /// <param name="message">消息文本。</param>
        /// <param name="caption">标题。</param>
        /// <returns>消息框结果。</returns>
        public static MessageBoxResult Error(string message, string caption = "错误")
        {
            return Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// 显示确认对话框。
        /// </summary>
        /// <param name="message">消息文本。</param>
        /// <param name="caption">标题。</param>
        /// <returns>如果用户点击"是"则返回 true，否则返回 false。</returns>
        public static bool Confirm(string message, string caption = "确认")
        {
            var result = Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        /// <summary>
        /// 显示确认对话框（带取消按钮）。
        /// </summary>
        /// <param name="message">消息文本。</param>
        /// <param name="caption">标题。</param>
        /// <returns>消息框结果。</returns>
        public static MessageBoxResult ConfirmCancel(string message, string caption = "确认")
        {
            return Show(message, caption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        }

        #endregion
    }

    /// <summary>
    /// 消息框结果扩展类，包含选项复选框状态。
    /// </summary>
    public class MessageBoxResultEx
    {
        /// <summary>
        /// 获取消息框结果。
        /// </summary>
        public MessageBoxResult Result { get; }

        /// <summary>
        /// 获取选项复选框是否被选中。
        /// </summary>
        public bool WasOptionChecked { get; }

        /// <summary>
        /// 初始化 MessageBoxResultEx 的新实例。
        /// </summary>
        /// <param name="result">消息框结果。</param>
        /// <param name="wasOptionChecked">选项复选框是否被选中。</param>
        public MessageBoxResultEx(MessageBoxResult result, bool wasOptionChecked)
        {
            Result = result;
            WasOptionChecked = wasOptionChecked;
        }

        /// <summary>
        /// 隐式转换为 MessageBoxResult。
        /// </summary>
        /// <param name="resultEx">MessageBoxResultEx 实例。</param>
        public static implicit operator MessageBoxResult(MessageBoxResultEx resultEx)
        {
            return resultEx.Result;
        }

        /// <summary>
        /// 检查结果是否为指定值。
        /// </summary>
        /// <param name="result">要比较的结果。</param>
        /// <returns>如果结果匹配则返回 true，否则返回 false。</returns>
        public bool Is(MessageBoxResult result)
        {
            return Result == result;
        }
    }

    /// <summary>
    /// 消息框按钮类型。
    /// </summary>
    public enum MessageBoxButton
    {
        /// <summary>
        /// 确定按钮。
        /// </summary>
        OK = 0,

        /// <summary>
        /// 确定和取消按钮。
        /// </summary>
        OKCancel = 1,

        /// <summary>
        /// 是和否按钮。
        /// </summary>
        YesNo = 4,

        /// <summary>
        /// 是、否和取消按钮。
        /// </summary>
        YesNoCancel = 3
    }

    /// <summary>
    /// 消息框图标类型。
    /// </summary>
    public enum MessageBoxImage
    {
        /// <summary>
        /// 无图标。
        /// </summary>
        None = 0,

        /// <summary>
        /// 错误图标。
        /// </summary>
        Error = 16,

        /// <summary>
        /// 问号图标。
        /// </summary>
        Question = 32,

        /// <summary>
        /// 警告图标。
        /// </summary>
        Warning = 48,

        /// <summary>
        /// 信息图标。
        /// </summary>
        Information = 64
    }

    /// <summary>
    /// 消息框结果。
    /// </summary>
    public enum MessageBoxResult
    {
        /// <summary>
        /// 无结果。
        /// </summary>
        None = 0,

        /// <summary>
        /// 确定。
        /// </summary>
        OK = 1,

        /// <summary>
        /// 取消。
        /// </summary>
        Cancel = 2,

        /// <summary>
        /// 是。
        /// </summary>
        Yes = 6,

        /// <summary>
        /// 否。
        /// </summary>
        No = 7
    }
}
