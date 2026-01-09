using Lemoo.Core.Abstractions.Persistence;
using Lemoo.Core.Application;
using Lemoo.Core.Application.Common;
using TaskEntity = Lemoo.Modules.TaskManager.Domain.Entities.Task;
using TaskStatus = Lemoo.Modules.TaskManager.Domain.ValueObjects.TaskStatus;
using TaskPriority = Lemoo.Modules.TaskManager.Domain.ValueObjects.TaskPriority;

namespace Lemoo.Modules.TaskManager.Application.Repositories;

/// <summary>
/// 任务仓储接口
/// </summary>
public interface ITaskRepository : IRepository<TaskEntity, Guid>
{
    /// <summary>
    /// 搜索任务
    /// </summary>
    System.Threading.Tasks.Task<IEnumerable<TaskEntity>> SearchAsync(
        string? keyword = null,
        TaskStatus? status = null,
        TaskPriority? priority = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 分页搜索任务
    /// </summary>
    System.Threading.Tasks.Task<PagedResult<TaskEntity>> SearchPagedAsync(
        string? keyword = null,
        TaskStatus? status = null,
        TaskPriority? priority = null,
        int pageIndex = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
}
