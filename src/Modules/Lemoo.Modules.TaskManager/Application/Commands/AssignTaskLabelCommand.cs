using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Application.Common;

namespace Lemoo.Modules.TaskManager.Application.Commands;

/// <summary>
/// 为任务分配标签命令
/// </summary>
public record AssignTaskLabelCommand(
    Guid TaskId,
    Guid LabelId
) : ICommand<Result>;

/// <summary>
/// 批量为任务分配标签命令
/// </summary>
public record AssignTaskLabelsCommand(
    Guid TaskId,
    List<Guid> LabelIds
) : ICommand<Result>;
