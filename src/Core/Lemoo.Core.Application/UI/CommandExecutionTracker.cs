using Microsoft.Extensions.Logging;

namespace Lemoo.Core.Application.UI;

/// <summary>
/// 命令执行追踪器实现
/// </summary>
public class CommandExecutionTracker : ICommandExecutionTracker
{
    private readonly ILogger<CommandExecutionTracker> _logger;
    private readonly Stack<string> _commandStack = new();
    
    public event EventHandler<CommandExecutionEventArgs>? CommandStarted;
    public event EventHandler<CommandExecutionEventArgs>? CommandCompleted;
    public event EventHandler<CommandExecutionEventArgs>? CommandFailed;
    
    public bool IsExecuting => _commandStack.Count > 0;
    public string? CurrentCommandName => _commandStack.TryPeek(out var name) ? name : null;

    public CommandExecutionTracker(ILogger<CommandExecutionTracker> logger)
    {
        _logger = logger;
    }

    public IDisposable TrackCommand(string commandName)
    {
        return new CommandTracker(this, commandName);
    }

    private class CommandTracker : IDisposable
    {
        private readonly CommandExecutionTracker _tracker;
        private readonly string _commandName;
        private readonly DateTime _startTime;
        private bool _disposed;

        public CommandTracker(CommandExecutionTracker tracker, string commandName)
        {
            _tracker = tracker;
            _commandName = commandName;
            _startTime = DateTime.UtcNow;
            
            _tracker._commandStack.Push(_commandName);
            var args = new CommandExecutionEventArgs(_commandName, _startTime);
            _tracker.CommandStarted?.Invoke(_tracker, args);
            _tracker._logger.LogDebug("命令开始执行: {CommandName}", _commandName);
        }

        public void Complete(bool isSuccess = true, string? errorMessage = null)
        {
            if (_disposed)
                return;

            var endTime = DateTime.UtcNow;
            var args = new CommandExecutionEventArgs(_commandName, _startTime, endTime, isSuccess, errorMessage);
            
            if (isSuccess)
            {
                _tracker.CommandCompleted?.Invoke(_tracker, args);
                _tracker._logger.LogDebug("命令执行完成: {CommandName}, 耗时: {Duration}ms", 
                    _commandName, args.Duration?.TotalMilliseconds);
            }
            else
            {
                _tracker.CommandFailed?.Invoke(_tracker, args);
                _tracker._logger.LogWarning("命令执行失败: {CommandName}, 错误: {ErrorMessage}, 耗时: {Duration}ms", 
                    _commandName, errorMessage, args.Duration?.TotalMilliseconds);
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            // 如果还没有完成，自动标记为完成（可能因为异常导致）
            if (!_disposed)
            {
                Complete(); // 确保状态一致性
            }

            if (_tracker._commandStack.Count > 0 && _tracker._commandStack.Peek() == _commandName)
            {
                _tracker._commandStack.Pop();
            }

            _disposed = true;
        }
    }
}

