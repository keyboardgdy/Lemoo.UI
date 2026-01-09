using Lemoo.Core.Abstractions.Caching;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Lemoo.Core.Infrastructure.Caching;

/// <summary>
/// 内存缓存服务实现 - 支持模式匹配删除
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ConcurrentDictionary<string, object> _keyIndex = new();
    private readonly object _lockObject = new();

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        _memoryCache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var options = new MemoryCacheEntryOptions();
        
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration;
        }
        else
        {
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        }
        
        // 注册移除回调，清理键索引
        options.RegisterPostEvictionCallback((key, value, reason, state) =>
        {
            if (key is string keyString)
            {
                _keyIndex.TryRemove(keyString, out _);
            }
        });
        
        _memoryCache.Set(key, value, options);
        
        // 添加到键索引
        _keyIndex.TryAdd(key, null!);
        
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _memoryCache.Remove(key);
        _keyIndex.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // 将通配符模式转换为正则表达式
        var regexPattern = "^" + Regex.Escape(pattern)
            .Replace("\\*", ".*")
            .Replace("\\?", ".") + "$";
        
        var regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
        // 查找匹配的键
        var keysToRemove = _keyIndex.Keys
            .Where(key => regex.IsMatch(key))
            .ToList();
        
        // 删除匹配的键
        foreach (var key in keysToRemove)
        {
            _memoryCache.Remove(key);
            _keyIndex.TryRemove(key, out _);
        }
        
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var exists = _memoryCache.TryGetValue(key, out _);
        return Task.FromResult(exists);
    }
}

