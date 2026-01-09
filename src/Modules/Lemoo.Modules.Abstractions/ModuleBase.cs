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
    
    public ModuleMetadata Metadata => new()
    {
        Name = Name,
        Version = Version,
        Description = Description,
        Dependencies = Dependencies,
        AssemblyName = GetType().Assembly.GetName().Name ?? string.Empty
    };
    
    public virtual void PreConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // 子类可以重写以在配置服务之前执行操作
    }
    
    public abstract void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    
    public virtual void PostConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // 子类可以重写以在配置服务之后执行操作
    }
    
    public abstract void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder, IConfiguration configuration);
    
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

