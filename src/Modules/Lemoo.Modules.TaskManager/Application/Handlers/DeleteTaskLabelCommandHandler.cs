using Lemoo.Core.Application.Common;
using Lemoo.Modules.TaskManager.Application.Commands;
using Lemoo.Modules.TaskManager.Application.Repositories;
using MediatR;

namespace Lemoo.Modules.TaskManager.Application.Handlers;

/// <summary>
/// 删除任务标签命令处理器
/// </summary>
public class DeleteTaskLabelCommandHandler : ICommandHandler<DeleteTaskLabelCommand, Result>
{
    private readonly ITaskLabelRepository _labelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaskLabelCommandHandler(
        ITaskLabelRepository labelRepository,
        IUnitOfWork unitOfWork)
    {
        _labelRepository = labelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteTaskLabelCommand request, CancellationToken cancellationToken)
    {
        var label = await _labelRepository.GetByIdAsync(request.Id, cancellationToken);
        if (label == null)
        {
            return Result.Failure($"标签 ID '{request.Id}' 不存在");
        }

        _labelRepository.Delete(label);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
