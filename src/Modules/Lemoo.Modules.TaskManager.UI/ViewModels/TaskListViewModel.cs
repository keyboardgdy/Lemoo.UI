using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Lemoo.Modules.TaskManager.Application.Commands;
using Lemoo.Modules.TaskManager.Application.DTOs;
using Lemoo.Modules.TaskManager.Application.Queries;
using TaskStatus = Lemoo.Modules.TaskManager.Domain.ValueObjects.TaskStatus;
using TaskPriority = Lemoo.Modules.TaskManager.Domain.ValueObjects.TaskPriority;

namespace Lemoo.Modules.TaskManager.UI.ViewModels;

/// <summary>
/// 任务列表视图模型
/// </summary>
public partial class TaskListViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    
    [ObservableProperty]
    private ObservableCollection<TaskDto> _tasks = new();
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private string _searchKeyword = string.Empty;
    
    [ObservableProperty]
    private TaskStatus? _selectedStatus;
    
    [ObservableProperty]
    private TaskPriority? _selectedPriority;
    
    public TaskListViewModel(IMediator mediator)
    {
        _mediator = mediator;
        LoadTasksCommand = new AsyncRelayCommand(LoadTasksAsync);
        SearchCommand = new AsyncRelayCommand(SearchTasksAsync);
        CreateTaskCommand = new RelayCommand(CreateTask);
        CompleteTaskCommand = new AsyncRelayCommand<TaskDto>(CompleteTaskAsync);
        DeleteTaskCommand = new AsyncRelayCommand<TaskDto>(DeleteTaskAsync);
    }
    
    public ICommand LoadTasksCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand CreateTaskCommand { get; }
    public ICommand CompleteTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }
    
    private async Task LoadTasksAsync()
    {
        IsLoading = true;
        try
        {
            var query = new GetAllTasksQuery();
            var result = await _mediator.Send(query);
            
            if (result.IsSuccess && result.Data != null)
            {
                Tasks.Clear();
                foreach (var task in result.Data)
                {
                    Tasks.Add(task);
                }
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private async Task SearchTasksAsync()
    {
        IsLoading = true;
        try
        {
            var query = new SearchTasksQuery(
                Keyword: string.IsNullOrWhiteSpace(SearchKeyword) ? null : SearchKeyword,
                Status: SelectedStatus,
                Priority: SelectedPriority
            );
            
            var result = await _mediator.Send(query);
            
            if (result.IsSuccess && result.Data != null)
            {
                Tasks.Clear();
                foreach (var task in result.Data.Data)
                {
                    Tasks.Add(task);
                }
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private void CreateTask()
    {
        // TODO: 打开编辑对话框（新建模式）
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
        
        // TODO: 确认删除对话框
        
        var command = new DeleteTaskCommand(task.Id);
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            await LoadTasksAsync();
        }
    }
}
