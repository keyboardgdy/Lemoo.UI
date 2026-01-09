using Lemoo.Core.Domain.DomainEvents;

namespace Lemoo.Modules.TaskManager.Domain.DomainEvents;

/// <summary>
/// 任务完成领域事件
/// </summary>
public class TaskCompletedEvent : DomainEventBase
{
    public Guid TaskId { get; }
    public string Title { get; }
    public DateTime CompletedAt { get; }
    
    public TaskCompletedEvent(Guid taskId, string title, DateTime completedAt)
    {
        TaskId = taskId;
        Title = title;
        CompletedAt = completedAt;
    }
}
