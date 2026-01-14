using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Lemoo.Modules.TaskManager.Application.DTOs;
using Lemoo.Modules.TaskManager.Application.Queries;

namespace Lemoo.Modules.TaskManager.UI.ViewModels;

/// <summary>
/// 任务列（用于看板视图）
/// </summary>
public partial class TaskColumn : ObservableObject
{
    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private int _status;

    [ObservableProperty]
    private int _count;

    [ObservableProperty]
    private ObservableCollection<TaskDto> _tasks = new();

    public TaskColumn(string title, int status)
    {
        Title = title;
        Status = status;
    }
}

/// <summary>
/// 任务看板视图模型
/// </summary>
public partial class TaskBoardViewModel : ObservableObject
{
    private readonly IMediator _mediator;

    [ObservableProperty]
    private bool _isLoading;

    public ObservableCollection<TaskColumn> Columns { get; } = new();

    public IAsyncRelayCommand LoadTasksCommand { get; }
    public IRelayCommand<TaskDto> ViewDetailCommand { get; }
    public IAsyncRelayCommand<TaskDto> CompleteTaskCommand { get; }
    public IAsyncRelayCommand<TaskDto> DeleteTaskCommand { get; }

    public TaskBoardViewModel(IMediator mediator)
    {
        _mediator = mediator;

        // 初始化看板列
        Columns.Add(new TaskColumn("待办", 1));
        Columns.Add(new TaskColumn("进行中", 2));
        Columns.Add(new TaskColumn("已完成", 3));

        LoadTasksCommand = new AsyncRelayCommand(LoadTasksAsync);
        ViewDetailCommand = new RelayCommand<TaskDto>(ViewDetail);
        CompleteTaskCommand = new AsyncRelayCommand<TaskDto>(CompleteTaskAsync);
        DeleteTaskCommand = new AsyncRelayCommand<TaskDto>(DeleteTaskAsync);
    }

    private async Task LoadTasksAsync()
    {
        IsLoading = true;
        try
        {
            var query = new GetAllTasksQuery();
            var result = await _mediator.Send(query);

            if (result.IsSuccess && result.Data != null)
            {
                // 清空所有列
                foreach (var column in Columns)
                {
                    column.Tasks.Clear();
                }

                // 将任务分配到对应列
                foreach (var task in result.Data)
                {
                    var enrichedTask = EnrichTaskDto(task);
                    var column = Columns.FirstOrDefault(c => c.Status == task.Status);
                    column?.Tasks.Add(enrichedTask);
                }

                // 更新每列的计数
                foreach (var column in Columns)
                {
                    column.Count = column.Tasks.Count;
                }
            }
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

    private void ViewDetail(TaskDto? task)
    {
        if (task == null) return;
        // TODO: 导航到详情页面
        RequestViewDetail?.Invoke(task.Id);
    }

    private async Task CompleteTaskAsync(TaskDto? task)
    {
        if (task == null) return;

        var command = new Application.Commands.CompleteTaskCommand(task.Id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            await LoadTasksAsync();
        }
    }

    private async Task DeleteTaskAsync(TaskDto? task)
    {
        if (task == null) return;

        var command = new Application.Commands.DeleteTaskCommand(task.Id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            await LoadTasksAsync();
        }
    }

    /// <summary>
    /// 请求查看详情事件
    /// </summary>
    public event Action<Guid>? RequestViewDetail;
}
