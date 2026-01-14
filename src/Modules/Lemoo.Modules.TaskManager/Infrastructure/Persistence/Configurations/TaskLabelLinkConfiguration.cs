using Lemoo.Modules.TaskManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lemoo.Modules.TaskManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// 任务-标签关联实体配置
/// </summary>
public class TaskLabelLinkConfiguration : IEntityTypeConfiguration<TaskLabelLink>
{
    public void Configure(EntityTypeBuilder<TaskLabelLink> builder)
    {
        builder.ToTable("TaskLabelLinks");

        // 配置复合主键
        builder.HasKey(x => new { x.TaskId, x.LabelId });

        // 配置与 Task 的关系
        builder.HasOne(x => x.Task)
            .WithMany(t => t.Labels)
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // 配置与 TaskLabel 的关系
        builder.HasOne(x => x.Label)
            .WithMany()
            .HasForeignKey(x => x.LabelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.AssignedAt)
            .IsRequired();
    }
}
