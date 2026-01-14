using Lemoo.Core.Application.Common;
using Lemoo.Modules.TaskManager.Application.Queries;
using Lemoo.Modules.TaskManager.Application.Repositories;
using MediatR;

namespace Lemoo.Modules.TaskManager.Application.Handlers;

/// <summary>
/// 获取任务统计查询处理器
/// </summary>
public class GetTaskStatisticsQueryHandler : IQueryHandler<GetTaskStatisticsQuery, Result<TaskStatisticsDto>>
{
    private readonly ITaskRepository _taskRepository;

    public GetTaskStatisticsQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Result<TaskStatisticsDto>> Handle(GetTaskStatisticsQuery request, CancellationToken cancellationToken)
    {
        var statistics = await _taskRepository.GetStatisticsAsync(cancellationToken);
        return Result<TaskStatisticsDto>.Success(statistics);
    }
}
