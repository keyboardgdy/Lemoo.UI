using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Application.Common;
using Lemoo.Modules.TaskManager.Application.DTOs;

namespace Lemoo.Modules.TaskManager.Application.Commands;

/// <summary>
/// 更新任务标签命令
/// </summary>
public record UpdateTaskLabelCommand(
    Guid Id,
    string Name,
    string? Description,
    string Color
) : ICommand<Result<TaskLabelDto>>;
