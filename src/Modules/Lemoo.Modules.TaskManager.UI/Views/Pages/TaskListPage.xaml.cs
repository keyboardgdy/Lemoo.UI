using System.Windows.Controls;
using Lemoo.Modules.TaskManager.UI.ViewModels;

namespace Lemoo.Modules.TaskManager.UI.Views.Pages;

/// <summary>
/// TaskListPage.xaml 的交互逻辑
/// </summary>
public partial class TaskListPage : Page
{
    public TaskListPage(TaskListViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        
        // 加载数据
        viewModel.LoadTasksCommand.Execute(null);
    }
}
