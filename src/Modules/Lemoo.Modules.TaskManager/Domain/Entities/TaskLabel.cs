using Lemoo.Core.Domain.Aggregates;

namespace Lemoo.Modules.TaskManager.Domain.Entities;

/// <summary>
/// 任务标签实体（聚合根）
/// </summary>
public class TaskLabel : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Color { get; private set; } = "#0078D4"; // 默认蓝色

    // EF Core 需要无参构造函数
    private TaskLabel() { }

    /// <summary>
    /// 创建标签（工厂方法）
    /// </summary>
    public static TaskLabel Create(string name, string? description, string color)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("标签名称不能为空", nameof(name));

        var label = new TaskLabel
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            Color = color,
            CreatedAt = DateTime.UtcNow
        };

        return label;
    }

    /// <summary>
    /// 更新标签信息
    /// </summary>
    public void Update(string name, string? description, string color)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("标签名称不能为空", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        Color = color;
        UpdatedAt = DateTime.UtcNow;
    }
}
