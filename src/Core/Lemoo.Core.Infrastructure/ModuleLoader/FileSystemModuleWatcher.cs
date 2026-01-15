using System.Reflection;
using Lemoo.Core.Abstractions.Module;
using Lemoo.Core.Common.Module;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lemoo.Core.Infrastructure.ModuleLoader;

/// <summary>
/// 文件系统模块监视器 - 支持模块热重载
/// </summary>
public class FileSystemModuleWatcher : IModuleHotReloadService
{
    private readonly IModuleLoader _moduleLoader;
    private readonly IServiceProvider _serviceProvider;
    private readonly ModuleDiscoveryOptions _options;
    private readonly ILogger<FileSystemModuleWatcher> _logger;
    private readonly Dictionary<string, FileSystemWatcher> _watchers = new();
    private readonly SemaphoreSlim _reloadSemaphore = new(1, 1);
    private readonly Dictionary<string, DateTime> _lastReloadTimes = new();
    private readonly Dictionary<string, int> _consecutiveFailures = new();
    private readonly TimeSpan _reloadCooldown = TimeSpan.FromSeconds(2);
    private readonly int _maxConsecutiveFailures = 3;

    public event EventHandler<ModuleReloadedEventArgs>? ModuleReloaded;
    public event EventHandler<ModuleReloadFailedEventArgs>? ModuleReloadFailed;

    public FileSystemModuleWatcher(
        IModuleLoader moduleLoader,
        IServiceProvider serviceProvider,
        IOptions<ModuleDiscoveryOptions> options,
        ILogger<FileSystemModuleWatcher> logger)
    {
        _moduleLoader = moduleLoader;
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> ReloadModuleAsync(string moduleName, CancellationToken cancellationToken = default)
    {
        await _reloadSemaphore.WaitAsync(cancellationToken);

        try
        {
            // 检查冷却时间
            if (_lastReloadTimes.TryGetValue(moduleName, out var lastReloadTime))
            {
                var timeSinceLastReload = DateTime.UtcNow - lastReloadTime;
                if (timeSinceLastReload < _reloadCooldown)
                {
                    _logger.LogDebug("模块重载冷却中，跳过: {ModuleName} (剩余: {RemainingMs}ms)",
                        moduleName, (_reloadCooldown - timeSinceLastReload).TotalMilliseconds);
                    return false;
                }
            }

            _logger.LogInformation("开始重新加载模块: {ModuleName}", moduleName);

            // 1. 卸载旧模块
            await UnloadModuleAsync(moduleName, cancellationToken);

            // 2. 等待文件释放和 GC
            await Task.Delay(500, cancellationToken);
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // 3. 重新加载所有模块
            var loadResult = await _moduleLoader.LoadModulesWithResultAsync(cancellationToken);

            if (!loadResult.Success)
            {
                var exception = new Exception($"模块加载失败: {loadResult.ErrorMessage}");
                HandleReloadFailure(moduleName, exception);
                return false;
            }

            // 4. 查找重新加载的模块
            var reloadedModule = loadResult.Modules.FirstOrDefault(m => m.Name == moduleName);

            if (reloadedModule == null)
            {
                var exception = new Exception($"模块未找到: {moduleName}");
                HandleReloadFailure(moduleName, exception);
                return false;
            }

            // 5. 重置失败计数
            _consecutiveFailures.Remove(moduleName);
            _lastReloadTimes[moduleName] = DateTime.UtcNow;

            // 6. 通知应用模块已重新加载
            ModuleReloaded?.Invoke(this, new ModuleReloadedEventArgs(moduleName, reloadedModule));

            _logger.LogInformation("模块重新加载成功: {ModuleName} (耗时: {ElapsedMs}ms)",
                moduleName, loadResult.LoadDuration.TotalMilliseconds);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "模块重新加载失败: {ModuleName}", moduleName);
            HandleReloadFailure(moduleName, ex);
            return false;
        }
        finally
        {
            _reloadSemaphore.Release();
        }
    }

    private void HandleReloadFailure(string moduleName, Exception exception)
    {
        _consecutiveFailures.TryGetValue(moduleName, out var failures);
        _consecutiveFailures[moduleName] = failures + 1;

        if (_consecutiveFailures[moduleName] >= _maxConsecutiveFailures)
        {
            _logger.LogError(
                "模块 {ModuleName} 连续重载失败 {FailureCount} 次，将停止自动重载。请手动检查模块状态。",
                moduleName, _consecutiveFailures[moduleName]);
        }

        ModuleReloadFailed?.Invoke(this, new ModuleReloadFailedEventArgs(moduleName, exception));
    }

    public async Task WatchModulesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("开始监视模块文件变化...");

        foreach (var path in _options.Paths)
        {
            if (!Directory.Exists(path))
            {
                _logger.LogWarning("模块目录不存在，跳过监视: {ModulePath}", path);
                continue;
            }

            var watcher = new FileSystemWatcher(path)
            {
                Filter = _options.FilePattern,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size,
                IncludeSubdirectories = _options.RecursiveSearch
            };

            watcher.Changed += async (sender, e) => await OnModuleChangedAsync(e);
            watcher.Created += async (sender, e) => await OnModuleChangedAsync(e);
            watcher.Deleted += async (sender, e) => await OnModuleDeletedAsync(e);
            watcher.Renamed += async (sender, e) => await OnModuleRenamedAsync(e);
            watcher.Error += async (sender, e) => await OnWatcherErrorAsync(e);

            watcher.EnableRaisingEvents = true;
            _watchers[path] = watcher;

            _logger.LogInformation("已设置文件监视器: {Path}", path);
        }

        // 保持运行直到取消
        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("模块监视已取消");
        }
        finally
        {
            await StopWatchingAsync(cancellationToken);
        }
    }

    public async Task StopWatchingAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("停止模块文件监视...");

        foreach (var watcher in _watchers.Values)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }

        _watchers.Clear();

        _logger.LogInformation("模块文件监视已停止");
        await Task.CompletedTask;
    }

    private async Task OnModuleChangedAsync(FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed &&
            e.ChangeType != WatcherChangeTypes.Created)
        {
            return;
        }

        // 等待文件写入完成
        await Task.Delay(500);

        // 提取模块名称（去掉 .Module.dll 后缀）
        var fileName = Path.GetFileNameWithoutExtension(e.Name);
        var moduleName = fileName?.Replace(".Module", "");

        if (string.IsNullOrEmpty(moduleName))
        {
            _logger.LogDebug("无法从文件名提取模块名称: {FileName}", e.Name);
            return;
        }

        // 检查是否超过最大失败次数
        if (_consecutiveFailures.TryGetValue(moduleName, out var failures) &&
            failures >= _maxConsecutiveFailures)
        {
            _logger.LogWarning("模块 {ModuleName} 已达到最大失败次数，跳过自动重载", moduleName);
            return;
        }

        _logger.LogInformation("检测到模块文件变化: {ModuleName} ({ChangeType})",
            moduleName, e.ChangeType);

        await ReloadModuleAsync(moduleName);
    }

    private async Task OnModuleDeletedAsync(FileSystemEventArgs e)
    {
        var fileName = Path.GetFileNameWithoutExtension(e.Name);
        var moduleName = fileName?.Replace(".Module", "");

        if (!string.IsNullOrEmpty(moduleName))
        {
            _logger.LogWarning("检测到模块文件被删除: {ModuleName}", moduleName);

            // 卸载已删除的模块
            try
            {
                await _moduleLoader.UnloadModuleAsync(moduleName);
                _consecutiveFailures.Remove(moduleName);
                _lastReloadTimes.Remove(moduleName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "卸载已删除的模块失败: {ModuleName}", moduleName);
            }
        }
    }

    private async Task OnModuleRenamedAsync(RenamedEventArgs e)
    {
        var oldFileName = Path.GetFileNameWithoutExtension(e.OldName);
        var newFileName = Path.GetFileNameWithoutExtension(e.Name);
        var oldModuleName = oldFileName?.Replace(".Module", "");
        var newModuleName = newFileName?.Replace(".Module", "");

        _logger.LogInformation("检测到模块文件重命名: {OldName} -> {NewName}",
            oldModuleName ?? e.OldName, newModuleName ?? e.Name);

        // 如果是模块重命名，需要卸载旧模块并加载新模块
        if (!string.IsNullOrEmpty(oldModuleName) && !string.IsNullOrEmpty(newModuleName))
        {
            try
            {
                await _moduleLoader.UnloadModuleAsync(oldModuleName);
                await ReloadModuleAsync(newModuleName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理模块重命名失败");
            }
        }
    }

    private async Task OnWatcherErrorAsync(ErrorEventArgs e)
    {
        _logger.LogError(e.GetException(), "文件监视器发生错误");

        // 尝试重启监视器
        try
        {
            await StopWatchingAsync();
            await Task.Delay(1000);
            await WatchModulesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "重启文件监视器失败");
        }
    }

    private async Task UnloadModuleAsync(string moduleName, CancellationToken cancellationToken)
    {
        _logger.LogDebug("开始卸载模块: {ModuleName}", moduleName);

        var unloaded = await _moduleLoader.UnloadModuleAsync(moduleName);

        if (!unloaded)
        {
            _logger.LogWarning("模块卸载失败或超时: {ModuleName}", moduleName);
        }

        _logger.LogDebug("模块卸载完成: {ModuleName}", moduleName);
    }

    /// <summary>
    /// 重置模块的失败计数（允许再次尝试自动重载）
    /// </summary>
    public void ResetFailureCount(string moduleName)
    {
        _consecutiveFailures.Remove(moduleName);
        _logger.LogInformation("已重置模块 {ModuleName} 的失败计数", moduleName);
    }

    /// <summary>
    /// 获取监视状态
    /// </summary>
    public ModuleWatcherStatus GetStatus()
    {
        return new ModuleWatcherStatus
        {
            IsWatching = _watchers.Count > 0,
            WatchedPaths = _watchers.Keys.ToList(),
            LastReloadTimes = new Dictionary<string, DateTime>(_lastReloadTimes),
            ConsecutiveFailures = new Dictionary<string, int>(_consecutiveFailures)
        };
    }
}

/// <summary>
/// 模块监视器状态
/// </summary>
public class ModuleWatcherStatus
{
    public bool IsWatching { get; set; }
    public List<string> WatchedPaths { get; set; } = new();
    public Dictionary<string, DateTime> LastReloadTimes { get; set; } = new();
    public Dictionary<string, int> ConsecutiveFailures { get; set; } = new();
}
