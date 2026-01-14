using TaskEntity = Lemoo.Modules.TaskManager.Domain.Entities.Task;
using Lemoo.Modules.TaskManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lemoo.Modules.TaskManager.Infrastructure.Persistence;

/// <summary>
/// 任务管理模块数据库上下文
/// </summary>
public class TaskManagerDbContext : DbContext
{
    public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskEntity> Tasks { get; set; }
    public DbSet<TaskLabel> TaskLabels { get; set; }
    public DbSet<TaskLabelLink> TaskLabelLinks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 应用所有配置
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskManagerDbContext).Assembly);
    }
}
