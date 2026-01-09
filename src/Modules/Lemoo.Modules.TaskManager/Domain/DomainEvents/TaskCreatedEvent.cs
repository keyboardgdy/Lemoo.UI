using Lemoo.Core.Domain.DomainEvents;

namespace Lemoo.Modules.TaskManager.Domain.DomainEvents;

/// <summary>
/// 任务创建领域事件
/// </summary>
public class TaskCreatedEvent : DomainEventBase
{
    public Guid TaskId { get; }
    public string Title { get; }
    
    public TaskCreatedEvent(Guid taskId, string title)
    {
        TaskId = taskId;
        Title = title;
    }
}
