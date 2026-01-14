using Lemoo.Modules.TaskManager.UI.ViewModels;
using Lemoo.Modules.TaskManager.Application.Repositories;
using MediatR;

namespace Lemoo.Modules.TaskManager.UI.Views.Dialogs;

/// <summary>
/// TaskEditDialog.xaml 的交互逻辑
/// </summary>
public partial class TaskEditDialog : Window
{
    private readonly TaskEditViewModel _viewModel;

    public TaskEditDialog(TaskEditViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        // 订阅关闭请求
        _viewModel.RequestClose += (result) =>
        {
            DialogResult = result;
            Close();
        };

        // 窗口关闭时清理
        Closing += (s, e) =>
        {
            _viewModel.RequestClose -= (r) => { };
        };
    }

    /// <summary>
    /// 显示创建任务对话框
    /// </summary>
    public static bool? ShowCreate(Window owner, IMediator mediator, ITaskLabelRepository labelRepository)
    {
        var viewModel = new TaskEditViewModel(mediator, labelRepository, null);
        var dialog = new TaskEditDialog(viewModel)
        {
            Owner = owner
        };
        return dialog.ShowDialog();
    }

    /// <summary>
    /// 显示编辑任务对话框
    /// </summary>
    public static bool? ShowEdit(Window owner, IMediator mediator, ITaskLabelRepository labelRepository, Guid taskId)
    {
        var viewModel = new TaskEditViewModel(mediator, labelRepository, taskId);
        var dialog = new TaskEditDialog(viewModel)
        {
            Owner = owner
        };
        return dialog.ShowDialog();
    }
}
