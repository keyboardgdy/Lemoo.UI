using Lemoo.UI.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Lemoo.Modules.TaskManager.UI;

/// <summary>
/// 任务管理UI模块定义
/// </summary>
public class TaskManagerUI : IModuleUI
{
    public string ModuleName => "TaskManager";

    public void RegisterUI(IServiceCollection services)
    {
        // 注册视图模型
        services.AddTransient<ViewModels.TaskListViewModel>();
        services.AddTransient<ViewModels.TaskDashboardViewModel>();
        services.AddTransient<ViewModels.TaskDetailViewModel>();
        services.AddTransient<ViewModels.TaskBoardViewModel>();
    }

    public IReadOnlyList<NavigationItemMetadata> GetNavigationItems()
    {
        return new List<NavigationItemMetadata>
        {
            new NavigationItemMetadata
            {
                PageKey = "task-dashboard",
                Title = "任务仪表板",
                Icon = "\uE74C",  // 仪表板图标
                Module = ModuleName,
                Description = "查看任务统计和趋势",
                IsEnabled = true,
                Order = 1
            },
            new NavigationItemMetadata
            {
                PageKey = "task-board",
                Title = "任务看板",
                Icon = "\uE80A",  // 看板图标
                Module = ModuleName,
                Description = "以看板方式查看任务",
                IsEnabled = true,
                Order = 2
            },
            new NavigationItemMetadata
            {
                PageKey = "task-list",
                Title = "任务列表",
                Icon = "\uE8A5",  // 任务图标
                Module = ModuleName,
                Description = "查看和管理任务",
                IsEnabled = true,
                Order = 3
            }
        };
    }
}
