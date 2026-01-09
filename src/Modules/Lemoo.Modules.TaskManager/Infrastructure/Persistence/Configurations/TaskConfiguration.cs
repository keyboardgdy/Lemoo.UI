using TaskEntity = Lemoo.Modules.TaskManager.Domain.Entities.Task;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lemoo.Modules.TaskManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// 任务实体配置
/// </summary>
public class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.ToTable("Tasks");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(t => t.Description)
            .HasMaxLength(1000);
        
        builder.Property(t => t.Priority)
            .IsRequired()
            .HasConversion<int>();
        
        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<int>();
        
        builder.Property(t => t.DueDate)
            .HasColumnType("datetime2");
        
        builder.Property(t => t.CompletedAt)
            .HasColumnType("datetime2");
        
        builder.Property(t => t.CreatedAt)
            .IsRequired();
        
        builder.Property(t => t.UpdatedAt);
    }
}
