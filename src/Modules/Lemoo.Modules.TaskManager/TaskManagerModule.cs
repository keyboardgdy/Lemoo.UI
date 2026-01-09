using Lemoo.Modules.Abstractions;
using Lemoo.Core.Application.Extensions;
using Lemoo.Core.Abstractions.Persistence;
using Lemoo.Core.Infrastructure.Persistence;
using Lemoo.Modules.TaskManager.Application.Repositories;
using Lemoo.Modules.TaskManager.Infrastructure.Persistence;
using Lemoo.Modules.TaskManager.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lemoo.Modules.TaskManager;

/// <summary>
/// 任务管理模块定义
/// </summary>
public class TaskManagerModule : ModuleBase
{
    public override string Name => "TaskManager";
    public override string Version => "1.0.0";
    public override string Description => "任务管理模块 - 用于演示 Lemoo 架构功能";
    
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // 1. 注册数据库上下文
        var connectionString = configuration.GetConnectionString("TaskManager");
        if (!string.IsNullOrEmpty(connectionString))
        {
            services.AddDbContext<TaskManagerDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
        }
        
        // 2. 注册工作单元（使用TaskManagerDbContext）
        services.AddScoped<IUnitOfWork>(provider =>
        {
            var context = provider.GetRequiredService<TaskManagerDbContext>();
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<UnitOfWork>();
            return new UnitOfWork(context, logger);
        });
        
        // 3. 注册仓储
        services.AddScoped<ITaskRepository, TaskRepository>();
        
        // 4. 注册 MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(TaskManagerModule).Assembly);
        });
        
        // 5. 注册 FluentValidation
        services.AddValidatorsFromAssembly(typeof(TaskManagerModule).Assembly);
        
        // 6. 注册 CQRS 管道行为
        services.AddCqrsPipelineBehaviors();
    }
    
    public override void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TaskManager");
        if (!string.IsNullOrEmpty(connectionString))
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
    
    public override void ConfigureDbContext(ModelBuilder modelBuilder)
    {
        base.ConfigureDbContext(modelBuilder);
        
        // 应用实体配置
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskManagerModule).Assembly);
    }
}
