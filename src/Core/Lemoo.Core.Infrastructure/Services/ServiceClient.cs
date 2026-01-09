using Lemoo.Core.Abstractions.Deployment;
using Lemoo.Core.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace Lemoo.Core.Infrastructure.Services;

/// <summary>
/// 服务客户端实现 - 支持本地和HTTP模式
/// </summary>
/// <typeparam name="TService">服务类型</typeparam>
public class ServiceClient<TService> : IServiceClient<TService>
    where TService : class
{
    private readonly IDeploymentModeService _deploymentMode;
    private readonly IServiceProvider _serviceProvider;
    private readonly HttpClient? _httpClient;
    private readonly ILogger<ServiceClient<TService>> _logger;
    private readonly string? _apiBaseUrl;

    public ServiceClient(
        IDeploymentModeService deploymentMode,
        IServiceProvider serviceProvider,
        IHttpClientFactory? httpClientFactory,
        ILogger<ServiceClient<TService>> logger,
        IConfiguration configuration)
    {
        _deploymentMode = deploymentMode;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _apiBaseUrl = configuration.GetValue<string>("Lemoo:Api:BaseUrl");
        
        if (_deploymentMode.IsApiMode && httpClientFactory != null)
        {
            _httpClient = httpClientFactory.CreateClient(typeof(TService).Name);
            if (_httpClient != null && !string.IsNullOrWhiteSpace(_apiBaseUrl))
            {
                _httpClient.BaseAddress = new Uri(_apiBaseUrl);
            }
        }
    }

    public async Task<TResult> ExecuteAsync<TResult>(Func<TService, Task<TResult>> operation)
    {
        if (_deploymentMode.IsLocalMode)
        {
            // 本地模式：直接调用服务
            var service = _serviceProvider.GetRequiredService<TService>();
            return await operation(service);
        }
        else if (_deploymentMode.IsApiMode && _httpClient != null)
        {
            // API模式：通过HTTP调用
            return await ExecuteHttpAsync(operation);
        }
        else
        {
            throw new InvalidOperationException("无法确定服务调用模式");
        }
    }

    public async Task ExecuteAsync(Func<TService, Task> operation)
    {
        if (_deploymentMode.IsLocalMode)
        {
            // 本地模式：直接调用服务
            var service = _serviceProvider.GetRequiredService<TService>();
            await operation(service);
        }
        else if (_deploymentMode.IsApiMode && _httpClient != null)
        {
            // API模式：通过HTTP调用
            await ExecuteHttpAsync(operation);
        }
        else
        {
            throw new InvalidOperationException("无法确定服务调用模式");
        }
    }

    private async Task<TResult> ExecuteHttpAsync<TResult>(Func<TService, Task<TResult>> operation)
    {
        if (_httpClient == null || string.IsNullOrWhiteSpace(_apiBaseUrl))
        {
            _logger.LogWarning("HTTP客户端未配置，回退到本地模式");
            var service = _serviceProvider.GetRequiredService<TService>();
            return await operation(service);
        }

        // HTTP模式：通过反射获取操作信息并发送HTTP请求
        // 注意：这是一个简化实现，实际应用中应该使用代理模式或代码生成
        
        // 尝试从操作中提取方法信息
        var operationMethod = operation.Method;
        var serviceType = typeof(TService);
        
        // 构建API端点路径（约定：服务名/方法名）
        var serviceName = serviceType.Name.Replace("Service", "").ToLowerInvariant();
        var methodName = operationMethod.Name.Replace("Async", "").ToLowerInvariant();
        var endpoint = $"/api/{serviceName}/{methodName}";
        
            try
            {
                // 发送HTTP POST请求
                var response = await _httpClient.PostAsync(endpoint, null);
                response.EnsureSuccessStatusCode();
                
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TResult>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                if (result == null)
                {
                    throw new InvalidOperationException("HTTP响应反序列化失败");
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HTTP调用失败: {Endpoint}", endpoint);
                throw;
            }
    }

    private async Task ExecuteHttpAsync(Func<TService, Task> operation)
    {
        if (_httpClient == null || string.IsNullOrWhiteSpace(_apiBaseUrl))
        {
            _logger.LogWarning("HTTP客户端未配置，回退到本地模式");
            var service = _serviceProvider.GetRequiredService<TService>();
            await operation(service);
            return;
        }

        var operationMethod = operation.Method;
        var serviceType = typeof(TService);
        
        var serviceName = serviceType.Name.Replace("Service", "").ToLowerInvariant();
        var methodName = operationMethod.Name.Replace("Async", "").ToLowerInvariant();
        var endpoint = $"/api/{serviceName}/{methodName}";
        
        try
        {
            var response = await _httpClient.PostAsync(endpoint, null);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HTTP调用失败: {Endpoint}", endpoint);
            throw;
        }
    }
}

