using Lemoo.Core.Abstractions.UI;
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
    }
    
    public IReadOnlyList<NavigationItemMetadata> GetNavigationItems()
    {
        return new List<NavigationItemMetadata>
        {
            new NavigationItemMetadata
            {
                PageKey = "task-list",
                Title = "任务管理",
                Icon = "\uE8A5",  // 任务图标
                Module = ModuleName,
                Description = "查看和管理任务",
                IsEnabled = true,
                Order = 1
            }
        };
    }
}
