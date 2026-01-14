using Lemoo.Core.Application.Common;
using Lemoo.Modules.TaskManager.Application.Commands;
using Lemoo.Modules.TaskManager.Application.DTOs;
using Lemoo.Modules.TaskManager.Application.Repositories;
using MediatR;

namespace Lemoo.Modules.TaskManager.Application.Handlers;

/// <summary>
/// 更新任务标签命令处理器
/// </summary>
public class UpdateTaskLabelCommandHandler : ICommandHandler<UpdateTaskLabelCommand, Result<TaskLabelDto>>
{
    private readonly ITaskLabelRepository _labelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaskLabelCommandHandler(
        ITaskLabelRepository labelRepository,
        IUnitOfWork unitOfWork)
    {
        _labelRepository = labelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TaskLabelDto>> Handle(UpdateTaskLabelCommand request, CancellationToken cancellationToken)
    {
        var label = await _labelRepository.GetByIdAsync(request.Id, cancellationToken);
        if (label == null)
        {
            return Result<TaskLabelDto>.Failure($"标签 ID '{request.Id}' 不存在");
        }

        // 检查标签名是否被其他标签使用
        var existingLabel = await _labelRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingLabel != null && existingLabel.Id != request.Id)
        {
            return Result<TaskLabelDto>.Failure($"标签名称 '{request.Name}' 已被其他标签使用");
        }

        // 更新标签
        label.Update(request.Name, request.Description, request.Color);
        _labelRepository.Update(label);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new TaskLabelDto
        {
            Id = label.Id,
            Name = label.Name,
            Description = label.Description,
            Color = label.Color,
            CreatedAt = label.CreatedAt,
            UpdatedAt = label.UpdatedAt
        };

        return Result<TaskLabelDto>.Success(dto);
    }
}
