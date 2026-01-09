using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Application.Common;
using Lemoo.Modules.TaskManager.Application.DTOs;
using Lemoo.Modules.TaskManager.Domain.ValueObjects;

namespace Lemoo.Modules.TaskManager.Application.Commands;

/// <summary>
/// 创建任务命令
/// </summary>
public record CreateTaskCommand(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate
) : ICommand<Result<TaskDto>>;
