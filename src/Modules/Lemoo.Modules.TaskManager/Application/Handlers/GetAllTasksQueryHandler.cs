using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Application.Common;
using Lemoo.Modules.TaskManager.Application.DTOs;
using Lemoo.Modules.TaskManager.Application.Queries;
using Lemoo.Modules.TaskManager.Application.Repositories;
using TaskEntity = Lemoo.Modules.TaskManager.Domain.Entities.Task;
using MediatR;

namespace Lemoo.Modules.TaskManager.Application.Handlers;

/// <summary>
/// 获取所有任务查询处理器
/// </summary>
public class GetAllTasksQueryHandler 
    : IQueryHandler<GetAllTasksQuery, Result<IEnumerable<TaskDto>>>
{
    private readonly ITaskRepository _repository;
    
    public GetAllTasksQueryHandler(ITaskRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Result<IEnumerable<TaskDto>>> Handle(
        GetAllTasksQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var tasks = await _repository.GetAllAsync(cancellationToken);
            var dtos = tasks.Select(MapToDto).ToList();
            
            return Result<IEnumerable<TaskDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<TaskDto>>.Failure(ex.Message);
        }
    }
    
    private TaskDto MapToDto(TaskEntity task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Priority = (int)task.Priority,
            PriorityName = GetPriorityName(task.Priority),
            Status = (int)task.Status,
            StatusName = GetStatusName(task.Status),
            DueDate = task.DueDate,
            CompletedAt = task.CompletedAt,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            IsOverdue = task.IsOverdue()
        };
    }
    
    private string GetPriorityName(Domain.ValueObjects.TaskPriority priority)
    {
        return priority switch
        {
            Domain.ValueObjects.TaskPriority.Low => "低",
            Domain.ValueObjects.TaskPriority.Medium => "中",
            Domain.ValueObjects.TaskPriority.High => "高",
            _ => "未知"
        };
    }
    
    private string GetStatusName(Domain.ValueObjects.TaskStatus status)
    {
        return status switch
        {
            Domain.ValueObjects.TaskStatus.Pending => "待办",
            Domain.ValueObjects.TaskStatus.InProgress => "进行中",
            Domain.ValueObjects.TaskStatus.Completed => "已完成",
            _ => "未知"
        };
    }
}
