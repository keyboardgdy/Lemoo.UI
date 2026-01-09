using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Application.Common;
using Lemoo.Modules.TaskManager.Application.DTOs;

namespace Lemoo.Modules.TaskManager.Application.Queries;

/// <summary>
/// 获取单个任务查询
/// </summary>
public record GetTaskQuery(Guid Id) : IQuery<Result<TaskDto>>;
