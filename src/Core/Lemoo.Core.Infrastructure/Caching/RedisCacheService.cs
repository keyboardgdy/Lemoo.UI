using Lemoo.Core.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Lemoo.Core.Infrastructure.Caching;

/// <summary>
/// Redis缓存服务实现 - 支持分布式缓存
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly HashSet<string> _keyIndex = new();

    public RedisCacheService(IDistributedCache distributedCache, ILogger<RedisCacheService> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cachedBytes = await _distributedCache.GetAsync(key, cancellationToken);
            if (cachedBytes == null || cachedBytes.Length == 0)
                return default;

            var json = System.Text.Encoding.UTF8.GetString(cachedBytes);
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "从Redis获取缓存失败: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);

            var options = new DistributedCacheEntryOptions();
            if (expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiration;
            }
            else
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            }

            await _distributedCache.SetAsync(key, bytes, options, cancellationToken);
            
            lock (_keyIndex)
            {
                _keyIndex.Add(key);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "设置Redis缓存失败: {Key}", key);
            throw;
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
            
            lock (_keyIndex)
            {
                _keyIndex.Remove(key);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除Redis缓存失败: {Key}", key);
            throw;
        }
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            // 将通配符模式转换为正则表达式
            var regexPattern = "^" + Regex.Escape(pattern)
                .Replace("\\*", ".*")
                .Replace("\\?", ".") + "$";
            
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            
            // 查找匹配的键
            List<string> keysToRemove;
            lock (_keyIndex)
            {
                keysToRemove = _keyIndex.Where(key => regex.IsMatch(key)).ToList();
            }
            
            // 删除匹配的键
            var tasks = keysToRemove.Select(key => RemoveAsync(key, cancellationToken));
            await Task.WhenAll(tasks);
            
            _logger.LogInformation("根据模式删除缓存: {Pattern}, 删除数量: {Count}", pattern, keysToRemove.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根据模式删除Redis缓存失败: {Pattern}", pattern);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cachedBytes = await _distributedCache.GetAsync(key, cancellationToken);
            return cachedBytes != null && cachedBytes.Length > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查Redis缓存是否存在失败: {Key}", key);
            return false;
        }
    }
}

