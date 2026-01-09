using Lemoo.Core.Domain.DomainEvents;

namespace Lemoo.Modules.TaskManager.Domain.DomainEvents;

/// <summary>
/// 任务删除领域事件
/// </summary>
public class TaskDeletedEvent : DomainEventBase
{
    public Guid TaskId { get; }
    public string Title { get; }
    
    public TaskDeletedEvent(Guid taskId, string title)
    {
        TaskId = taskId;
        Title = title;
    }
}
