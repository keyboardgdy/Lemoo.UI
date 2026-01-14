using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Lemoo.Modules.TaskManager.UI.ViewModels;

namespace Lemoo.Modules.TaskManager.UI.Views.Pages;

/// <summary>
/// TaskDetailPage.xaml 的交互逻辑
/// </summary>
public partial class TaskDetailPage : Page
{
    private readonly TaskDetailViewModel _viewModel;

    public TaskDetailPage(TaskDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += async (s, e) => await _viewModel.LoadTaskCommand.ExecuteAsync(null);

        // 订阅事件
        _viewModel.RequestClose += () =>
        {
            NavigationService?.GoBack();
        };

        _viewModel.RequestRefresh += () =>
        {
            // TODO: 通知列表刷新
        };

        _viewModel.RequestEdit += (taskId) =>
        {
            // TODO: 打开编辑对话框
        };
    }

    private void OnBackClick(object sender, RoutedEventArgs e)
    {
        NavigationService?.GoBack();
    }
}
