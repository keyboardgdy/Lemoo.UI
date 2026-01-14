using System;
using System.Windows.Input;

namespace Lemoo.UI.Commands
{
    /// <summary>
    /// 简单的命令实现。
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?>? _execute;
        private readonly Func<object?, bool>? _canExecute;

        /// <summary>
        /// 初始化 <see cref="RelayCommand"/> 类的新实例。
        /// </summary>
        /// <param name="execute">执行委托</param>
        public RelayCommand(Action<object?>? execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// 初始化 <see cref="RelayCommand"/> 类的新实例。
        /// </summary>
        /// <param name="execute">执行委托</param>
        /// <param name="canExecute">是否可执行委托</param>
        public RelayCommand(Action<object?>? execute, Func<object?, bool>? canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// 发生更改时调用。
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// 确定命令是否可以执行。
        /// </summary>
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// 执行命令。
        /// </summary>
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }

    /// <summary>
    /// 简单的命令实现。
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T?>? _execute;
        private readonly Func<T?, bool>? _canExecute;

        /// <summary>
        /// 初始化 <see cref="RelayCommand{T}"/> 类的新实例。
        /// </summary>
        /// <param name="execute">执行委托</param>
        public RelayCommand(Action<T?>? execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// 初始化 <see cref="RelayCommand{T}"/> 类的新实例。
        /// </summary>
        /// <param name="execute">执行委托</param>
        /// <param name="canExecute">是否可执行委托</param>
        public RelayCommand(Action<T?>? execute, Func<T?, bool>? canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// 发生更改时调用。
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// 确定命令是否可以执行。
        /// </summary>
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute((T?)parameter);
        }

        /// <summary>
        /// 执行命令。
        /// </summary>
        public void Execute(object? parameter)
        {
            _execute((T?)parameter);
        }
    }

    /// <summary>
    /// 中继命令接口。
    /// </summary>
    public interface IRelayCommand : ICommand
    {
        /// <summary>
        /// 引发 <see cref="CanExecuteChanged"/> 事件。
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
