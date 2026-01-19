namespace Lemoo.UI.WPF.Services
{
    /// <summary>
    /// 通知消息类型
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// 信息
        /// </summary>
        Info,

        /// <summary>
        /// 成功
        /// </summary>
        Success,

        /// <summary>
        /// 警告
        /// </summary>
        Warning,

        /// <summary>
        /// 错误
        /// </summary>
        Error
    }

    /// <summary>
    /// 通知服务接口
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// 显示通知
        /// </summary>
        void Show(string message, NotificationType type = NotificationType.Info, int duration = 2000);

        /// <summary>
        /// 显示成功通知
        /// </summary>
        void ShowSuccess(string message, int duration = 2000);

        /// <summary>
        /// 显示错误通知
        /// </summary>
        void ShowError(string message, int duration = 3000);

        /// <summary>
        /// 显示警告通知
        /// </summary>
        void ShowWarning(string message, int duration = 2000);
    }
}
