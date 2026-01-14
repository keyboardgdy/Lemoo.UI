using Lemoo.Core.Abstractions.Persistence;
using Lemoo.Core.Application.Common;
using Lemoo.Modules.TaskManager.Application.DTOs;
using Lemoo.Modules.TaskManager.Application.Repositories;
using TaskEntity = Lemoo.Modules.TaskManager.Domain.Entities.Task;
using TaskStatus = Lemoo.Modules.TaskManager.Domain.ValueObjects.TaskStatus;
using TaskPriority = Lemoo.Modules.TaskManager.Domain.ValueObjects.TaskPriority;
using Lemoo.Modules.TaskManager.Infrastructure.Persistence;
using Lemoo.Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lemoo.Modules.TaskManager.Infrastructure.Repositories;

/// <summary>
/// 任务仓储实现
/// </summary>
public class TaskRepository : Repository<TaskEntity, Guid>, ITaskRepository
{
    public TaskRepository(TaskManagerDbContext context, ILogger<TaskRepository> logger) 
        : base(context, logger)
    {
    }
    
    protected TaskManagerDbContext TaskContext => (TaskManagerDbContext)DbContext;
    
    public async System.Threading.Tasks.Task<IEnumerable<TaskEntity>> SearchAsync(
        string? keyword = null,
        TaskStatus? status = null,
        TaskPriority? priority = null,
        CancellationToken cancellationToken = default)
    {
        var query = TaskContext.Tasks.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(t => 
                t.Title.Contains(keyword) || 
                (t.Description != null && t.Description.Contains(keyword)));
        }
        
        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }
        
        if (priority.HasValue)
        {
            query = query.Where(t => t.Priority == priority.Value);
        }
        
        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }
    
    public async System.Threading.Tasks.Task<PagedResult<TaskEntity>> SearchPagedAsync(
        string? keyword = null,
        TaskStatus? status = null,
        TaskPriority? priority = null,
        int pageIndex = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = TaskContext.Tasks.AsQueryable();
        
        // 应用筛选条件
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(t => 
                t.Title.Contains(keyword) || 
                (t.Description != null && t.Description.Contains(keyword)));
        }
        
        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }
        
        if (priority.HasValue)
        {
            query = query.Where(t => t.Priority == priority.Value);
        }
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return PagedResult<TaskEntity>.Create(items, pageIndex, pageSize, totalCount);
    }

    public async System.Threading.Tasks.Task<TaskStatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var sevenDaysAgo = now.AddDays(-7);

        var tasks = await TaskContext.Tasks.ToListAsync(cancellationToken);

        var totalTasks = tasks.Count;
        var pendingTasks = tasks.Count(t => t.Status == TaskStatus.Pending);
        var inProgressTasks = tasks.Count(t => t.Status == TaskStatus.InProgress);
        var completedTasks = tasks.Count(t => t.Status == TaskStatus.Completed);
        var overdueTasks = tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value < now && t.Status != TaskStatus.Completed);
        var highPriorityTasks = tasks.Count(t => t.Priority == TaskPriority.High);

        var completionRate = totalTasks > 0 ? (double)completedTasks / totalTasks * 100 : 0;

        var tasksByStatus = new Dictionary<string, int>
        {
            { "待办", pendingTasks },
            { "进行中", inProgressTasks },
            { "已完成", completedTasks }
        };

        var tasksByPriority = new Dictionary<string, int>
        {
            { "低", tasks.Count(t => t.Priority == TaskPriority.Low) },
            { "中", tasks.Count(t => t.Priority == TaskPriority.Medium) },
            { "高", highPriorityTasks }
        };

        // 获取最近7天每天完成的任务数
        var dailyCompletedTasks = new List<DailyTaskCountDto>();
        for (int i = 6; i >= 0; i--)
        {
            var date = now.AddDays(-i);
            var dateStart = date.Date;
            var dateEnd = date.Date.AddDays(1);

            var count = tasks.Count(t =>
                t.CompletedAt.HasValue &&
                t.CompletedAt.Value >= dateStart &&
                t.CompletedAt.Value < dateEnd);

            dailyCompletedTasks.Add(new DailyTaskCountDto
            {
                Date = date,
                Count = count
            });
        }

        return new TaskStatisticsDto
        {
            TotalTasks = totalTasks,
            PendingTasks = pendingTasks,
            InProgressTasks = inProgressTasks,
            CompletedTasks = completedTasks,
            OverdueTasks = overdueTasks,
            HighPriorityTasks = highPriorityTasks,
            CompletionRate = Math.Round(completionRate, 2),
            TasksByStatus = tasksByStatus,
            TasksByPriority = tasksByPriority,
            DailyCompletedTasks = dailyCompletedTasks
        };
    }
}
