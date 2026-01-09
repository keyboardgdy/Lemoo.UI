using Lemoo.Core.Domain.DomainEvents;

namespace Lemoo.Modules.TaskManager.Domain.DomainEvents;

/// <summary>
/// 任务更新领域事件
/// </summary>
public class TaskUpdatedEvent : DomainEventBase
{
    public Guid TaskId { get; }
    public string Title { get; }
    
    public TaskUpdatedEvent(Guid taskId, string title)
    {
        TaskId = taskId;
        Title = title;
    }
}
