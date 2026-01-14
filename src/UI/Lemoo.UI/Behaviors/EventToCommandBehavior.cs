using System;
using System.Windows;
using System.Windows.Input;
using Lemoo.UI.Interactivity;

namespace Lemoo.UI.Behaviors
{
    /// <summary>
    /// 将事件绑定到命令的行为。
    /// </summary>
    public class EventToCommandBehavior : Behavior<FrameworkElement>
    {
        private Delegate? _eventHandler;

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
            UnregisterEvent();
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

            var eventInfo = AssociatedObject.GetType().GetEvent(EventName);
            eventInfo?.RemoveEventHandler(AssociatedObject, _eventHandler);

            _eventHandler = null;
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
    }
}
