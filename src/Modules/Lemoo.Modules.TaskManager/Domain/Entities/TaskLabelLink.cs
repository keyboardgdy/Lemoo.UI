namespace Lemoo.Modules.TaskManager.Domain.Entities;

/// <summary>
/// 任务-标签关联实体
/// </summary>
public class TaskLabelLink
{
    public Guid TaskId { get; set; }
    public Guid LabelId { get; set; }
    public DateTime AssignedAt { get; set; }

    /// <summary>
    /// 导航属性 - 任务
    /// </summary>
    public Task? Task { get; set; }

    /// <summary>
    /// 导航属性 - 标签
    /// </summary>
    public TaskLabel? Label { get; set; }

    public TaskLabelLink()
    {
        AssignedAt = DateTime.UtcNow;
    }

    public TaskLabelLink(Guid taskId, Guid labelId)
    {
        TaskId = taskId;
        LabelId = labelId;
        AssignedAt = DateTime.UtcNow;
    }
}
