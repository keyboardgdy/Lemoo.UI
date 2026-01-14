using Lemoo.Core.Abstractions.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Lemoo.Modules.Abstractions;

/// <summary>
/// 模块基类
/// </summary>
public abstract class ModuleBase : IModule
{
    public abstract string Name { get; }
    public abstract string Version { get; }
    public abstract string Description { get; }
    public virtual IReadOnlyList<string> Dependencies => Array.Empty<string>();

    /// <summary>
    /// 依赖的模块列表（支持版本范围）
    /// </summary>
    public virtual IReadOnlyList<ModuleDependency> DependencyModules => Array.Empty<ModuleDependency>();

    /// <summary>
    /// 导出的服务类型（供其他模块使用）
    /// </summary>
    public virtual IReadOnlyList<string> ExportedServices => Array.Empty<string>();

    /// <summary>
    /// 导入的服务类型（从其他模块导入）
    /// </summary>
    public virtual IReadOnlyList<string> ImportedServices => Array.Empty<string>();

    public ModuleMetadata Metadata => new()
    {
        Name = Name,
        Version = Version,
        Description = Description,
        Dependencies = Dependencies,
        AssemblyName = GetType().Assembly.GetName().Name ?? string.Empty,
        ExportedServices = ExportedServices,
        ImportedServices = ImportedServices
    };

    public virtual void PreConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // 子类可以重写以在配置服务之前执行操作
    }

    public virtual Task PreConfigureServicesAsync(IServiceCollection services, IConfiguration configuration, CancellationToken cancellationToken = default)
    {
        // 默认调用同步方法，子类可以重写以提供异步实现
        PreConfigureServices(services, configuration);
        return Task.CompletedTask;
    }

    public abstract void ConfigureServices(IServiceCollection services, IConfiguration configuration);

    public virtual Task ConfigureServicesAsync(IServiceCollection services, IConfiguration configuration, CancellationToken cancellationToken = default)
    {
        // 默认调用同步方法，子类可以重写以提供异步实现
        ConfigureServices(services, configuration);
        return Task.CompletedTask;
    }

    public virtual void PostConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // 子类可以重写以在配置服务之后执行操作
    }

    public virtual Task PostConfigureServicesAsync(IServiceCollection services, IConfiguration configuration, CancellationToken cancellationToken = default)
    {
        // 默认调用同步方法，子类可以重写以提供异步实现
        PostConfigureServices(services, configuration);
        return Task.CompletedTask;
    }

    public abstract void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder, IConfiguration configuration);

    public virtual Task ConfigureDbContextAsync(DbContextOptionsBuilder optionsBuilder, IConfiguration configuration, CancellationToken cancellationToken = default)
    {
        // 默认调用同步方法，子类可以重写以提供异步实现
        ConfigureDbContext(optionsBuilder, configuration);
        return Task.CompletedTask;
    }

    public virtual void ConfigureDbContext(ModelBuilder modelBuilder)
    {
        // 子类可以重写以配置数据库模型
    }

    public virtual Task OnApplicationStartingAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnApplicationStartedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnApplicationStoppingAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnApplicationStoppedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
