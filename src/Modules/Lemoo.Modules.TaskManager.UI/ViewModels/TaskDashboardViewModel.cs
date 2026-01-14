using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Lemoo.Modules.TaskManager.Application.DTOs;
using Lemoo.Modules.TaskManager.Application.Queries;

namespace Lemoo.Modules.TaskManager.UI.ViewModels;

/// <summary>
/// 任务仪表板视图模型
/// </summary>
public partial class TaskDashboardViewModel : ObservableObject
{
    private readonly IMediator _mediator;

    [ObservableProperty]
    private TaskStatisticsDto? _statistics;

    [ObservableProperty]
    private bool _isLoading;

    public TaskDashboardViewModel(IMediator mediator)
    {
        _mediator = mediator;
        LoadStatisticsCommand = new AsyncRelayCommand(LoadStatisticsAsync);
    }

    public IAsyncRelayCommand LoadStatisticsCommand { get; }

    private async Task LoadStatisticsAsync()
    {
        IsLoading = true;
        try
        {
            var query = new GetTaskStatisticsQuery();
            var result = await _mediator.Send(query);

            if (result.IsSuccess && result.Data != null)
            {
                Statistics = result.Data;
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// 在页面加载时自动加载统计数据
    /// </summary>
    public async Task InitializeAsync()
    {
        await LoadStatisticsAsync();
    }
}
