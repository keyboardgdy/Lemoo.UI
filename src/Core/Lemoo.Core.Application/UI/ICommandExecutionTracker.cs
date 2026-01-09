namespace Lemoo.Core.Application.UI;

/// <summary>
/// 命令执行追踪器接口 - 用于追踪命令执行状态
/// </summary>
public interface ICommandExecutionTracker
{
    /// <summary>
    /// 命令开始执行事件
    /// </summary>
    event EventHandler<CommandExecutionEventArgs>? CommandStarted;
    
    /// <summary>
    /// 命令执行完成事件
    /// </summary>
    event EventHandler<CommandExecutionEventArgs>? CommandCompleted;
    
    /// <summary>
    /// 命令执行失败事件
    /// </summary>
    event EventHandler<CommandExecutionEventArgs>? CommandFailed;
    
    /// <summary>
    /// 是否正在执行命令
    /// </summary>
    bool IsExecuting { get; }
    
    /// <summary>
    /// 当前执行的命令名称
    /// </summary>
    string? CurrentCommandName { get; }
    
    /// <summary>
    /// 开始追踪命令执行
    /// </summary>
    IDisposable TrackCommand(string commandName);
}

/// <summary>
/// 命令执行事件参数
/// </summary>
public class CommandExecutionEventArgs : EventArgs
{
    public string CommandName { get; }
    public DateTime StartTime { get; }
    public DateTime? EndTime { get; }
    public TimeSpan? Duration => EndTime.HasValue ? EndTime.Value - StartTime : null;
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }

    public CommandExecutionEventArgs(
        string commandName, 
        DateTime startTime, 
        DateTime? endTime = null, 
        bool isSuccess = true, 
        string? errorMessage = null)
    {
        CommandName = commandName;
        StartTime = startTime;
        EndTime = endTime;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }
}

