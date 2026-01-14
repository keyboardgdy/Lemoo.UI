using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Application.Common;

namespace Lemoo.Modules.TaskManager.Application.Commands;

/// <summary>
/// 从任务移除标签命令
/// </summary>
public record RemoveTaskLabelCommand(
    Guid TaskId,
    Guid LabelId
) : ICommand<Result>;
