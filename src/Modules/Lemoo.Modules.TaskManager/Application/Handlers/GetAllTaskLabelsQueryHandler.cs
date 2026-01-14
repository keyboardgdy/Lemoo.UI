using Lemoo.Core.Application.Common;
using Lemoo.Modules.TaskManager.Application.DTOs;
using Lemoo.Modules.TaskManager.Application.Queries;
using Lemoo.Modules.TaskManager.Application.Repositories;
using MediatR;

namespace Lemoo.Modules.TaskManager.Application.Handlers;

/// <summary>
/// 获取所有任务标签查询处理器
/// </summary>
public class GetAllTaskLabelsQueryHandler : IQueryHandler<GetAllTaskLabelsQuery, Result<List<TaskLabelDto>>>
{
    private readonly ITaskLabelRepository _labelRepository;

    public GetAllTaskLabelsQueryHandler(ITaskLabelRepository labelRepository)
    {
        _labelRepository = labelRepository;
    }

    public async Task<Result<List<TaskLabelDto>>> Handle(GetAllTaskLabelsQuery request, CancellationToken cancellationToken)
    {
        var labels = await _labelRepository.GetAllAsync(cancellationToken);

        var dtos = labels.Select(l => new TaskLabelDto
        {
            Id = l.Id,
            Name = l.Name,
            Description = l.Description,
            Color = l.Color,
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt
        }).ToList();

        return Result<List<TaskLabelDto>>.Success(dtos);
    }
}
