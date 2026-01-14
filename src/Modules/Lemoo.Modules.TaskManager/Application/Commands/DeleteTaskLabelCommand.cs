using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Application.Common;

namespace Lemoo.Modules.TaskManager.Application.Commands;

/// <summary>
/// 删除任务标签命令
/// </summary>
public record DeleteTaskLabelCommand(Guid Id) : ICommand<Result>;
