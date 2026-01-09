namespace Lemoo.Modules.TaskManager.Application.DTOs;

/// <summary>
/// 任务数据传输对象
/// </summary>
public class TaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Priority { get; set; }  // TaskPriority 枚举值
    public string PriorityName { get; set; } = string.Empty;  // "低"、"中"、"高"
    public int Status { get; set; }  // TaskStatus 枚举值
    public string StatusName { get; set; } = string.Empty;  // "待办"、"进行中"、"已完成"
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsOverdue { get; set; }  // 是否过期
}
