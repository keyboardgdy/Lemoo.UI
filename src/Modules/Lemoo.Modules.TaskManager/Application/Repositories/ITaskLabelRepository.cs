using Lemoo.Modules.TaskManager.Application.DTOs;
using Lemoo.Modules.TaskManager.Domain.Entities;

namespace Lemoo.Modules.TaskManager.Application.Repositories;

/// <summary>
/// 任务标签仓储接口
/// </summary>
public interface ITaskLabelRepository
{
    /// <summary>
    /// 获取所有标签
    /// </summary>
    Task<List<TaskLabel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID获取标签
    /// </summary>
    Task<TaskLabel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据名称获取标签
    /// </summary>
    Task<TaskLabel?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加标签
    /// </summary>
    Task AddAsync(TaskLabel label, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新标签
    /// </summary>
    void Update(TaskLabel label);

    /// <summary>
    /// 删除标签
    /// </summary>
    void Delete(TaskLabel label);

    /// <summary>
    /// 检查标签名称是否存在
    /// </summary>
    Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取任务的标签
    /// </summary>
    Task<List<TaskLabel>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
}
