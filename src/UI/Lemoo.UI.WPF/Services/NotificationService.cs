using System.ComponentModel;
using System.Windows;

namespace Lemoo.UI.WPF.Services
{
    /// <summary>
    /// 简单的内存通知服务
    /// 可以后续扩展为Toast通知
    /// </summary>
    public class NotificationService : INotificationService
    {
        private string? _currentMessage;
        private NotificationType _currentType;
        private bool _isVisible;

        /// <summary>
        /// 当前通知消息
        /// </summary>
        public string? CurrentMessage
        {
            get => _currentMessage;
            private set
            {
                _currentMessage = value;
                OnPropertyChanged(nameof(CurrentMessage));
            }
        }

        /// <summary>
        /// 当前通知类型
        /// </summary>
        public NotificationType CurrentType
        {
            get => _currentType;
            private set
            {
                _currentType = value;
                OnPropertyChanged(nameof(CurrentType));
            }
        }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            private set
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc/>
        public void Show(string message, NotificationType type = NotificationType.Info, int duration = 2000)
        {
            CurrentMessage = message;
            CurrentType = type;
            IsVisible = true;

            // 自动隐藏
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = System.TimeSpan.FromMilliseconds(duration)
            };
            timer.Tick += (s, e) =>
            {
                IsVisible = false;
                timer.Stop();
            };
            timer.Start();
        }

        /// <inheritdoc/>
        public void ShowSuccess(string message, int duration = 2000)
        {
            Show(message, NotificationType.Success, duration);
        }

        /// <inheritdoc/>
        public void ShowError(string message, int duration = 3000)
        {
            Show(message, NotificationType.Error, duration);
        }

        /// <inheritdoc/>
        public void ShowWarning(string message, int duration = 2000)
        {
            Show(message, NotificationType.Warning, duration);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
