using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Abstractions.Persistence;
using Lemoo.Core.Application.Common;
using Lemoo.Modules.TaskManager.Application.Commands;
using Lemoo.Modules.TaskManager.Application.DTOs;
using Lemoo.Modules.TaskManager.Application.Repositories;
using TaskEntity = Lemoo.Modules.TaskManager.Domain.Entities.Task;
using MediatR;

namespace Lemoo.Modules.TaskManager.Application.Handlers;

/// <summary>
/// 创建任务命令处理器
/// </summary>
public class CreateTaskCommandHandler 
    : ICommandHandler<CreateTaskCommand, Result<TaskDto>>
{
    private readonly ITaskRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public CreateTaskCommandHandler(ITaskRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<TaskDto>> Handle(
        CreateTaskCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var taskEntity = TaskEntity.Create(
                request.Title,
                request.Description,
                request.Priority,
                request.DueDate);
            
            await _repository.AddAsync(taskEntity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            var dto = MapToDto(taskEntity);
            return Result<TaskDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<TaskDto>.Failure(ex.Message);
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
