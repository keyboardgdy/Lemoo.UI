using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Application.Common;
using Lemoo.Modules.TaskManager.Application.DTOs;

namespace Lemoo.Modules.TaskManager.Application.Commands;

/// <summary>
/// 创建任务标签命令
/// </summary>
public record CreateTaskLabelCommand(
    string Name,
    string? Description,
    string Color
) : ICommand<Result<TaskLabelDto>>;
