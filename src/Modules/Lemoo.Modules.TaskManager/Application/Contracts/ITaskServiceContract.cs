using Lemoo.Core.Abstractions.Module;
using Lemoo.Modules.TaskManager.Application.DTOs;

namespace Lemoo.Modules.TaskManager.Application.Contracts;

/// <summary>
/// 任务管理服务契约接口 - 定义模块对外提供的服务
/// </summary>
[ModuleContract("TaskService", "任务管理服务契约")]
public interface ITaskServiceContract : IModuleContract
{
    string ContractName => "TaskService";
    string Version => "1.0.0";
    string Description => "提供任务管理相关的服务";
    string ProviderModule => "TaskManager";

    /// <summary>
    /// 获取所有任务
    /// </summary>
    Task<IReadOnlyList<TaskDto>> GetAllTasksAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据 ID 获取任务
    /// </summary>
    Task<TaskDto?> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建任务
    /// </summary>
    Task<TaskDto?> CreateTaskAsync(string title, string? description, TaskPriority priority, DateTime? dueDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新任务
    /// </summary>
    Task<bool> UpdateTaskAsync(Guid id, string title, string? description, TaskPriority priority, DateTime? dueDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除任务
    /// </summary>
    Task<bool> DeleteTaskAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 完成任务
    /// </summary>
    Task<bool> CompleteTaskAsync(Guid id, CancellationToken cancellationToken = default);
}
