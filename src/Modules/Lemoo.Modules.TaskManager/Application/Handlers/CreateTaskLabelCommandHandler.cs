using Lemoo.Core.Application.Common;
using Lemoo.Modules.TaskManager.Application.Commands;
using Lemoo.Modules.TaskManager.Application.DTOs;
using Lemoo.Modules.TaskManager.Application.Repositories;
using Lemoo.Modules.TaskManager.Domain.Entities;
using MediatR;

namespace Lemoo.Modules.TaskManager.Application.Handlers;

/// <summary>
/// 创建任务标签命令处理器
/// </summary>
public class CreateTaskLabelCommandHandler : ICommandHandler<CreateTaskLabelCommand, Result<TaskLabelDto>>
{
    private readonly ITaskLabelRepository _labelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskLabelCommandHandler(
        ITaskLabelRepository labelRepository,
        IUnitOfWork unitOfWork)
    {
        _labelRepository = labelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TaskLabelDto>> Handle(CreateTaskLabelCommand request, CancellationToken cancellationToken)
    {
        // 检查标签名是否已存在
        if (await _labelRepository.ExistsAsync(request.Name, cancellationToken))
        {
            return Result<TaskLabelDto>.Failure($"标签名称 '{request.Name}' 已存在");
        }

        // 创建标签
        var label = TaskLabel.Create(request.Name, request.Description, request.Color);
        await _labelRepository.AddAsync(label, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 返回 DTO
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
