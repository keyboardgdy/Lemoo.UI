using Lemoo.Modules.TaskManager.UI.ViewModels;

namespace Lemoo.Modules.TaskManager.UI.Views.Pages;

/// <summary>
/// TaskDashboardPage.xaml 的交互逻辑
/// </summary>
public partial class TaskDashboardPage : Page
{
    private readonly TaskDashboardViewModel _viewModel;

    public TaskDashboardPage(TaskDashboardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += async (s, e) => await _viewModel.InitializeAsync();
    }
}
