using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Lemoo.Modules.TaskManager.Application.Commands;
using Lemoo.Modules.TaskManager.Application.DTOs;
using Lemoo.Modules.TaskManager.Application.Queries;
using Lemoo.Modules.TaskManager.Application.Repositories;
using TaskPriority = Lemoo.Modules.TaskManager.Domain.ValueObjects.TaskPriority;
using System.Windows;

namespace Lemoo.Modules.TaskManager.UI.ViewModels;

/// <summary>
/// 任务优先级选项
/// </summary>
public class PriorityOption
{
    public string DisplayName { get; set; } = string.Empty;
    public TaskPriority Value { get; set; }
}

/// <summary>
/// 任务编辑视图模型
/// </summary>
public partial class TaskEditViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    private readonly ITaskLabelRepository _labelRepository;
    private readonly Guid? _taskId;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private PriorityOption? _selectedPriority;

    [ObservableProperty]
    private DateTime? _dueDate;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasErrors;

    public List<PriorityOption> PriorityOptions { get; } = new()
    {
        new PriorityOption { DisplayName = "低", Value = TaskPriority.Low },
        new PriorityOption { DisplayName = "中", Value = TaskPriority.Medium },
        new PriorityOption { DisplayName = "高", Value = TaskPriority.High }
    };

    public string WindowTitle => _taskId.HasValue ? "编辑任务" : "新建任务";

    public IAsyncRelayCommand SaveCommand { get; }
    public IRelayCommand CancelCommand { get; }
    public IAsyncRelayCommand LoadLabelsCommand { get; }

    public TaskEditViewModel(IMediator mediator, ITaskLabelRepository labelRepository, Guid? taskId = null)
    {
        _mediator = mediator;
        _labelRepository = labelRepository;
        _taskId = taskId;

        SelectedPriority = PriorityOptions[1]; // 默认中优先级

        SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
        CancelCommand = new RelayCommand(Cancel);
        LoadLabelsCommand = new AsyncRelayCommand(LoadLabelsAsync);

        // 如果是编辑模式，加载任务数据
        if (_taskId.HasValue)
        {
            LoadTaskDataAsync();
        }
    }

    partial void OnSelectedPriorityChanged(PriorityOption? value)
    {
        SaveCommand.NotifyCanExecuteChanged();
    }

    partial void OnTitleChanged(string value)
    {
        SaveCommand.NotifyCanExecuteChanged();
    }

    private bool CanSave()
    {
        return !string.IsNullOrWhiteSpace(Title) && SelectedPriority != null && !IsLoading;
    }

    private async Task LoadTaskDataAsync()
    {
        IsLoading = true;
        try
        {
            var query = new GetTaskQuery(_taskId!.Value);
            var result = await _mediator.Send(query);

            if (result.IsSuccess && result.Data != null)
            {
                var task = result.Data;
                Title = task.Title;
                Description = task.Description;
                DueDate = task.DueDate;
                SelectedPriority = PriorityOptions.FirstOrDefault(p => p.Value == (TaskPriority)task.Priority);
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

    private async Task LoadLabelsAsync()
    {
        // TODO: 加载标签列表用于标签选择
        // var labels = await _labelRepository.GetAllAsync();
        // AvailableLabels.Clear();
        // foreach (var label in labels)
        // {
        //     AvailableLabels.Add(new TaskLabelCheckBoxItem { Label = label, IsSelected = false });
        // }
        await Task.CompletedTask;
    }

    private async Task SaveAsync()
    {
        if (!CanSave()) return;

        IsLoading = true;
        HasErrors = false;
        ErrorMessage = string.Empty;

        try
        {
            if (_taskId.HasValue)
            {
                // 更新现有任务
                var command = new UpdateTaskCommand(
                    _taskId.Value,
                    Title,
                    Description,
                    SelectedPriority!.Value,
                    DueDate
                );

                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    HasErrors = true;
                    ErrorMessage = result.ErrorMessage ?? "更新任务失败";
                    return;
                }
            }
            else
            {
                // 创建新任务
                var command = new CreateTaskCommand(
                    Title,
                    Description,
                    SelectedPriority!.Value,
                    DueDate
                );

                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    HasErrors = true;
                    ErrorMessage = result.ErrorMessage ?? "创建任务失败";
                    return;
                }
            }

            // 关闭对话框
            RequestClose?.Invoke(true);
        }
        catch (Exception ex)
        {
            HasErrors = true;
            ErrorMessage = $"保存任务时发生错误: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void Cancel()
    {
        RequestClose?.Invoke(false);
    }

    /// <summary>
    /// 请求关闭对话框事件
    /// </summary>
    public event Action<bool>? RequestClose;
}
