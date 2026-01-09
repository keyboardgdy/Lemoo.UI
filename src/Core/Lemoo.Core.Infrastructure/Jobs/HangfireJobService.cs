using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Lemoo.Core.Abstractions.Jobs;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Lemoo.Core.Infrastructure.Jobs;

/// <summary>
/// Hangfire后台任务服务实现
/// </summary>
public class HangfireJobService : IBackgroundJobService
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly ILogger<HangfireJobService> _logger;

    public HangfireJobService(
        IBackgroundJobClient backgroundJobClient,
        IRecurringJobManager recurringJobManager,
        ILogger<HangfireJobService> logger)
    {
        _backgroundJobClient = backgroundJobClient;
        _recurringJobManager = recurringJobManager;
        _logger = logger;
    }

    public Task<string> EnqueueAsync<TJob>(
        TJob job, 
        CancellationToken cancellationToken = default) 
        where TJob : class
    {
        try
        {
            var jobJson = JsonSerializer.Serialize(job);
            var jobId = _backgroundJobClient.Enqueue(() => ExecuteJob<TJob>(jobJson));
            
            _logger.LogInformation("任务已入队: {JobType}, JobId: {JobId}", typeof(TJob).Name, jobId);
            return Task.FromResult(jobId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "入队任务失败: {JobType}", typeof(TJob).Name);
            throw;
        }
    }

    public Task<string> ScheduleAsync<TJob>(
        TJob job, 
        DateTimeOffset scheduleAt, 
        CancellationToken cancellationToken = default) 
        where TJob : class
    {
        try
        {
            var jobJson = JsonSerializer.Serialize(job);
            var delay = scheduleAt - DateTimeOffset.UtcNow;
            
            if (delay <= TimeSpan.Zero)
            {
                // 如果时间已过，立即执行
                return EnqueueAsync(job, cancellationToken);
            }
            
            var jobId = _backgroundJobClient.Schedule(() => ExecuteJob<TJob>(jobJson), delay);
            
            _logger.LogInformation("任务已调度: {JobType}, JobId: {JobId}, 执行时间: {ScheduleAt}", 
                typeof(TJob).Name, jobId, scheduleAt);
            return Task.FromResult(jobId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调度任务失败: {JobType}", typeof(TJob).Name);
            throw;
        }
    }

    public Task<string> ScheduleRecurringAsync<TJob>(
        TJob job, 
        string cronExpression, 
        CancellationToken cancellationToken = default) 
        where TJob : class
    {
        try
        {
            var jobId = Guid.NewGuid().ToString();
            var jobJson = JsonSerializer.Serialize(job);
            
            _recurringJobManager.AddOrUpdate(
                jobId,
                () => ExecuteJob<TJob>(jobJson),
                cronExpression);
            
            _logger.LogInformation("重复任务已调度: {JobType}, JobId: {JobId}, Cron: {Cron}", 
                typeof(TJob).Name, jobId, cronExpression);
            return Task.FromResult(jobId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调度重复任务失败: {JobType}", typeof(TJob).Name);
            throw;
        }
    }

    public Task<bool> DeleteAsync(
        string jobId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Hangfire的删除操作需要通过JobStorage
            // 这里简化处理，实际应该使用Hangfire.Storage.IMonitoringApi
            _recurringJobManager.RemoveIfExists(jobId);
            
            _logger.LogInformation("删除任务: {JobId}", jobId);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除任务失败: {JobId}", jobId);
            return Task.FromResult(false);
        }
    }

    public Task<JobStatus?> GetStatusAsync(
        string jobId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Hangfire的状态查询需要通过IMonitoringApi
            // 这里简化处理，返回基本状态
            var status = new JobStatus
            {
                JobId = jobId,
                State = JobState.Enqueued,
                CreatedAt = DateTime.UtcNow
            };

            return Task.FromResult<JobStatus?>(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取任务状态失败: {JobId}", jobId);
            return Task.FromResult<JobStatus?>(null);
        }
    }

    // Hangfire需要公共静态方法
    public static void ExecuteJob<TJob>(string jobJson) where TJob : class
    {
        var job = JsonSerializer.Deserialize<TJob>(jobJson);
        if (job is Abstractions.Jobs.BackgroundJob backgroundJob)
        {
            backgroundJob.ExecuteAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
        else
        {
            throw new InvalidOperationException($"任务类型 {typeof(TJob).Name} 必须继承自 BackgroundJob");
        }
    }

    private static JobState MapHangfireState(string state)
    {
        return state switch
        {
            "Enqueued" => JobState.Enqueued,
            "Scheduled" => JobState.Scheduled,
            "Processing" => JobState.Processing,
            "Succeeded" => JobState.Succeeded,
            "Failed" => JobState.Failed,
            "Deleted" => JobState.Deleted,
            _ => JobState.Enqueued
        };
    }
}

