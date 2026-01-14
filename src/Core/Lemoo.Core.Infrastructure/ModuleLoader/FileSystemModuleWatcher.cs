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
            _logger.LogInformation("开始重新加载模块: {ModuleName}", moduleName);

            // 1. 卸载旧模块（如果需要）
            await UnloadModuleAsync(moduleName, cancellationToken);

            // 2. 重新加载所有模块
            var loadResult = await _moduleLoader.LoadModulesWithResultAsync(cancellationToken);

            if (!loadResult.Success)
            {
                var exception = new Exception($"模块加载失败: {loadResult.ErrorMessage}");
                ModuleReloadFailed?.Invoke(this, new ModuleReloadFailedEventArgs(moduleName, exception));
                return false;
            }

            // 3. 查找重新加载的模块
            var reloadedModule = loadResult.Modules.FirstOrDefault(m => m.Name == moduleName);

            if (reloadedModule == null)
            {
                var exception = new Exception($"模块未找到: {moduleName}");
                ModuleReloadFailed?.Invoke(this, new ModuleReloadFailedEventArgs(moduleName, exception));
                return false;
            }

            // 4. 通知应用模块已重新加载
            ModuleReloaded?.Invoke(this, new ModuleReloadedEventArgs(moduleName, reloadedModule));

            _logger.LogInformation("模块重新加载成功: {ModuleName}", moduleName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "模块重新加载失败: {ModuleName}", moduleName);
            ModuleReloadFailed?.Invoke(this, new ModuleReloadFailedEventArgs(moduleName, ex));
            return false;
        }
        finally
        {
            _reloadSemaphore.Release();
        }
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
        // 等待文件写入完成
        await Task.Delay(500);

        var moduleName = Path.GetFileNameWithoutExtension(e.Name);
        _logger.LogInformation("检测到模块文件变化: {ModuleName}", moduleName);

        if (!string.IsNullOrEmpty(moduleName))
        {
            await ReloadModuleAsync(moduleName);
        }
    }

    private async Task OnModuleDeletedAsync(FileSystemEventArgs e)
    {
        var moduleName = Path.GetFileNameWithoutExtension(e.Name);
        _logger.LogWarning("检测到模块文件被删除: {ModuleName}", moduleName);
    }

    private async Task OnModuleRenamedAsync(RenamedEventArgs e)
    {
        var oldModuleName = Path.GetFileNameWithoutExtension(e.OldName);
        var newModuleName = Path.GetFileNameWithoutExtension(e.Name);
        _logger.LogInformation("检测到模块文件重命名: {OldName} -> {NewName}", oldModuleName, newModuleName);
    }

    private async Task UnloadModuleAsync(string moduleName, CancellationToken cancellationToken)
    {
        _logger.LogDebug("开始卸载模块: {ModuleName}", moduleName);

        // 使用 ModuleLoader 的卸载功能
        if (_moduleLoader is ModuleLoader loader)
        {
            var unloaded = await loader.UnloadModuleAsync(moduleName);
            if (!unloaded)
            {
                _logger.LogWarning("模块卸载失败或超时: {ModuleName}", moduleName);
            }
        }

        _logger.LogDebug("模块卸载完成: {ModuleName}", moduleName);
    }
}
