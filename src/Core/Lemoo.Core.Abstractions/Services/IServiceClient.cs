namespace Lemoo.Core.Abstractions.Services;

/// <summary>
/// 服务客户端接口 - 用于统一本地和API模式的服务调用
/// </summary>
/// <typeparam name="TService">服务类型</typeparam>
public interface IServiceClient<TService> where TService : class
{
    /// <summary>
    /// 执行带返回值的操作
    /// </summary>
    Task<TResult> ExecuteAsync<TResult>(Func<TService, Task<TResult>> operation);
    
    /// <summary>
    /// 执行无返回值的操作
    /// </summary>
    Task ExecuteAsync(Func<TService, Task> operation);
}

