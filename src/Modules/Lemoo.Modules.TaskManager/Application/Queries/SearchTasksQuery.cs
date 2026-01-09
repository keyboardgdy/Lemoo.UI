using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Application;
using Lemoo.Core.Application.Common;
using Lemoo.Modules.TaskManager.Application.DTOs;
using TaskStatus = Lemoo.Modules.TaskManager.Domain.ValueObjects.TaskStatus;
using TaskPriority = Lemoo.Modules.TaskManager.Domain.ValueObjects.TaskPriority;

namespace Lemoo.Modules.TaskManager.Application.Queries;

/// <summary>
/// 搜索任务查询（支持筛选和分页）
/// </summary>
public record SearchTasksQuery(
    string? Keyword = null,
    TaskStatus? Status = null,
    TaskPriority? Priority = null,
    int PageIndex = 1,
    int PageSize = 20
) : IQuery<Result<PagedResult<TaskDto>>>;
