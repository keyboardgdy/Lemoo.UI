namespace Lemoo.Modules.TaskManager.Application.DTOs;

/// <summary>
/// 任务标签数据传输对象
/// </summary>
public class TaskLabelDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
