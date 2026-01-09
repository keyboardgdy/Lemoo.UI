using Lemoo.Core.Abstractions.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Resources;
using System.Text.Json;

namespace Lemoo.Core.Infrastructure.Localization;

/// <summary>
/// 资源文件本地化服务实现
/// </summary>
public class ResourceFileLocalizationService : ILocalizationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ResourceFileLocalizationService> _logger;
    private readonly Dictionary<string, JsonResourceManager> _resourceManagers = new();
    private readonly string _resourcesPath;
    private string _currentCulture;

    public ResourceFileLocalizationService(
        IConfiguration configuration,
        ILogger<ResourceFileLocalizationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _resourcesPath = configuration.GetValue<string>("Lemoo:Localization:ResourcesPath") 
            ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
        _currentCulture = configuration.GetValue<string>("Lemoo:Localization:DefaultCulture") 
            ?? CultureInfo.CurrentCulture.Name;
        
        LoadResourceManagers();
    }

    public string CurrentCulture
    {
        get => _currentCulture;
        set
        {
            if (_currentCulture != value)
            {
                var oldCulture = _currentCulture;
                _currentCulture = value;
                CultureChanged?.Invoke(this, new CultureChangedEventArgs(oldCulture, value));
                _logger.LogInformation("文化已切换: {OldCulture} -> {NewCulture}", oldCulture, value);
            }
        }
    }

    public event EventHandler<CultureChangedEventArgs>? CultureChanged;

    public string GetString(string key, params object[] args)
    {
        return GetString(key, CurrentCulture, args);
    }

    public string GetString(string key, string? culture, params object[] args)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;

        culture ??= CurrentCulture;
        var cultureInfo = new CultureInfo(culture);
        
        // 尝试从资源文件获取
        var resourceManager = GetResourceManager(cultureInfo);
        if (resourceManager != null)
        {
            var value = resourceManager.GetString(key, cultureInfo);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return args.Length > 0 ? string.Format(cultureInfo, value, args) : value;
            }
        }
        
        // 如果找不到，返回键名
        _logger.LogWarning("未找到本地化资源: Key={Key}, Culture={Culture}", key, culture);
        return args.Length > 0 ? string.Format(cultureInfo, key, args) : key;
    }

    public string GetStringOrDefault(string key, string defaultValue, params object[] args)
    {
        var value = GetString(key, args);
        return value == key ? defaultValue : value;
    }

    public bool HasKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;

        var cultureInfo = new CultureInfo(CurrentCulture);
        var resourceManager = GetResourceManager(cultureInfo);
        
        if (resourceManager != null)
        {
            var value = resourceManager.GetString(key, cultureInfo);
            return !string.IsNullOrWhiteSpace(value);
        }
        
        return false;
    }

    public IReadOnlyList<string> GetSupportedCultures()
    {
        var cultures = new List<string>();
        
        if (Directory.Exists(_resourcesPath))
        {
            var resourceFiles = Directory.GetFiles(_resourcesPath, "*.resx", SearchOption.AllDirectories);
            foreach (var file in resourceFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var parts = fileName.Split('.');
                if (parts.Length >= 2)
                {
                    var culture = parts[^1]; // 获取最后一部分作为文化代码
                    if (!cultures.Contains(culture))
                    {
                        cultures.Add(culture);
                    }
                }
            }
        }
        
        // 如果没有找到特定文化，至少返回默认文化
        if (cultures.Count == 0)
        {
            cultures.Add(CultureInfo.CurrentCulture.Name);
        }
        
        return cultures;
    }

    private JsonResourceManager? GetResourceManager(CultureInfo cultureInfo)
    {
        var baseName = $"Resources.{cultureInfo.Name}";
        
        if (_resourceManagers.TryGetValue(baseName, out var manager))
        {
            return manager;
        }
        
        // 尝试加载资源文件
        var resourcePath = Path.Combine(_resourcesPath, $"{baseName}.resx");
        if (File.Exists(resourcePath))
        {
            try
            {
                // 这里简化处理，实际应该使用ResXResourceReader
                // 为了简化，我们使用JSON格式的资源文件
                var jsonPath = Path.Combine(_resourcesPath, $"{baseName}.json");
                if (File.Exists(jsonPath))
                {
                    var json = File.ReadAllText(jsonPath);
                    var resources = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    
                    if (resources != null)
                    {
                        var resourceManager = new JsonResourceManager(resources);
                        _resourceManagers[baseName] = resourceManager;
                        return resourceManager;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "加载资源文件失败: {ResourcePath}", resourcePath);
            }
        }
        
        return null;
    }

    private void LoadResourceManagers()
    {
        if (!Directory.Exists(_resourcesPath))
        {
            Directory.CreateDirectory(_resourcesPath);
            _logger.LogInformation("创建资源目录: {ResourcesPath}", _resourcesPath);
            return;
        }
        
        var jsonFiles = Directory.GetFiles(_resourcesPath, "*.json", SearchOption.TopDirectoryOnly);
        foreach (var file in jsonFiles)
        {
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var json = File.ReadAllText(file);
                var resources = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                
                if (resources != null)
                {
                    _resourceManagers[fileName] = new JsonResourceManager(resources);
                    _logger.LogDebug("加载资源文件: {FileName}, 资源数量: {Count}", fileName, resources.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "加载资源文件失败: {FilePath}", file);
            }
        }
    }

    private class JsonResourceManager
    {
        private readonly Dictionary<string, string> _resources;

        public JsonResourceManager(Dictionary<string, string> resources)
        {
            _resources = resources;
        }

        public string? GetString(string name, CultureInfo culture)
        {
            return _resources.TryGetValue(name, out var value) ? value : null;
        }
    }
}

