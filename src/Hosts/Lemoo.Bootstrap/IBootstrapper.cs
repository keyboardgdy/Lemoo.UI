using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lemoo.Bootstrap;

/// <summary>
/// 启动引导器接口
/// </summary>
public interface IBootstrapper
{
    /// <summary>
    /// 引导应用程序
    /// </summary>
    Task<BootstrapResult> BootstrapAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 注册服务
    /// </summary>
    void RegisterServices(IServiceCollection services, IConfiguration configuration);
    
    /// <summary>
    /// 配置宿主
    /// </summary>
    void ConfigureHost(IHostBuilder hostBuilder);
}

/// <summary>
/// 引导结果
/// </summary>
public class BootstrapResult
{
    public bool IsSuccess { get; set; }
    public IReadOnlyList<BootstrapError> Errors { get; set; } = Array.Empty<BootstrapError>();
    public TimeSpan ElapsedTime { get; set; }
    public string? Message { get; set; }
}

/// <summary>
/// 引导错误
/// </summary>
public class BootstrapError
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
}

