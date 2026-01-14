using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Lemoo.Modules.TaskManager.Application.Commands;
using Lemoo.Modules.TaskManager.Application.DTOs;
using Lemoo.Modules.TaskManager.Application.Queries;

namespace Lemoo.Modules.TaskManager.UI.ViewModels;

/// <summary>
/// 任务详情视图模型
/// </summary>
public partial class TaskDetailViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    private readonly Guid _taskId;

    [ObservableProperty]
    private TaskDto? _task;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasErrors;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public IAsyncRelayCommand LoadTaskCommand { get; }
    public IAsyncRelayCommand CompleteTaskCommand { get; }
    public IAsyncRelayCommand DeleteTaskCommand { get; }
    public IRelayCommand EditTaskCommand { get; }

    public TaskDetailViewModel(IMediator mediator, Guid taskId)
    {
        _mediator = mediator;
        _taskId = taskId;

        LoadTaskCommand = new AsyncRelayCommand(LoadTaskAsync);
        CompleteTaskCommand = new AsyncRelayCommand(CompleteTaskAsync, CanCompleteTask);
        DeleteTaskCommand = new AsyncRelayCommand(DeleteTaskAsync);
        EditTaskCommand = new RelayCommand(EditTask);
    }

    partial void OnTaskChanged(TaskDto? value)
    {
        CompleteTaskCommand.NotifyCanExecuteChanged();
    }

    private bool CanCompleteTask()
    {
        return Task != null && Task.Status != 3 && !IsLoading; // 3 = Completed
    }

    private async Task LoadTaskAsync()
    {
        IsLoading = true;
        HasErrors = false;
        try
        {
            var query = new GetTaskQuery(_taskId);
            var result = await _mediator.Send(query);

            if (result.IsSuccess && result.Data != null)
            {
                Task = EnrichTaskDto(result.Data);
            }
            else
            {
                HasErrors = true;
                ErrorMessage = result.ErrorMessage ?? "加载任务失败";
            }
        }
        catch
        {
            HasErrors = true;
            ErrorMessage = "加载任务时发生错误";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private TaskDto EnrichTaskDto(TaskDto task)
    {
        // 添加状态颜色
        task.StatusColor = task.Status switch
        {
            1 => "#6C757D", // Pending - 灰色
            2 => "#0078D4", // InProgress - 蓝色
            3 => "#107C10", // Completed - 绿色
            _ => "#6C757D"
        };

        // 添加优先级颜色
        task.PriorityColor = task.Priority switch
        {
            1 => "#6C757D", // Low - 灰色
            2 => "#FF8C00", // Medium - 橙色
            3 => "#D13438", // High - 红色
            _ => "#6C757D"
        };

        return task;
    }

    private async Task CompleteTaskAsync()
    {
        if (Task == null || Task.Status == 3) return;

        IsLoading = true;
        try
        {
            var command = new CompleteTaskCommand(Task.Id);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                await LoadTaskAsync();
                RequestRefresh?.Invoke();
            }
            else
            {
                HasErrors = true;
                ErrorMessage = result.ErrorMessage ?? "完成任务失败";
            }
        }
        catch
        {
            HasErrors = true;
            ErrorMessage = "完成任务时发生错误";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DeleteTaskAsync()
    {
        if (Task == null) return;

        IsLoading = true;
        try
        {
            var command = new DeleteTaskCommand(Task.Id);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                RequestClose?.Invoke();
            }
            else
            {
                HasErrors = true;
                ErrorMessage = result.ErrorMessage ?? "删除任务失败";
            }
        }
        catch
        {
            HasErrors = true;
            ErrorMessage = "删除任务时发生错误";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void EditTask()
    {
        RequestEdit?.Invoke(Task?.Id ?? Guid.Empty);
    }

    /// <summary>
    /// 请求关闭详情视图事件
    /// </summary>
    public event Action? RequestClose;

    /// <summary>
    /// 请求编辑任务事件
    /// </summary>
    public event Action<Guid>? RequestEdit;

    /// <summary>
    /// 请求刷新事件（用于更新列表）
    /// </summary>
    public event Action? RequestRefresh;
}
