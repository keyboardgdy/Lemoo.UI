using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Abstractions.Persistence;
using Lemoo.Core.Application.Common;
using Lemoo.Modules.TaskManager.Application.Commands;
using Lemoo.Modules.TaskManager.Application.Repositories;
using MediatR;

namespace Lemoo.Modules.TaskManager.Application.Handlers;

/// <summary>
/// 删除任务命令处理器
/// </summary>
public class DeleteTaskCommandHandler 
    : ICommandHandler<DeleteTaskCommand, Result>
{
    private readonly ITaskRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public DeleteTaskCommandHandler(ITaskRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result> Handle(
        DeleteTaskCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var task = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (task == null)
            {
                return Result.Failure("任务不存在");
            }
            
            await _repository.DeleteAsync(task, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
