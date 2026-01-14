using Lemoo.Core.Abstractions.Module;
using Lemoo.Modules.TaskManager.Application.Commands;
using Lemoo.Modules.TaskManager.Application.DTOs;
using Lemoo.Modules.TaskManager.Application.Queries;
using MediatR;

namespace Lemoo.Modules.TaskManager.Application.Contracts;

/// <summary>
/// 任务服务契约适配器 - 将契约接口调用转换为 MediatR 命令/查询
/// </summary>
public class TaskServiceContractAdapter : ITaskServiceContract
{
    private readonly ISender _mediator;

    public TaskServiceContractAdapter(ISender mediator)
    {
        _mediator = mediator;
    }

    public async Task<IReadOnlyList<TaskDto>> GetAllTasksAsync(CancellationToken cancellationToken = default)
    {
        var query = new GetAllTasksQuery();
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsSuccess && result.Value != null)
        {
            return result.Value;
        }

        return Array.Empty<TaskDto>();
    }

    public async Task<TaskDto?> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetTaskByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {
            return result.Value;
        }

        return null;
    }

    public async Task<TaskDto?> CreateTaskAsync(string title, string? description, TaskPriority priority, DateTime? dueDate, CancellationToken cancellationToken = default)
    {
        var command = new CreateTaskCommand(title, description, priority, dueDate);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return result.Value;
        }

        return null;
    }

    public async Task<bool> UpdateTaskAsync(Guid id, string title, string? description, TaskPriority priority, DateTime? dueDate, CancellationToken cancellationToken = default)
    {
        var command = new UpdateTaskCommand(id, title, description, priority, dueDate);
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess;
    }

    public async Task<bool> DeleteTaskAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteTaskCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess;
    }

    public async Task<bool> CompleteTaskAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new CompleteTaskCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess;
    }
}
