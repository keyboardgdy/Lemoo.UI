using Lemoo.Modules.TaskManager.UI.ViewModels;

namespace Lemoo.Modules.TaskManager.UI.Views.Pages;

/// <summary>
/// TaskBoardPage.xaml 的交互逻辑
/// </summary>
public partial class TaskBoardPage : Page
{
    private readonly TaskBoardViewModel _viewModel;

    public TaskBoardPage(TaskBoardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += async (s, e) => await _viewModel.LoadTasksCommand.ExecuteAsync(null);

        // 订阅查看详情事件
        _viewModel.RequestViewDetail += (taskId) =>
        {
            // TODO: 导航到详情页面
        };
    }
}
