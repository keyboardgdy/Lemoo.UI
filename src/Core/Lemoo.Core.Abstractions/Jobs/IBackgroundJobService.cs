namespace Lemoo.Core.Abstractions.Jobs;

/// <summary>
/// 后台任务服务接口 - 提供后台任务和作业调度功能
/// </summary>
public interface IBackgroundJobService
{
    /// <summary>
    /// 入队任务（立即执行）
    /// </summary>
    /// <typeparam name="TJob">任务类型</typeparam>
    /// <param name="job">任务实例</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务ID</returns>
    Task<string> EnqueueAsync<TJob>(
        TJob job, 
        CancellationToken cancellationToken = default) 
        where TJob : class;
    
    /// <summary>
    /// 调度任务（延迟执行）
    /// </summary>
    /// <typeparam name="TJob">任务类型</typeparam>
    /// <param name="job">任务实例</param>
    /// <param name="scheduleAt">执行时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务ID</returns>
    Task<string> ScheduleAsync<TJob>(
        TJob job, 
        DateTimeOffset scheduleAt, 
        CancellationToken cancellationToken = default) 
        where TJob : class;
    
    /// <summary>
    /// 调度重复任务
    /// </summary>
    /// <typeparam name="TJob">任务类型</typeparam>
    /// <param name="job">任务实例</param>
    /// <param name="cronExpression">Cron表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务ID</returns>
    Task<string> ScheduleRecurringAsync<TJob>(
        TJob job, 
        string cronExpression, 
        CancellationToken cancellationToken = default) 
        where TJob : class;
    
    /// <summary>
    /// 删除任务
    /// </summary>
    /// <param name="jobId">任务ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否删除成功</returns>
    Task<bool> DeleteAsync(
        string jobId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取任务状态
    /// </summary>
    /// <param name="jobId">任务ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务状态</returns>
    Task<JobStatus?> GetStatusAsync(
        string jobId, 
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 任务状态
/// </summary>
public class JobStatus
{
    public string JobId { get; set; } = string.Empty;
    public JobState State { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public int MaxRetries { get; set; }
}

/// <summary>
/// 任务状态枚举
/// </summary>
public enum JobState
{
    Enqueued,
    Scheduled,
    Processing,
    Succeeded,
    Failed,
    Deleted
}

/// <summary>
/// 后台任务基类
/// </summary>
public abstract class BackgroundJob
{
    public Guid JobId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public int MaxRetries { get; set; } = 3;
    public TimeSpan? Timeout { get; set; }
    
    /// <summary>
    /// 执行任务
    /// </summary>
    public abstract Task ExecuteAsync(CancellationToken cancellationToken = default);
}

