using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Application.Common;
using Lemoo.Modules.TaskManager.Application.DTOs;

namespace Lemoo.Modules.TaskManager.Application.Commands;

/// <summary>
/// 完成任务命令
/// </summary>
public record CompleteTaskCommand(Guid Id) : ICommand<Result<TaskDto>>;
