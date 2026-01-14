using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Lemoo.Core.Abstractions.Module;

/// <summary>
/// 模块接口
/// </summary>
public interface IModule
{
    /// <summary>
    /// 模块名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 模块版本
    /// </summary>
    string Version { get; }

    /// <summary>
    /// 模块描述
    /// </summary>
    string Description { get; }

    /// <summary>
    /// 依赖的模块名称列表（已过时，请使用 DependencyModules）
    /// </summary>
    [Obsolete("请使用 DependencyModules 属性，它支持版本范围")]
    IReadOnlyList<string> Dependencies { get; }

    /// <summary>
    /// 依赖的模块列表（支持版本范围）
    /// </summary>
    IReadOnlyList<ModuleDependency> DependencyModules { get; }

    /// <summary>
    /// 模块元数据
    /// </summary>
    ModuleMetadata Metadata { get; }

    /// <summary>
    /// 预配置服务（在其他模块配置之前）
    /// </summary>
    void PreConfigureServices(IServiceCollection services, IConfiguration configuration);

    /// <summary>
    /// 异步预配置服务（在其他模块配置之前）
    /// </summary>
    Task PreConfigureServicesAsync(IServiceCollection services, IConfiguration configuration, CancellationToken cancellationToken = default);

    /// <summary>
    /// 配置服务
    /// </summary>
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);

    /// <summary>
    /// 异步配置服务
    /// </summary>
    Task ConfigureServicesAsync(IServiceCollection services, IConfiguration configuration, CancellationToken cancellationToken = default);

    /// <summary>
    /// 后配置服务（在其他模块配置之后）
    /// </summary>
    void PostConfigureServices(IServiceCollection services, IConfiguration configuration);

    /// <summary>
    /// 异步后配置服务（在其他模块配置之后）
    /// </summary>
    Task PostConfigureServicesAsync(IServiceCollection services, IConfiguration configuration, CancellationToken cancellationToken = default);

    /// <summary>
    /// 配置数据库上下文
    /// </summary>
    void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder, IConfiguration configuration);

    /// <summary>
    /// 异步配置数据库上下文
    /// </summary>
    Task ConfigureDbContextAsync(DbContextOptionsBuilder optionsBuilder, IConfiguration configuration, CancellationToken cancellationToken = default);

    /// <summary>
    /// 配置数据库模型
    /// </summary>
    void ConfigureDbContext(ModelBuilder modelBuilder);

    /// <summary>
    /// 应用程序启动前
    /// </summary>
    Task OnApplicationStartingAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);

    /// <summary>
    /// 应用程序启动后
    /// </summary>
    Task OnApplicationStartedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);

    /// <summary>
    /// 应用程序停止前
    /// </summary>
    Task OnApplicationStoppingAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);

    /// <summary>
    /// 应用程序停止后
    /// </summary>
    Task OnApplicationStoppedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);
}

/// <summary>
/// 模块元数据
/// </summary>
public class ModuleMetadata
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IReadOnlyList<string> Dependencies { get; set; } = Array.Empty<string>();
    public string AssemblyName { get; set; } = string.Empty;

    /// <summary>
    /// 模块导出的服务类型（供其他模块使用）
    /// </summary>
    public IReadOnlyList<string> ExportedServices { get; set; } = Array.Empty<string>();

    /// <summary>
    /// 模块导入的服务类型（从其他模块导入）
    /// </summary>
    public IReadOnlyList<string> ImportedServices { get; set; } = Array.Empty<string>();

    /// <summary>
    /// 模块状态
    /// </summary>
    public ModuleState State { get; set; } = ModuleState.Loaded;
}

/// <summary>
/// 模块状态
/// </summary>
public enum ModuleState
{
    /// <summary>
    /// 已加载
    /// </summary>
    Loaded,

    /// <summary>
    /// 启动中
    /// </summary>
    Starting,

    /// <summary>
    /// 已启动
    /// </summary>
    Started,

    /// <summary>
    /// 停止中
    /// </summary>
    Stopping,

    /// <summary>
    /// 已停止
    /// </summary>
    Stopped,

    /// <summary>
    /// 已卸载
    /// </summary>
    Unloaded,

    /// <summary>
    /// 错误
    /// </summary>
    Error
}

