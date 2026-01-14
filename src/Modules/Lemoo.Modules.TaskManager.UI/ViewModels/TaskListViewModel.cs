using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Lemoo.Modules.TaskManager.Application.Commands;
using Lemoo.Modules.TaskManager.Application.DTOs;
using Lemoo.Modules.TaskManager.Application.Queries;
using Lemoo.Modules.TaskManager.Application.Repositories;
using Lemoo.Modules.TaskManager.UI.Views.Dialogs;
using TaskStatus = Lemoo.Modules.TaskManager.Domain.ValueObjects.TaskStatus;
using TaskPriority = Lemoo.Modules.TaskManager.Domain.ValueObjects.TaskPriority;

namespace Lemoo.Modules.TaskManager.UI.ViewModels;

/// <summary>
/// 任务筛选选项
/// </summary>
public class FilterOption
{
    public string DisplayName { get; set; } = string.Empty;
    public object? Value { get; set; }
}

/// <summary>
/// 任务列表视图模型
/// </summary>
public partial class TaskListViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    private readonly ITaskLabelRepository _labelRepository;

    [ObservableProperty]
    private ObservableCollection<TaskDto> _tasks = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _searchKeyword = string.Empty;

    [ObservableProperty]
    private FilterOption? _selectedStatus;

    [ObservableProperty]
    private FilterOption? _selectedPriority;

    public List<FilterOption> StatusOptions { get; } = new()
    {
        new FilterOption { DisplayName = "全部", Value = null },
        new FilterOption { DisplayName = "待办", Value = TaskStatus.Pending },
        new FilterOption { DisplayName = "进行中", Value = TaskStatus.InProgress },
        new FilterOption { DisplayName = "已完成", Value = TaskStatus.Completed }
    };

    public List<FilterOption> PriorityOptions { get; } = new()
    {
        new FilterOption { DisplayName = "全部", Value = null },
        new FilterOption { DisplayName = "低", Value = TaskPriority.Low },
        new FilterOption { DisplayName = "中", Value = TaskPriority.Medium },
        new FilterOption { DisplayName = "高", Value = TaskPriority.High }
    };

    public bool HasNoTasks => Tasks.Count == 0 && !IsLoading;

    public TaskListViewModel(IMediator mediator, ITaskLabelRepository labelRepository)
    {
        _mediator = mediator;
        _labelRepository = labelRepository;
        SelectedStatus = StatusOptions[0];
        SelectedPriority = PriorityOptions[0];

        LoadTasksCommand = new AsyncRelayCommand(LoadTasksAsync);
        SearchCommand = new AsyncRelayCommand(SearchTasksAsync);
        CreateTaskCommand = new RelayCommand(CreateTask);
        EditTaskCommand = new RelayCommand<TaskDto>(EditTask);
        CompleteTaskCommand = new AsyncRelayCommand<TaskDto>(CompleteTaskAsync);
        DeleteTaskCommand = new AsyncRelayCommand<TaskDto>(DeleteTaskAsync);
    }

    public ICommand LoadTasksCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand CreateTaskCommand { get; }
    public ICommand EditTaskCommand { get; }
    public ICommand CompleteTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }

    private async Task LoadTasksAsync()
    {
        IsLoading = true;
        OnPropertyChanged(nameof(HasNoTasks));
        try
        {
            var query = new GetAllTasksQuery();
            var result = await _mediator.Send(query);

            if (result.IsSuccess && result.Data != null)
            {
                Tasks.Clear();
                foreach (var task in result.Data)
                {
                    var enrichedTask = EnrichTaskDto(task);
                    Tasks.Add(enrichedTask);
                }
            }
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(HasNoTasks));
        }
    }

    private async Task SearchTasksAsync()
    {
        IsLoading = true;
        OnPropertyChanged(nameof(HasNoTasks));
        try
        {
            var status = SelectedStatus?.Value as TaskStatus?;
            var priority = SelectedPriority?.Value as TaskPriority?;

            var query = new SearchTasksQuery(
                Keyword: string.IsNullOrWhiteSpace(SearchKeyword) ? null : SearchKeyword,
                Status: status,
                Priority: priority
            );

            var result = await _mediator.Send(query);

            if (result.IsSuccess && result.Data != null)
            {
                Tasks.Clear();
                foreach (var task in result.Data.Data)
                {
                    var enrichedTask = EnrichTaskDto(task);
                    Tasks.Add(enrichedTask);
                }
            }
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(HasNoTasks));
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

    private void CreateTask()
    {
        var ownerWindow = Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
        if (ownerWindow == null) return;

        var result = TaskEditDialog.ShowCreate(ownerWindow, _mediator, _labelRepository);
        if (result == true)
        {
            // 对话框成功关闭，刷新任务列表
            Task.Run(async () => await LoadTasksAsync());
        }
    }

    private void EditTask(TaskDto? task)
    {
        if (task == null) return;

        var ownerWindow = Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
        if (ownerWindow == null) return;

        var result = TaskEditDialog.ShowEdit(ownerWindow, _mediator, _labelRepository, task.Id);
        if (result == true)
        {
            // 对话框成功关闭，刷新任务列表
            Task.Run(async () => await LoadTasksAsync());
        }
    }

    private async Task CompleteTaskAsync(TaskDto? task)
    {
        if (task == null) return;

        var command = new CompleteTaskCommand(task.Id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            await LoadTasksAsync();
        }
    }

    private async Task DeleteTaskAsync(TaskDto? task)
    {
        if (task == null) return;

        // 显示确认对话框
        var result = MessageBox.Show(
            $"确定要删除任务 \"{task.Title}\" 吗？",
            "确认删除",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            var command = new DeleteTaskCommand(task.Id);
            var deleteResult = await _mediator.Send(command);

            if (deleteResult.IsSuccess)
            {
                await LoadTasksAsync();
            }
        }
    }
}
