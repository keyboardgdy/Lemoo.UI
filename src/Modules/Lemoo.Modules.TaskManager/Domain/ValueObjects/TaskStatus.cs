namespace Lemoo.Modules.TaskManager.Domain.ValueObjects;

/// <summary>
/// 任务状态值对象
/// </summary>
public enum TaskStatus
{
    Pending = 1,      // 待办
    InProgress = 2,   // 进行中
    Completed = 3     // 已完成
}
