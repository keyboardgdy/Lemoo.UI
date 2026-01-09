using System.Text.Json;
using Lemoo.Core.Abstractions.Caching;
using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Application.Attributes;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lemoo.Core.Application.Behaviors;

/// <summary>
/// 缓存管道行为 - 自动处理查询结果的缓存
/// </summary>
/// <typeparam name="TRequest">请求类型</typeparam>
/// <typeparam name="TResponse">返回类型</typeparam>
public class CacheBehavior<TRequest, TResponse> : MediatR.IPipelineBehavior<TRequest, TResponse>
    where TRequest : MediatR.IRequest<TResponse>
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CacheBehavior<TRequest, TResponse>> _logger;
    
    public CacheBehavior(
        ICacheService cacheService,
        ILogger<CacheBehavior<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }
    
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // 只对查询启用缓存，命令不使用缓存
        // 检查请求类型是否实现了 IQuery<TResponse> 接口
        var isQuery = request.GetType()
            .GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>));
        
        if (!isQuery)
        {
            return await next();
        }
        
        var cacheAttribute = typeof(TRequest).GetCustomAttributes(typeof(CacheAttribute), true)
            .FirstOrDefault() as CacheAttribute;
        
        if (cacheAttribute == null)
        {
            return await next();
        }
        
        var cacheKey = GenerateCacheKey(request, cacheAttribute);
        
        // 尝试从缓存获取
        var cachedValue = await _cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            _logger.LogDebug("缓存命中: {CacheKey}", cacheKey);
            return cachedValue;
        }
        
        // 缓存未命中，执行查询
        _logger.LogDebug("缓存未命中: {CacheKey}", cacheKey);
        var response = await next();
        
        // 将结果存入缓存
        if (response != null)
        {
            await _cacheService.SetAsync(
                cacheKey,
                response,
                cacheAttribute.Expiration,
                cancellationToken);
            _logger.LogDebug("结果已缓存: {CacheKey}, 过期时间: {Expiration}", 
                cacheKey, cacheAttribute.Expiration);
        }
        
        return response;
    }
    
    private string GenerateCacheKey(TRequest request, CacheAttribute attribute)
    {
        // 确保前缀包含请求类型，避免不同请求类型之间的冲突
        var requestTypeName = typeof(TRequest).Name;
        var prefix = attribute.KeyPrefix != null 
            ? $"{attribute.KeyPrefix}:{requestTypeName}" 
            : requestTypeName;
        
        // 使用更稳定的哈希算法生成缓存键
        var requestJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
        
        // 使用SHA256哈希确保缓存键的唯一性和稳定性
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(requestJson));
        var hashString = Convert.ToBase64String(hashBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
        
        return $"{prefix}:{hashString}";
    }
}

