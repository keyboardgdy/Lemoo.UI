using System;
using System.IO;
using System.Threading.Tasks;
using Lemoo.UI.Helpers;
using Lemoo.UI.WPF.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lemoo.UI.WPF.Services;

/// <summary>
/// 用户设置服务实现
/// 将用户设置持久化到本地 JSON 文件
/// </summary>
public class UserSettingsService : IUserSettingsService
{
    private const string SettingsFileName = "user-settings.json";
    private readonly string _settingsFilePath;
    private UserSettings _currentSettings;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// 设置变更事件
    /// </summary>
    public event EventHandler<SettingsChangedEventArgs>? SettingsChanged;

    /// <summary>
    /// 当前用户设置
    /// </summary>
    public UserSettings CurrentSettings => _currentSettings;

    /// <summary>
    /// 构造函数
    /// </summary>
    public UserSettingsService()
    {
        // 获取本地应用数据路径
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(localAppData, "Lemoo.UI");
        _settingsFilePath = Path.Combine(appFolder, SettingsFileName);

        // 配置 JSON 序列化选项
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        // 初始化默认设置
        _currentSettings = new UserSettings();
    }

    /// <summary>
    /// 加载用户设置
    /// </summary>
    public async Task<UserSettings> LoadAsync()
    {
        try
        {
            // 如果文件不存在，返回默认设置
            if (!File.Exists(_settingsFilePath))
            {
                System.Diagnostics.Debug.WriteLine($"UserSettingsService: 设置文件不存在，使用默认设置");
                await SaveAsync(); // 创建默认设置文件
                return _currentSettings;
            }

            // 读取 JSON 文件
            var json = await File.ReadAllTextAsync(_settingsFilePath);
            var settings = JsonSerializer.Deserialize<UserSettings>(json, _jsonOptions);

            if (settings != null)
            {
                _currentSettings = settings;
                System.Diagnostics.Debug.WriteLine($"UserSettingsService: 成功加载用户设置，最后更新时间: {settings.LastUpdated}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"UserSettingsService: 反序列化失败，使用默认设置");
            }

            return _currentSettings;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"UserSettingsService: 加载设置失败: {ex.Message}");
            return _currentSettings;
        }
    }

    /// <summary>
    /// 保存用户设置
    /// </summary>
    public async Task SaveAsync(UserSettings? settings = null)
    {
        try
        {
            var settingsToSave = settings ?? _currentSettings;
            settingsToSave.LastUpdated = DateTime.Now;

            // 确保目录存在
            var directory = Path.GetDirectoryName(_settingsFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 序列化为 JSON
            var json = JsonSerializer.Serialize(settingsToSave, _jsonOptions);

            // 写入文件
            await File.WriteAllTextAsync(_settingsFilePath, json);

            System.Diagnostics.Debug.WriteLine($"UserSettingsService: 成功保存用户设置到: {_settingsFilePath}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"UserSettingsService: 保存设置失败: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 更新主题设置
    /// </summary>
    public async Task UpdateThemeAsync(string themeName)
    {
        var oldTheme = _currentSettings.Theme.CurrentTheme;

        if (oldTheme == themeName)
            return;

        _currentSettings.Theme.CurrentTheme = themeName;

        // 应用主题
        if (Enum.TryParse<ThemeManager.Theme>(themeName, out var theme))
        {
            ThemeManager.CurrentTheme = theme;
        }

        await SaveAsync();
        OnSettingsChanged("Theme", oldTheme, themeName);
    }

    /// <summary>
    /// 更新窗口设置
    /// </summary>
    public async Task UpdateWindowAsync(double width, double height, double left, double top, string windowState)
    {
        var oldSettings = new WindowSettings
        {
            Width = _currentSettings.Window.Width,
            Height = _currentSettings.Window.Height,
            Left = _currentSettings.Window.Left,
            Top = _currentSettings.Window.Top,
            WindowState = _currentSettings.Window.WindowState
        };

        _currentSettings.Window.Width = width;
        _currentSettings.Window.Height = height;
        // 将 NaN 转换为 0 以确保可序列化
        _currentSettings.Window.Left = double.IsNaN(left) ? 0 : left;
        _currentSettings.Window.Top = double.IsNaN(top) ? 0 : top;
        _currentSettings.Window.WindowState = windowState;

        await SaveAsync();
        OnSettingsChanged("Window", oldSettings, _currentSettings.Window);
    }

    /// <summary>
    /// 重置为默认设置
    /// </summary>
    public async Task ResetToDefaultAsync()
    {
        var oldSettings = _currentSettings;
        _currentSettings = new UserSettings();

        await SaveAsync();

        // 应用默认主题
        ThemeManager.CurrentTheme = ThemeManager.Theme.Base;

        OnSettingsChanged("All", oldSettings, _currentSettings);
    }

    /// <summary>
    /// 触发设置变更事件
    /// </summary>
    protected virtual void OnSettingsChanged(string key, object? oldValue, object? newValue)
    {
        SettingsChanged?.Invoke(this, new SettingsChangedEventArgs(key, oldValue, newValue));
    }
}
