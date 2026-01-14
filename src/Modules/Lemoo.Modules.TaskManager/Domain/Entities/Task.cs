using Lemoo.Core.Domain.Aggregates;
using TaskStatus = Lemoo.Modules.TaskManager.Domain.ValueObjects.TaskStatus;
using TaskPriority = Lemoo.Modules.TaskManager.Domain.ValueObjects.TaskPriority;
using Lemoo.Modules.TaskManager.Domain.DomainEvents;

namespace Lemoo.Modules.TaskManager.Domain.Entities;

/// <summary>
/// 任务实体（聚合根）
/// </summary>
public class Task : AggregateRoot
{
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TaskPriority Priority { get; private set; }
    public TaskStatus Status { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    /// <summary>
    /// 任务标签关联（导航属性）
    /// </summary>
    public ICollection<TaskLabelLink> Labels { get; private set; } = new List<TaskLabelLink>();

    // EF Core 需要无参构造函数
    private Task() { }
    
    /// <summary>
    /// 创建任务（工厂方法）
    /// </summary>
    public static Task Create(
        string title, 
        string? description, 
        TaskPriority priority, 
        DateTime? dueDate)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("任务标题不能为空", nameof(title));
        
        var task = new Task
        {
            Title = title.Trim(),
            Description = description?.Trim(),
            Priority = priority,
            Status = TaskStatus.Pending,
            DueDate = dueDate,
            CreatedAt = DateTime.UtcNow
        };
        
        // 发布领域事件
        task.AddDomainEvent(new TaskCreatedEvent(task.Id, task.Title));
        
        return task;
    }
    
    /// <summary>
    /// 更新任务信息
    /// </summary>
    public void Update(string title, string? description, TaskPriority priority, DateTime? dueDate)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("任务标题不能为空", nameof(title));
        
        if (Status == TaskStatus.Completed)
            throw new InvalidOperationException("已完成的任务不能修改");
        
        Title = title.Trim();
        Description = description?.Trim();
        Priority = priority;
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new TaskUpdatedEvent(Id, Title));
    }
    
    /// <summary>
    /// 开始任务
    /// </summary>
    public void Start()
    {
        if (Status == TaskStatus.Completed)
            throw new InvalidOperationException("已完成的任务不能重新开始");
        
        if (Status == TaskStatus.InProgress)
            return; // 已经是进行中状态
        
        Status = TaskStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new TaskUpdatedEvent(Id, Title));
    }
    
    /// <summary>
    /// 完成任务
    /// </summary>
    public void Complete()
    {
        if (Status == TaskStatus.Completed)
            return; // 已经完成
        
        Status = TaskStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new TaskCompletedEvent(Id, Title, CompletedAt.Value));
    }
    
    /// <summary>
    /// 重新打开任务
    /// </summary>
    public void Reopen()
    {
        if (Status != TaskStatus.Completed)
            return; // 只有已完成的任务才能重新打开
        
        Status = TaskStatus.Pending;
        CompletedAt = null;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new TaskUpdatedEvent(Id, Title));
    }
    
    /// <summary>
    /// 检查是否过期
    /// </summary>
    public bool IsOverdue()
    {
        return DueDate.HasValue 
            && DueDate.Value < DateTime.UtcNow 
            && Status != TaskStatus.Completed;
    }
}
