using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lemoo.UI.Helpers;
using Lemoo.UI.WPF.Services;
using System.Collections.ObjectModel;

namespace Lemoo.UI.WPF.ViewModels.Pages;

/// <summary>
/// 设置页面视图模型
/// </summary>
public partial class SettingsViewModel : ObservableObject, IDisposable
{
    private readonly IUserSettingsService _userSettingsService;

    /// <summary>
    /// 所有可用主题
    /// </summary>
    public ObservableCollection<ThemeInfo> Themes { get; }

    [ObservableProperty]
    private ThemeInfo? _selectedTheme;

    /// <summary>
    /// 构造函数
    /// </summary>
    public SettingsViewModel(IUserSettingsService userSettingsService)
    {
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));

        // 初始化主题列表
        Themes = new ObservableCollection<ThemeInfo>(ThemeManager.AvailableThemes);

        // 设置当前选中的主题
        UpdateSelectedTheme();

        // 监听主题变化
        ThemeManager.ThemeChanged += OnThemeChanged;
    }

    /// <summary>
    /// 更新选中的主题
    /// </summary>
    private void UpdateSelectedTheme()
    {
        var currentTheme = ThemeManager.CurrentTheme;
        SelectedTheme = Themes.FirstOrDefault(t => t.ThemeType == currentTheme);
    }

    /// <summary>
    /// 主题变化事件处理
    /// </summary>
    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        UpdateSelectedTheme();
    }

    /// <summary>
    /// 选择主题
    /// </summary>
    [RelayCommand]
    private async Task SelectTheme(ThemeInfo? theme)
    {
        if (theme != null)
        {
            // 保存主题设置到用户配置
            await _userSettingsService.UpdateThemeAsync(theme.ThemeType.ToString());

            // ThemeManager.CurrentTheme 会在 UpdateThemeAsync 中设置
            // 选中状态会在 OnThemeChanged 中通过 SelectedTheme 更新
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        ThemeManager.ThemeChanged -= OnThemeChanged;
    }
}
