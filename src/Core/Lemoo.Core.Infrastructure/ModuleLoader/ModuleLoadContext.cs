using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Lemoo.Core.Infrastructure.ModuleLoader;

/// <summary>
/// 模块程序集加载上下文 - 支持可卸载的模块加载
/// </summary>
public class ModuleLoadContext : AssemblyLoadContext
{
    private readonly string _moduleName;
    private readonly ILogger<ModuleLoadContext> _logger;
    private readonly AssemblyDependencyResolver _resolver;

    /// <summary>
    /// 模块名称
    /// </summary>
    public string ModuleName => _moduleName;

    /// <summary>
    /// 模块程序集路径
    /// </summary>
    public string? ModuleAssemblyPath { get; private set; }

    public ModuleLoadContext(
        string moduleName,
        string modulePath,
        ILogger<ModuleLoadContext> logger)
        : base(moduleName, isCollectible: true)
    {
        _moduleName = moduleName;
        _logger = logger;
        _resolver = new AssemblyDependencyResolver(modulePath);
        ModuleAssemblyPath = modulePath;

        _logger.LogDebug("创建模块加载上下文: {ModuleName}, Path: {ModulePath}", moduleName, modulePath);
    }

    /// <summary>
    /// 加载程序集时的自定义解析逻辑
    /// </summary>
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        _logger.LogTrace("尝试加载程序集: {AssemblyName}", assemblyName.Name);

        // 1. 首先尝试从依赖解析器加载（处理模块依赖）
        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if (assemblyPath != null)
        {
            _logger.LogDebug("从依赖解析器加载程序集: {AssemblyName} from {AssemblyPath}", assemblyName.Name, assemblyPath);
            return LoadFromAssemblyPath(assemblyPath);
        }

        // 2. 尝试从默认加载上下文加载（处理共享依赖）
        try
        {
            var defaultAssembly = AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
            if (defaultAssembly != null)
            {
                _logger.LogDebug("从默认加载上下文加载程序集: {AssemblyName}", assemblyName.Name);
                return defaultAssembly;
            }
        }
        catch
        {
            // 忽略，继续尝试其他方式
        }

        // 3. 返回 null 让框架使用默认加载逻辑
        _logger.LogTrace("无法解析程序集，使用默认加载逻辑: {AssemblyName}", assemblyName.Name);
        return null;
    }

    /// <summary>
    /// 卸载模块加载上下文
    /// </summary>
    public async Task<bool> UnloadAsync(TimeSpan timeout = default)
    {
        var actualTimeout = timeout == default ? TimeSpan.FromSeconds(30) : timeout;

        _logger.LogInformation("开始卸载模块加载上下文: {ModuleName}", _moduleName);

        try
        {
            // 1. 触发卸载
            Unload();

            // 2. 等待卸载完成
            var cts = new CancellationTokenSource(actualTimeout);
            var startTime = DateTime.UtcNow;

            while (!IsUnloaded)
            {
                await Task.Delay(50, cts.Token);

                // 强制 GC 以加速卸载
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                if (DateTime.UtcNow - startTime > actualTimeout)
                {
                    _logger.LogWarning("模块卸载超时: {ModuleName}", _moduleName);
                    return false;
                }
            }

            _logger.LogInformation("模块卸载成功: {ModuleName}", _moduleName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "模块卸载失败: {ModuleName}", _moduleName);
            return false;
        }
    }

    /// <summary>
    /// 检查上下文是否已卸载
    /// </summary>
    private bool IsUnloaded
    {
        get
        {
            try
            {
                // 尝试访问上下文，如果已卸载会抛出异常
                var _ = Name;
                return false;
            }
            catch (ObjectDisposedException)
            {
                return true;
            }
        }
    }
}
