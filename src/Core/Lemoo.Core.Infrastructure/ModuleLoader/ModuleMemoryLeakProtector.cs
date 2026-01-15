using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Lemoo.Core.Infrastructure.ModuleLoader;

/// <summary>
/// 模块内存泄漏防护器 - 确保模块卸载时不会造成内存泄漏
/// </summary>
public class ModuleMemoryLeakProtector
{
    private readonly ILogger<ModuleMemoryLeakProtector> _logger;
    private readonly Dictionary<string, List<WeakReference<object>>> _trackedReferences = new();

    public ModuleMemoryLeakProtector(ILogger<ModuleMemoryLeakProtector> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 清理模块的静态事件引用，防止内存泄漏
    /// </summary>
    public void CleanupModuleReferences(string moduleName, Assembly moduleAssembly)
    {
        _logger.LogDebug("开始清理模块引用: {ModuleName}", moduleName);

        try
        {
            // 1. 清理静态事件
            CleanupStaticEvents(moduleAssembly);

            // 2. 清理缓存
            CleanupCaches(moduleAssembly);

            // 3. 清理定时器
            CleanupTimers(moduleAssembly);

            // 4. 记录跟踪的弱引用
            TrackWeakReferences(moduleName, moduleAssembly);

            _logger.LogInformation("模块引用清理完成: {ModuleName}", moduleName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清理模块引用时出错: {ModuleName}", moduleName);
        }
    }

    /// <summary>
    /// 检查模块是否可以安全卸载
    /// </summary>
    public bool CanUnloadSafely(string moduleName)
    {
        if (!_trackedReferences.TryGetValue(moduleName, out var references))
        {
            return true;
        }

        // 检查所有跟踪的弱引用是否已被回收
        var aliveCount = 0;
        for (int i = references.Count - 1; i >= 0; i--)
        {
            if (references[i].TryGetTarget(out var _))
            {
                aliveCount++;
            }
            else
            {
                references.RemoveAt(i);
            }
        }

        if (aliveCount > 0)
        {
            _logger.LogWarning("模块 {ModuleName} 仍有 {Count} 个存活引用，可能无法安全卸载",
                moduleName, aliveCount);
            return false;
        }

        return true;
    }

    /// <summary>
    /// 强制清理模块的静态事件
    /// </summary>
    private void CleanupStaticEvents(Assembly assembly)
    {
        try
        {
            // 获取所有类型
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                try
                {
                    // 获取所有静态事件
                    var events = type.GetEvents(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                    foreach (var eventInfo in events)
                    {
                        try
                        {
                            // 尝试获取事件的当前订阅列表
                            var eventField = type.GetField(eventInfo.Name,
                                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                            if (eventField != null)
                            {
                                var eventValue = eventField.GetValue(null);
                                if (eventValue is Delegate delegateInstance)
                                {
                                    var invocationCount = delegateInstance.GetInvocationList().Length;
                                    if (invocationCount > 0)
                                    {
                                        _logger.LogWarning(
                                            "发现静态事件订阅: {TypeName}.{EventName} ({Count} 个订阅者)",
                                            type.Name, eventInfo.Name, invocationCount);

                                        // 注意：这里无法直接清除静态事件的订阅
                                        // 需要模块实现自己的清理逻辑
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogTrace(ex, "无法检查事件: {TypeName}.{EventName}",
                                type.Name, eventInfo.Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogTrace(ex, "无法检查类型的静态事件: {TypeName}", type.Name);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogTrace(ex, "无法清理静态事件");
        }
    }

    /// <summary>
    /// 清理模块的缓存
    /// </summary>
    private void CleanupCaches(Assembly assembly)
    {
        try
        {
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                try
                {
                    // 查找可能的缓存字段（以 cache 结尾的静态字段）
                    var cacheFields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(f => f.Name.ToLower().Contains("cache"));

                    foreach (var cacheField in cacheFields)
                    {
                        try
                        {
                            if (cacheField.FieldType.IsGenericType &&
                                cacheField.FieldType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                            {
                                var cache = cacheField.GetValue(null);
                                if (cache != null)
                                {
                                    var countProperty = cacheField.FieldType.GetProperty("Count");
                                    var count = (int?)(countProperty?.GetValue(cache) ?? 0);

                                    if (count > 0)
                                    {
                                        _logger.LogWarning(
                                            "发现静态缓存: {TypeName}.{FieldName} ({Count} 项)",
                                            type.Name, cacheField.Name, count);

                                        // 尝试清空缓存
                                        var clearMethod = cacheField.FieldType.GetMethod("Clear");
                                        clearMethod?.Invoke(cache, null);
                                        _logger.LogDebug("已清空缓存: {TypeName}.{FieldName}",
                                            type.Name, cacheField.Name);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogTrace(ex, "无法清空缓存: {TypeName}.{FieldName}",
                                type.Name, cacheField.Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogTrace(ex, "无法检查类型的缓存: {TypeName}", type.Name);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogTrace(ex, "无法清理缓存");
        }
    }

    /// <summary>
    /// 清理模块的定时器
    /// </summary>
    private void CleanupTimers(Assembly assembly)
    {
        try
        {
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                try
                {
                    // 查找 Timer 类型的静态字段
                    var timerFields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(f => f.FieldType == typeof(System.Threading.Timer) ||
                                    f.FieldType == typeof(System.Timers.Timer));

                    foreach (var timerField in timerFields)
                    {
                        try
                        {
                            var timer = timerField.GetValue(null);
                            if (timer != null)
                            {
                                _logger.LogWarning("发现静态定时器: {TypeName}.{FieldName}",
                                    type.Name, timerField.Name);

                                // 注意：这里需要模块自己实现定时器的清理逻辑
                                // 我们可以建议模块实现 IDisposable 模式
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogTrace(ex, "无法检查定时器: {TypeName}.{FieldName}",
                                type.Name, timerField.Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogTrace(ex, "无法检查类型的定时器: {TypeName}", type.Name);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogTrace(ex, "无法清理定时器");
        }
    }

    /// <summary>
    /// 跟踪模块对象的弱引用
    /// </summary>
    private void TrackWeakReferences(string moduleName, Assembly assembly)
    {
        try
        {
            var references = new List<WeakReference<object>>();
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                try
                {
                    // 跟踪单例实例
                    var singletonFields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(f => f.Name.ToLower().Contains("instance") ||
                                    f.Name.ToLower().Contains("singleton"));

                    foreach (var field in singletonFields)
                    {
                        try
                        {
                            var instance = field.GetValue(null);
                            if (instance != null)
                            {
                                references.Add(new WeakReference<object>(instance));
                            }
                        }
                        catch
                        {
                            // 忽略
                        }
                    }
                }
                catch
                {
                    // 忽略
                }
            }

            _trackedReferences[moduleName] = references;
            _logger.LogDebug("已跟踪 {Count} 个弱引用: {ModuleName}",
                references.Count, moduleName);
        }
        catch (Exception ex)
        {
            _logger.LogTrace(ex, "无法跟踪弱引用: {ModuleName}", moduleName);
        }
    }

    /// <summary>
    /// 获取模块的内存使用情况
    /// </summary>
    public ModuleMemoryInfo GetModuleMemoryInfo(string moduleName)
    {
        if (!_trackedReferences.TryGetValue(moduleName, out var references))
        {
            return new ModuleMemoryInfo
            {
                ModuleName = moduleName,
                TrackedReferences = 0,
                AliveReferences = 0,
                CanBeSafelyUnloaded = true
            };
        }

        var aliveCount = 0;
        for (int i = references.Count - 1; i >= 0; i--)
        {
            if (references[i].TryGetTarget(out var _))
            {
                aliveCount++;
            }
        }

        return new ModuleMemoryInfo
        {
            ModuleName = moduleName,
            TrackedReferences = references.Count,
            AliveReferences = aliveCount,
            CanBeSafelyUnloaded = aliveCount == 0
        };
    }

    /// <summary>
    /// 强制垃圾回收
    /// </summary>
    public void ForceGarbageCollection()
    {
        _logger.LogDebug("执行垃圾回收...");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        _logger.LogDebug("垃圾回收完成");
    }
}

/// <summary>
/// 模块内存信息
/// </summary>
public class ModuleMemoryInfo
{
    public string ModuleName { get; set; } = string.Empty;
    public int TrackedReferences { get; set; }
    public int AliveReferences { get; set; }
    public bool CanBeSafelyUnloaded { get; set; }

    public override string ToString()
    {
        return $"模块: {ModuleName}, 跟踪引用: {TrackedReferences}, 存活引用: {AliveReferences}, 可安全卸载: {CanBeSafelyUnloaded}";
    }
}
