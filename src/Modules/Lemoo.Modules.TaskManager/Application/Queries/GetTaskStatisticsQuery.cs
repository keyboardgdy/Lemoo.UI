using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Application.Common;
using Lemoo.Modules.TaskManager.Application.DTOs;

namespace Lemoo.Modules.TaskManager.Application.Queries;

/// <summary>
/// 获取任务统计查询
/// </summary>
public record GetTaskStatisticsQuery() : IQuery<Result<TaskStatisticsDto>>;
