using System;
using System.Windows;
using System.Windows.Input;
using Lemoo.UI.Interactivity;

namespace Lemoo.UI.Behaviors
{
    /// <summary>
    /// 将事件绑定到命令的行为。
    /// </summary>
    public class EventToCommandBehavior : Behavior<FrameworkElement>, IDisposable
    {
        private Delegate? _eventHandler;
        private bool _disposed;

        /// <summary>
        /// 获取或设置要监听的事件名称。
        /// </summary>
        public string EventName { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置要执行的命令。
        /// </summary>
        public ICommand? Command { get; set; }

        /// <summary>
        /// 获取或设置命令参数。
        /// </summary>
        public object? CommandParameter { get; set; }

        /// <summary>
        /// 获取或设置是否将事件参数传递给命令。
        /// </summary>
        public bool PassEventArgsToCommand { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            RegisterEvent(EventName);
        }

        protected override void OnDetaching()
        {
            Dispose();
            base.OnDetaching();
        }

        private void RegisterEvent(string eventName)
        {
            if (string.IsNullOrEmpty(eventName) || AssociatedObject == null)
                return;

            UnregisterEvent();

            var eventInfo = AssociatedObject.GetType().GetEvent(eventName);
            if (eventInfo == null)
                throw new ArgumentException($"Event '{eventName}' not found on type '{AssociatedObject.GetType().Name}'");

            _eventHandler = Delegate.CreateDelegate(
                eventInfo.EventHandlerType!,
                this,
                nameof(OnEventRaised));

            eventInfo.AddEventHandler(AssociatedObject, _eventHandler);
        }

        private void UnregisterEvent()
        {
            if (_eventHandler == null || AssociatedObject == null)
                return;

            try
            {
                var eventInfo = AssociatedObject.GetType().GetEvent(EventName);
                eventInfo?.RemoveEventHandler(AssociatedObject, _eventHandler);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EventToCommandBehavior: UnregisterEvent failed: {ex.Message}");
            }
            finally
            {
                _eventHandler = null;
            }
        }

        private void OnEventRaised(object? sender, EventArgs e)
        {
            if (Command == null)
                return;

            var parameter = CommandParameter;
            if (PassEventArgsToCommand)
            {
                parameter = e;
            }

            if (Command.CanExecute(parameter))
            {
                Command.Execute(parameter);
            }
        }

        /// <summary>
        /// 释放行为占用的资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放行为占用的资源。
        /// </summary>
        /// <param name="disposing">是否正在释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // 清理托管资源
                UnregisterEvent();
            }

            _disposed = true;
        }

        /// <summary>
        /// 析构函数。
        /// </summary>
        ~EventToCommandBehavior()
        {
            Dispose(false);
        }
    }
}
