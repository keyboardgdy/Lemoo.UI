using Lemoo.Core.Abstractions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Lemoo.Core.Infrastructure.Configuration;

/// <summary>
/// 配置服务实现 - 支持配置热更新
/// </summary>
public class ConfigurationService : IConfigurationService, IDisposable
{
    private readonly IConfigurationRoot _configurationRoot;
    private readonly IServiceProvider _serviceProvider;
    private IChangeToken? _changeToken;
    private IDisposable? _changeTokenRegistration;

    public ConfigurationService(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configurationRoot = configuration as IConfigurationRoot 
            ?? throw new ArgumentException("配置必须是IConfigurationRoot类型以支持热更新", nameof(configuration));
        _serviceProvider = serviceProvider;
        
        // 监听配置变更
        _changeToken = _configurationRoot.GetReloadToken();
        _changeTokenRegistration = ChangeToken.OnChange(
            () => _configurationRoot.GetReloadToken(),
            OnConfigurationChanged);
    }

    public event EventHandler<ConfigurationChangedEventArgs>? ConfigurationChanged;

    public T? GetValue<T>(string key)
    {
        return _configurationRoot.GetValue<T?>(key);
    }

    public T GetValue<T>(string key, T defaultValue)
    {
        return _configurationRoot.GetValue(key, defaultValue) ?? defaultValue;
    }

    public IConfigurationSection GetSection(string key)
    {
        return _configurationRoot.GetSection(key);
    }

    public string? GetConnectionString(string name)
    {
        return _configurationRoot.GetConnectionString(name);
    }

    public IOptionsMonitor<T> GetOptionsMonitor<T>() where T : class
    {
        return _serviceProvider.GetRequiredService<IOptionsMonitor<T>>();
    }

    public void Reload()
    {
        _configurationRoot.Reload();
    }

    private void OnConfigurationChanged()
    {
        // 触发配置变更事件
        // 注意：这里简化处理，实际应该比较新旧值
        ConfigurationChanged?.Invoke(this, new ConfigurationChangedEventArgs("Configuration", null, null));
        
        // 更新变更令牌
        _changeToken = _configurationRoot.GetReloadToken();
    }

    public void Dispose()
    {
        _changeTokenRegistration?.Dispose();
    }
}

