namespace Lemoo.Modules.TaskManager.Application.DTOs;

/// <summary>
/// 任务统计数据传输对象
/// </summary>
public class TaskStatisticsDto
{
    /// <summary>
    /// 总任务数
    /// </summary>
    public int TotalTasks { get; set; }

    /// <summary>
    /// 待办任务数
    /// </summary>
    public int PendingTasks { get; set; }

    /// <summary>
    /// 进行中任务数
    /// </summary>
    public int InProgressTasks { get; set; }

    /// <summary>
    /// 已完成任务数
    /// </summary>
    public int CompletedTasks { get; set; }

    /// <summary>
    /// 过期任务数
    /// </summary>
    public int OverdueTasks { get; set; }

    /// <summary>
    /// 高优先级任务数
    /// </summary>
    public int HighPriorityTasks { get; set; }

    /// <summary>
    /// 完成率（百分比）
    /// </summary>
    public double CompletionRate { get; set; }

    /// <summary>
    /// 按状态分组的任务数
    /// </summary>
    public Dictionary<string, int> TasksByStatus { get; set; } = new();

    /// <summary>
    /// 按优先级分组的任务数
    /// </summary>
    public Dictionary<string, int> TasksByPriority { get; set; } = new();

    /// <summary>
    /// 最近7天每天完成任务数
    /// </summary>
    public List<DailyTaskCountDto> DailyCompletedTasks { get; set; } = new();
}

/// <summary>
/// 每日任务计数
/// </summary>
public class DailyTaskCountDto
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}
