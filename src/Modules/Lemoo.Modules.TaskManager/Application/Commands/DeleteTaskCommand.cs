using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Application.Common;

namespace Lemoo.Modules.TaskManager.Application.Commands;

/// <summary>
/// 删除任务命令
/// </summary>
public record DeleteTaskCommand(Guid Id) : ICommand<Result>;
