using Microsoft.Extensions.Logging;

namespace Lemoo.Core.Abstractions.Module;

/// <summary>
/// 模块生命周期管理器接口
/// </summary>
public interface IModuleLifecycleManager
{
    /// <summary>
    /// 获取模块状态
    /// </summary>
    Task<ModuleState> GetModuleStateAsync(string moduleName);

    /// <summary>
    /// 启动模块
    /// </summary>
    Task<ModuleOperationResult> StartModuleAsync(string moduleName, IServiceProvider serviceProvider);

    /// <summary>
    /// 停止模块
    /// </summary>
    Task<ModuleOperationResult> StopModuleAsync(string moduleName, IServiceProvider serviceProvider);

    /// <summary>
    /// 重启模块
    /// </summary>
    Task<ModuleOperationResult> RestartModuleAsync(string moduleName, IServiceProvider serviceProvider);

    /// <summary>
    /// 获取所有模块状态
    /// </summary>
    Task<Dictionary<string, ModuleState>> GetAllModuleStatesAsync();

    /// <summary>
    /// 模块状态变化事件
    /// </summary>
    event EventHandler<ModuleStateChangedEventArgs>? ModuleStateChanged;
}

/// <summary>
/// 模块生命周期管理器实现
/// </summary>
public class ModuleLifecycleManager : IModuleLifecycleManager
{
    private readonly IModuleLoader _moduleLoader;
    private readonly ILogger<ModuleLifecycleManager> _logger;
    private readonly Dictionary<string, ModuleState> _moduleStates = new();

    public event EventHandler<ModuleStateChangedEventArgs>? ModuleStateChanged;

    public ModuleLifecycleManager(IModuleLoader moduleLoader, ILogger<ModuleLifecycleManager> logger)
    {
        _moduleLoader = moduleLoader;
        _logger = logger;

        // 初始化所有模块状态为 Loaded
        foreach (var module in _moduleLoader.GetLoadedModules())
        {
            _moduleStates[module.Name] = ModuleState.Loaded;
        }
    }

    public Task<ModuleState> GetModuleStateAsync(string moduleName)
    {
        if (_moduleStates.TryGetValue(moduleName, out var state))
        {
            return Task.FromResult(state);
        }

        // 如果不在状态字典中，检查模块是否存在
        var module = _moduleLoader.GetModule(moduleName);
        if (module != null)
        {
            _moduleStates[moduleName] = ModuleState.Loaded;
            return Task.FromResult(ModuleState.Loaded);
        }

        return Task.FromResult(ModuleState.Unloaded);
    }

    public async Task<ModuleOperationResult> StartModuleAsync(string moduleName, IServiceProvider serviceProvider)
    {
        var module = _moduleLoader.GetModule(moduleName);
        if (module == null)
        {
            return ModuleOperationResult.Failed($"模块 '{moduleName}' 未找到");
        }

        var currentState = await GetModuleStateAsync(moduleName);
        if (currentState == ModuleState.Started)
        {
            return ModuleOperationResult.Succeeded($"模块 '{moduleName}' 已在运行中");
        }

        try
        {
            await SetModuleStateAsync(moduleName, ModuleState.Starting);

            // 调用模块的启动前钩子
            await module.OnApplicationStartingAsync(serviceProvider, default);

            // 调用模块的启动后钩子
            await module.OnApplicationStartedAsync(serviceProvider, default);

            await SetModuleStateAsync(moduleName, ModuleState.Started);

            _logger.LogInformation("模块启动成功: {ModuleName}", moduleName);
            return ModuleOperationResult.Succeeded($"模块 '{moduleName}' 启动成功");
        }
        catch (Exception ex)
        {
            await SetModuleStateAsync(moduleName, ModuleState.Error);
            _logger.LogError(ex, "模块启动失败: {ModuleName}", moduleName);
            return ModuleOperationResult.Failed($"模块 '{moduleName}' 启动失败: {ex.Message}");
        }
    }

    public async Task<ModuleOperationResult> StopModuleAsync(string moduleName, IServiceProvider serviceProvider)
    {
        var module = _moduleLoader.GetModule(moduleName);
        if (module == null)
        {
            return ModuleOperationResult.Failed($"模块 '{moduleName}' 未找到");
        }

        var currentState = await GetModuleStateAsync(moduleName);
        if (currentState == ModuleState.Stopped || currentState == ModuleState.Loaded)
        {
            return ModuleOperationResult.Succeeded($"模块 '{moduleName}' 已停止");
        }

        try
        {
            await SetModuleStateAsync(moduleName, ModuleState.Stopping);

            // 调用模块的停止前钩子
            await module.OnApplicationStoppingAsync(serviceProvider, default);

            // 调用模块的停止后钩子
            await module.OnApplicationStoppedAsync(serviceProvider, default);

            await SetModuleStateAsync(moduleName, ModuleState.Stopped);

            _logger.LogInformation("模块停止成功: {ModuleName}", moduleName);
            return ModuleOperationResult.Succeeded($"模块 '{moduleName}' 停止成功");
        }
        catch (Exception ex)
        {
            await SetModuleStateAsync(moduleName, ModuleState.Error);
            _logger.LogError(ex, "模块停止失败: {ModuleName}", moduleName);
            return ModuleOperationResult.Failed($"模块 '{moduleName}' 停止失败: {ex.Message}");
        }
    }

    public async Task<ModuleOperationResult> RestartModuleAsync(string moduleName, IServiceProvider serviceProvider)
    {
        // 先停止
        var stopResult = await StopModuleAsync(moduleName, serviceProvider);
        if (!stopResult.Success)
        {
            return stopResult;
        }

        // 再启动
        return await StartModuleAsync(moduleName, serviceProvider);
    }

    public Task<Dictionary<string, ModuleState>> GetAllModuleStatesAsync()
    {
        return Task.FromResult(new Dictionary<string, ModuleState>(_moduleStates));
    }

    private async Task SetModuleStateAsync(string moduleName, ModuleState newState)
    {
        var oldState = _moduleStates.GetValueOrDefault(moduleName, ModuleState.Unloaded);
        _moduleStates[moduleName] = newState;

        // 触发状态变化事件
        ModuleStateChanged?.Invoke(this, new ModuleStateChangedEventArgs(moduleName, oldState, newState));

        await Task.CompletedTask;
    }
}

/// <summary>
/// 模块操作结果
/// </summary>
public class ModuleOperationResult
{
    public bool Success { get; private set; }
    public string Message { get; private set; } = string.Empty;

    public static ModuleOperationResult Succeeded(string message)
    {
        return new ModuleOperationResult { Success = true, Message = message };
    }

    public static ModuleOperationResult Failed(string message)
    {
        return new ModuleOperationResult { Success = false, Message = message };
    }
}

/// <summary>
/// 模块状态变化事件参数
/// </summary>
public class ModuleStateChangedEventArgs : EventArgs
{
    public string ModuleName { get; }
    public ModuleState OldState { get; }
    public ModuleState NewState { get; }
    public DateTime ChangedAt { get; }

    public ModuleStateChangedEventArgs(string moduleName, ModuleState oldState, ModuleState newState)
    {
        ModuleName = moduleName;
        OldState = oldState;
        NewState = newState;
        ChangedAt = DateTime.UtcNow;
    }
}
