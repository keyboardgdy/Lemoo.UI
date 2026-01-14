using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Application.Common;
using Lemoo.Modules.TaskManager.Application.DTOs;

namespace Lemoo.Modules.TaskManager.Application.Queries;

/// <summary>
/// 获取所有任务标签查询
/// </summary>
public record GetAllTaskLabelsQuery() : IQuery<Result<List<TaskLabelDto>>>;
