using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Application.Common;
using Lemoo.Modules.TaskManager.Application.DTOs;

namespace Lemoo.Modules.TaskManager.Application.Queries;

/// <summary>
/// 获取所有任务查询
/// </summary>
public record GetAllTasksQuery() : IQuery<Result<IEnumerable<TaskDto>>>;
