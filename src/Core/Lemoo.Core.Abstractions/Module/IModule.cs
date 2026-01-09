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
    /// 依赖的模块名称列表
    /// </summary>
    IReadOnlyList<string> Dependencies { get; }
    
    /// <summary>
    /// 模块元数据
    /// </summary>
    ModuleMetadata Metadata { get; }
    
    /// <summary>
    /// 预配置服务（在其他模块配置之前）
    /// </summary>
    void PreConfigureServices(IServiceCollection services, IConfiguration configuration);
    
    /// <summary>
    /// 配置服务
    /// </summary>
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    
    /// <summary>
    /// 后配置服务（在其他模块配置之后）
    /// </summary>
    void PostConfigureServices(IServiceCollection services, IConfiguration configuration);
    
    /// <summary>
    /// 配置数据库上下文
    /// </summary>
    void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder, IConfiguration configuration);
    
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
}

