using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Lemoo.UI.Helpers;

/// <summary>
/// 专业主题管理器 - 支持原色/深色/浅色模式切换
/// 基于 VS Code 风格的主题切换方案
/// 优化：使用资源字典缓存和异步刷新提升性能
/// </summary>
public static class ThemeManager
{
    /// <summary>
    /// 主题枚举
    /// </summary>
    public enum Theme
    {
        /// <summary>
        /// 原色模式（Base 主题，当前配色方案）
        /// </summary>
        Base,
        /// <summary>
        /// 深色模式（VS Dark 风格）
        /// </summary>
        Dark,
        /// <summary>
        /// 浅色模式（VS Light 风格）
        /// </summary>
        Light,
        /// <summary>
        /// 霓虹赛博朋克主题（Neon Cyberpunk）
        /// </summary>
        NeonCyberpunk,
        /// <summary>
        /// 极光主题（Aurora Borealis）
        /// </summary>
        Aurora,
        /// <summary>
        /// 热带夕阳主题（Sunset Tropics）
        /// </summary>
        SunsetTropics,
        /// <summary>
        /// 玫瑰石英主题（Rose Quartz）
        /// </summary>
        RoseQuartz,
        /// <summary>
        /// 深海幽蓝主题（Deep Ocean）
        /// </summary>
        DeepOcean,
        /// <summary>
        /// 樱花幻梦主题（Sakura）
        /// </summary>
        Sakura,
        /// <summary>
        /// 午夜丝绒主题（Midnight Velvet）
        /// </summary>
        MidnightVelvet,
        /// <summary>
        /// 森林低语主题（Forest Whisper）
        /// </summary>
        ForestWhisper,
        /// <summary>
        /// 跟随系统主题
        /// </summary>
        System
    }

    // 主题资源字典 URI 常量（避免重复创建，提升性能）
    private static readonly Uri BaseThemeUri = new("pack://application:,,,/Lemoo.UI;component/Themes/Base/Base.xaml", UriKind.Absolute);
    private static readonly Uri DarkThemeUri = new("pack://application:,,,/Lemoo.UI;component/Themes/Dark/Dark.xaml", UriKind.Absolute);
    private static readonly Uri LightThemeUri = new("pack://application:,,,/Lemoo.UI;component/Themes/Light/Light.xaml", UriKind.Absolute);
    private static readonly Uri NeonCyberpunkThemeUri = new("pack://application:,,,/Lemoo.UI;component/Themes/NeonCyberpunk/NeonCyberpunk.xaml", UriKind.Absolute);
    private static readonly Uri AuroraThemeUri = new("pack://application:,,,/Lemoo.UI;component/Themes/Aurora/Aurora.xaml", UriKind.Absolute);
    private static readonly Uri SunsetTropicsThemeUri = new("pack://application:,,,/Lemoo.UI;component/Themes/SunsetTropics/SunsetTropics.xaml", UriKind.Absolute);
    private static readonly Uri RoseQuartzThemeUri = new("pack://application:,,,/Lemoo.UI;component/Themes/RoseQuartz/RoseQuartz.xaml", UriKind.Absolute);
    private static readonly Uri DeepOceanThemeUri = new("pack://application:,,,/Lemoo.UI;component/Themes/DeepOcean/DeepOcean.xaml", UriKind.Absolute);
    private static readonly Uri SakuraThemeUri = new("pack://application:,,,/Lemoo.UI;component/Themes/Sakura/Sakura.xaml", UriKind.Absolute);
    private static readonly Uri MidnightVelvetThemeUri = new("pack://application:,,,/Lemoo.UI;component/Themes/MidnightVelvet/MidnightVelvet.xaml", UriKind.Absolute);
    private static readonly Uri ForestWhisperThemeUri = new("pack://application:,,,/Lemoo.UI;component/Themes/ForestWhisper/ForestWhisper.xaml", UriKind.Absolute);

    // 主题资源字典路径后缀（用于快速识别）
    private static readonly string[] ThemePathSuffixes =
    {
        "/Themes/Base/Base.xaml",
        "/Themes/Dark/Dark.xaml",
        "/Themes/Light/Light.xaml",
        "/Themes/NeonCyberpunk/NeonCyberpunk.xaml",
        "/Themes/Aurora/Aurora.xaml",
        "/Themes/SunsetTropics/SunsetTropics.xaml",
        "/Themes/RoseQuartz/RoseQuartz.xaml",
        "/Themes/DeepOcean/DeepOcean.xaml",
        "/Themes/Sakura/Sakura.xaml",
        "/Themes/MidnightVelvet/MidnightVelvet.xaml",
        "/Themes/ForestWhisper/ForestWhisper.xaml"
    };

    // 资源字典缓存（避免重复创建，提升性能）
    private static readonly Dictionary<Theme, ResourceDictionary> _themeCache = new();
    
    // 当前主题资源字典的引用（用于快速更新）
    private static ResourceDictionary? _currentThemeDictionary;

    private static Theme _currentTheme = Theme.Base;

    /// <summary>
    /// 主题变化事件
    /// </summary>
    public static event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    /// <summary>
    /// 当前主题
    /// </summary>
    public static Theme CurrentTheme
    {
        get => _currentTheme;
        set
        {
            if (_currentTheme != value)
            {
                var oldTheme = _currentTheme;
                _currentTheme = value;
                ApplyTheme(value);
                ThemeChanged?.Invoke(null, new ThemeChangedEventArgs(oldTheme, value));
            }
        }
    }

    /// <summary>
    /// 获取所有可用主题的信息
    /// </summary>
    public static ReadOnlyCollection<ThemeInfo> AvailableThemes { get; }

    /// <summary>
    /// 静态构造函数 - 初始化主题信息
    /// </summary>
    static ThemeManager()
    {
        var themes = new List<ThemeInfo>
        {
            new ThemeInfo
            {
                ThemeType = Theme.Base,
                DisplayName = "原色模式",
                Description = "品牌原色配色方案",
                DetailDescription = "深色背景搭配青色强调色，展示 Lemoo 品牌特色",
                PreviewPrimaryColor = Color.FromRgb(0x1A, 0x1E, 0x24),
                PreviewSecondaryColor = Color.FromRgb(0x25, 0x2B, 0x33),
                PreviewAccentColor = Color.FromRgb(0x00, 0xAD, 0xB5),
                PreviewTextColor = Color.FromRgb(0xEE, 0xEE, 0xEE),
                IsDefault = true,
                Tag = "默认",
                Category = "Lemoo"
            },
            new ThemeInfo
            {
                ThemeType = Theme.Dark,
                DisplayName = "深色模式",
                Description = "Visual Studio Dark 风格",
                DetailDescription = "经典深色 IDE 配色，蓝色强调色，专业开发体验",
                PreviewPrimaryColor = Color.FromRgb(0x25, 0x25, 0x26),
                PreviewSecondaryColor = Color.FromRgb(0x1E, 0x1E, 0x1E),
                PreviewAccentColor = Color.FromRgb(0x00, 0x7A, 0xCC),
                PreviewTextColor = Color.FromRgb(0xE0, 0xE0, 0xE0),
                IsDefault = false,
                Category = "Visual Studio"
            },
            new ThemeInfo
            {
                ThemeType = Theme.Light,
                DisplayName = "浅色模式",
                Description = "Visual Studio Light 风格",
                DetailDescription = "清爽明亮配色方案，适合日间使用",
                PreviewPrimaryColor = Color.FromRgb(0xE8, 0xE8, 0xE8),
                PreviewSecondaryColor = Color.FromRgb(0xFA, 0xFA, 0xFA),
                PreviewAccentColor = Color.FromRgb(0x00, 0x7A, 0xCC),
                PreviewTextColor = Color.FromRgb(0x1E, 0x1E, 0x1E),
                IsDefault = false,
                Category = "Visual Studio"
            },
            new ThemeInfo
            {
                ThemeType = Theme.NeonCyberpunk,
                DisplayName = "霓虹赛博朋克",
                Description = "未来科技风格",
                DetailDescription = "霓虹色彩与深色背景的碰撞，营造强烈的科技感和未来感",
                PreviewPrimaryColor = Color.FromRgb(0x0D, 0x0D, 0x14),
                PreviewSecondaryColor = Color.FromRgb(0x0A, 0x0A, 0x0F),
                PreviewAccentColor = Color.FromRgb(0xFF, 0x00, 0x6E),
                PreviewTextColor = Color.FromRgb(0x00, 0xF5, 0xFF),
                IsDefault = false,
                Tag = "创新",
                Category = "创意主题"
            },
            new ThemeInfo
            {
                ThemeType = Theme.Aurora,
                DisplayName = "极光",
                Description = "北极光风格",
                DetailDescription = "流动的渐变色彩和柔和的光晕效果，带来宁静而神秘的视觉体验",
                PreviewPrimaryColor = Color.FromRgb(0x14, 0x1C, 0x27),
                PreviewSecondaryColor = Color.FromRgb(0x0F, 0x14, 0x19),
                PreviewAccentColor = Color.FromRgb(0x00, 0xD9, 0xA0),
                PreviewTextColor = Color.FromRgb(0xE0, 0xF7, 0xFA),
                IsDefault = false,
                Tag = "优雅",
                Category = "创意主题"
            },
            new ThemeInfo
            {
                ThemeType = Theme.SunsetTropics,
                DisplayName = "热带夕阳",
                Description = "热带海滩风格",
                DetailDescription = "温暖的大地色系与活力的珊瑚橙、青绿色的融合，充满热情与生机",
                PreviewPrimaryColor = Color.FromRgb(0xFF, 0xE5, 0xD9),
                PreviewSecondaryColor = Color.FromRgb(0xFF, 0xF8, 0xF0),
                PreviewAccentColor = Color.FromRgb(0xFF, 0x6B, 0x6B),
                PreviewTextColor = Color.FromRgb(0x2C, 0x3E, 0x50),
                IsDefault = false,
                Tag = "温暖",
                Category = "创意主题"
            },
            new ThemeInfo
            {
                ThemeType = Theme.RoseQuartz,
                DisplayName = "玫瑰石英",
                Description = "柔和优雅风格",
                DetailDescription = "柔和优雅的粉紫色调，灵感来自天然玫瑰石英的温润质感，营造温馨浪漫氛围",
                PreviewPrimaryColor = Color.FromRgb(0x1A, 0x15, 0x20),
                PreviewSecondaryColor = Color.FromRgb(0x2A, 0x22, 0x32),
                PreviewAccentColor = Color.FromRgb(0xE8, 0xB4, 0xBC),
                PreviewTextColor = Color.FromRgb(0xF5, 0xE6, 0xE8),
                IsDefault = false,
                Tag = "优雅",
                Category = "惊艳主题"
            },
            new ThemeInfo
            {
                ThemeType = Theme.DeepOcean,
                DisplayName = "深海幽蓝",
                Description = "沉稳深邃风格",
                DetailDescription = "沉稳深邃的蓝绿色调，灵感来自深邃海洋的神秘与宁静，带来专业专注感",
                PreviewPrimaryColor = Color.FromRgb(0x0B, 0x12, 0x19),
                PreviewSecondaryColor = Color.FromRgb(0x14, 0x22, 0x28),
                PreviewAccentColor = Color.FromRgb(0x00, 0xCE, 0xD1),
                PreviewTextColor = Color.FromRgb(0xE0, 0xF0, 0xF8),
                IsDefault = false,
                Tag = "专业",
                Category = "惊艳主题"
            },
            new ThemeInfo
            {
                ThemeType = Theme.Sakura,
                DisplayName = "樱花幻梦",
                Description = "唯美日式风格",
                DetailDescription = "唯美的日式樱花粉，灵感来自春日樱花飘落的梦幻场景，带来轻松愉悦的视觉体验",
                PreviewPrimaryColor = Color.FromRgb(0x1F, 0x1A, 0x24),
                PreviewSecondaryColor = Color.FromRgb(0x2D, 0x26, 0x34),
                PreviewAccentColor = Color.FromRgb(0xFF, 0xB7, 0xC5),
                PreviewTextColor = Color.FromRgb(0xFF, 0xF0, 0xF5),
                IsDefault = false,
                Tag = "梦幻",
                Category = "惊艳主题"
            },
            new ThemeInfo
            {
                ThemeType = Theme.MidnightVelvet,
                DisplayName = "午夜丝绒",
                Description = "奢华高贵风格",
                DetailDescription = "奢华的金紫配色，灵感来自午夜时分的高贵与优雅，展现精致的应用体验",
                PreviewPrimaryColor = Color.FromRgb(0x0F, 0x0A, 0x18),
                PreviewSecondaryColor = Color.FromRgb(0x1A, 0x14, 0x28),
                PreviewAccentColor = Color.FromRgb(0xD4, 0xAF, 0x37),
                PreviewTextColor = Color.FromRgb(0xF5, 0xE6, 0xF0),
                IsDefault = false,
                Tag = "奢华",
                Category = "惊艳主题"
            },
            new ThemeInfo
            {
                ThemeType = Theme.ForestWhisper,
                DisplayName = "森林低语",
                Description = "清新自然风格",
                DetailDescription = "清新的自然绿色，灵感来自清晨森林的清新与宁静，带来放松舒适的视觉感受",
                PreviewPrimaryColor = Color.FromRgb(0x0D, 0x1F, 0x12),
                PreviewSecondaryColor = Color.FromRgb(0x1A, 0x2F, 0x24),
                PreviewAccentColor = Color.FromRgb(0x52, 0xB7, 0x88),
                PreviewTextColor = Color.FromRgb(0xE0, 0xF0, 0xE8),
                IsDefault = false,
                Tag = "清新",
                Category = "惊艳主题"
            }
        };
        AvailableThemes = new ReadOnlyCollection<ThemeInfo>(themes);
    }

    /// <summary>
    /// 根据主题类型获取主题信息
    /// </summary>
    public static ThemeInfo? GetThemeInfo(Theme theme)
    {
        return AvailableThemes.FirstOrDefault(t => t.ThemeType == theme);
    }

    /// <summary>
    /// 应用主题
    /// 优化：使用资源字典缓存，避免重复创建
    /// </summary>
    /// <param name="theme">要应用的主题</param>
    public static void ApplyTheme(Theme theme)
    {
        var app = Application.Current;
        if (app == null)
        {
            System.Diagnostics.Debug.WriteLine("ThemeManager: Application.Current 为 null，无法应用主题");
            return;
        }

        var actualTheme = theme == Theme.System ? GetSystemTheme() : theme;

        // 优化：如果已经是当前主题且已应用，跳过
        if (_currentThemeDictionary != null && 
            _themeCache.TryGetValue(actualTheme, out var cachedDict) &&
            cachedDict == _currentThemeDictionary &&
            app.Resources.MergedDictionaries.Contains(_currentThemeDictionary))
        {
            System.Diagnostics.Debug.WriteLine($"ThemeManager: 主题 {actualTheme} 已应用，跳过");
            return;
        }

        // 移除旧主题资源字典（优化：只移除当前主题字典）
        if (_currentThemeDictionary != null && app.Resources.MergedDictionaries.Contains(_currentThemeDictionary))
        {
            app.Resources.MergedDictionaries.Remove(_currentThemeDictionary);
        }

        // 应用新主题
        var themeUri = actualTheme switch
        {
            Theme.Base => BaseThemeUri,
            Theme.Dark => DarkThemeUri,
            Theme.Light => LightThemeUri,
            Theme.NeonCyberpunk => NeonCyberpunkThemeUri,
            Theme.Aurora => AuroraThemeUri,
            Theme.SunsetTropics => SunsetTropicsThemeUri,
            Theme.RoseQuartz => RoseQuartzThemeUri,
            Theme.DeepOcean => DeepOceanThemeUri,
            Theme.Sakura => SakuraThemeUri,
            Theme.MidnightVelvet => MidnightVelvetThemeUri,
            Theme.ForestWhisper => ForestWhisperThemeUri,
            _ => BaseThemeUri
        };

        try
        {
            // 优化：从缓存获取或创建主题字典
            if (!_themeCache.TryGetValue(actualTheme, out var themeDict))
            {
                themeDict = new ResourceDictionary { Source = themeUri };
                _themeCache[actualTheme] = themeDict;
                System.Diagnostics.Debug.WriteLine($"ThemeManager: 创建并缓存主题 {actualTheme}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"ThemeManager: 使用缓存的主题 {actualTheme}");
            }
            
            // 确保 Resources 存在 MergedDictionaries
            if (app.Resources is not ResourceDictionary resourceDict)
            {
                // 如果 Resources 本身不是 ResourceDictionary，需要包装
                var wrapper = new ResourceDictionary();
                if (app.Resources != null)
                {
                    foreach (var key in app.Resources.Keys)
                    {
                        wrapper[key] = app.Resources[key];
                    }
                }
                app.Resources = wrapper;
                resourceDict = wrapper;
            }
            
            if (resourceDict.MergedDictionaries == null)
            {
                // 这不应该发生，但为了安全起见
                System.Diagnostics.Debug.WriteLine("ThemeManager: Resources.MergedDictionaries 为 null");
                return;
            }
            
            // 添加新主题（添加到末尾，确保优先级最高）
            resourceDict.MergedDictionaries.Add(themeDict);
            _currentThemeDictionary = themeDict;
            
            System.Diagnostics.Debug.WriteLine($"ThemeManager: 成功应用主题 {actualTheme}");
            
            // 优化：异步刷新所有窗口的资源引用，避免阻塞UI
            RefreshAllWindowsAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ThemeManager: 应用主题失败 ({actualTheme}): {ex.Message}");
            
            // 如果主题文件不存在，回退到基础主题
            if (actualTheme != Theme.Base)
            {
                try
                {
                    // 使用缓存的Base主题或创建新的
                    if (!_themeCache.TryGetValue(Theme.Base, out var fallbackDict))
                    {
                        fallbackDict = new ResourceDictionary { Source = BaseThemeUri };
                        _themeCache[Theme.Base] = fallbackDict;
                    }
                    app.Resources.MergedDictionaries.Add(fallbackDict);
                    _currentThemeDictionary = fallbackDict;
                    System.Diagnostics.Debug.WriteLine("ThemeManager: 已回退到 Base 主题");
                    RefreshAllWindowsAsync();
                }
                catch (Exception fallbackEx)
                {
                    System.Diagnostics.Debug.WriteLine($"ThemeManager: 回退到 Base 主题也失败: {fallbackEx.Message}");
                }
            }
        }
    }

    /// <summary>
    /// 移除旧主题资源字典
    /// 优化：现在使用 _currentThemeDictionary 引用直接移除，更高效
    /// 保留此方法用于清理可能遗留的主题字典（向后兼容）
    /// </summary>
    private static void RemoveOldThemes(Application app)
    {
        if (app.Resources.MergedDictionaries == null)
            return;

        // 优化：如果当前主题字典引用存在，直接移除
        if (_currentThemeDictionary != null && 
            app.Resources.MergedDictionaries.Contains(_currentThemeDictionary))
        {
            app.Resources.MergedDictionaries.Remove(_currentThemeDictionary);
            return;
        }

        // 向后兼容：如果引用不存在，使用旧方法查找并移除
        var dictionariesToRemove = new List<ResourceDictionary>();
        CollectThemeDictionaries(app.Resources, dictionariesToRemove);
        
        foreach (var dict in dictionariesToRemove)
        {
            if (app.Resources.MergedDictionaries.Contains(dict))
            {
                app.Resources.MergedDictionaries.Remove(dict);
            }
            else
            {
                RemoveFromNestedDictionaries(app.Resources.MergedDictionaries, dict);
            }
        }

        if (dictionariesToRemove.Count > 0)
        {
            System.Diagnostics.Debug.WriteLine($"ThemeManager: 已移除 {dictionariesToRemove.Count} 个旧主题资源字典");
        }
    }

    /// <summary>
    /// 递归收集所有主题资源字典
    /// </summary>
    private static void CollectThemeDictionaries(ResourceDictionary dict, List<ResourceDictionary> result)
    {
        if (dict.MergedDictionaries == null)
            return;

        foreach (var mergedDict in dict.MergedDictionaries)
        {
            if (mergedDict.Source != null)
            {
                var sourceString = mergedDict.Source.ToString();
                if (ThemePathSuffixes.Any(suffix => sourceString.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
                {
                    result.Add(mergedDict);
                }
            }
            
            // 递归检查嵌套的 MergedDictionaries
            CollectThemeDictionaries(mergedDict, result);
        }
    }

    /// <summary>
    /// 从嵌套的 MergedDictionaries 中移除指定的资源字典
    /// </summary>
    private static void RemoveFromNestedDictionaries(System.Collections.ObjectModel.Collection<ResourceDictionary> collection, ResourceDictionary target)
    {
        foreach (var dict in collection)
        {
            if (dict.MergedDictionaries != null && dict.MergedDictionaries.Contains(target))
            {
                dict.MergedDictionaries.Remove(target);
                return;
            }
            
            // 递归检查嵌套的 MergedDictionaries
            if (dict.MergedDictionaries != null)
            {
                RemoveFromNestedDictionaries(dict.MergedDictionaries, target);
            }
        }
    }

    /// <summary>
    /// 获取系统主题
    /// </summary>
    private static Theme GetSystemTheme()
    {
        // 高对比度模式优先使用深色主题
        if (SystemParameters.HighContrast)
        {
            return Theme.Dark;
        }

        // 检查系统主题名称
        var uxThemeName = SystemParameters.UxThemeName;
        if (uxThemeName != null && uxThemeName.Contains("Light", StringComparison.OrdinalIgnoreCase))
        {
            return Theme.Light;
        }

        // 默认返回深色主题
        return Theme.Dark;
    }

    /// <summary>
    /// 初始化主题系统
    /// 应在 Application 启动时调用
    /// </summary>
    public static void Initialize()
    {
        ApplyTheme(_currentTheme);
    }

    /// <summary>
    /// 检查主题资源字典是否存在
    /// </summary>
    /// <param name="theme">要检查的主题</param>
    /// <returns>如果主题资源字典存在返回 true，否则返回 false</returns>
    public static bool IsThemeAvailable(Theme theme)
    {
        var themeUri = theme switch
        {
            Theme.Base => BaseThemeUri,
            Theme.Dark => DarkThemeUri,
            Theme.Light => LightThemeUri,
            Theme.NeonCyberpunk => NeonCyberpunkThemeUri,
            Theme.Aurora => AuroraThemeUri,
            Theme.SunsetTropics => SunsetTropicsThemeUri,
            Theme.RoseQuartz => RoseQuartzThemeUri,
            Theme.DeepOcean => DeepOceanThemeUri,
            Theme.Sakura => SakuraThemeUri,
            Theme.MidnightVelvet => MidnightVelvetThemeUri,
            Theme.ForestWhisper => ForestWhisperThemeUri,
            _ => BaseThemeUri
        };

        try
        {
            // 尝试创建 ResourceDictionary 来验证资源是否存在
            var testDict = new ResourceDictionary { Source = themeUri };
            return testDict.Source != null;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 异步刷新所有窗口的资源引用，确保主题变化立即生效
    /// 优化：使用低优先级异步刷新，避免阻塞UI线程，只刷新可见窗口
    /// </summary>
    private static void RefreshAllWindowsAsync()
    {
        var app = Application.Current;
        if (app == null) return;

        // 使用低优先级异步刷新，避免阻塞UI线程
        app.Dispatcher.BeginInvoke(
            DispatcherPriority.Loaded,
            new Action(() =>
            {
                RefreshVisibleWindows();
            }));
    }

    /// <summary>
    /// 刷新可见窗口的资源引用
    /// 优化：只刷新可见的窗口，提升性能
    /// </summary>
    private static void RefreshVisibleWindows()
    {
        var app = Application.Current;
        if (app == null) return;

        // 只刷新可见的窗口，提升性能
        foreach (Window window in app.Windows.OfType<Window>().Where(w => w.IsVisible))
        {
            if (window is FrameworkElement fe)
            {
                RefreshElementResources(fe);
            }
        }
    }

    /// <summary>
    /// 刷新元素资源引用
    /// 优化：使用更高效的方式触发资源刷新
    /// </summary>
    private static void RefreshElementResources(FrameworkElement element)
    {
        var resources = element.Resources;
        if (resources != null && resources.MergedDictionaries.Count > 0)
        {
            // 优化：临时移除并重新添加第一个字典，触发刷新
            // 这种方式比创建临时字典更高效
            var first = resources.MergedDictionaries[0];
            resources.MergedDictionaries.RemoveAt(0);
            resources.MergedDictionaries.Insert(0, first);
        }
        
        // 递归刷新子元素（仅对可见元素）
        if (element is Panel panel)
        {
            foreach (var child in panel.Children.OfType<FrameworkElement>())
            {
                RefreshElementResources(child);
            }
        }
    }
}

/// <summary>
/// 主题变化事件参数
/// </summary>
public class ThemeChangedEventArgs : EventArgs
{
    /// <summary>
    /// 旧主题
    /// </summary>
    public ThemeManager.Theme OldTheme { get; }

    /// <summary>
    /// 新主题
    /// </summary>
    public ThemeManager.Theme NewTheme { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="oldTheme">旧主题</param>
    /// <param name="newTheme">新主题</param>
    public ThemeChangedEventArgs(ThemeManager.Theme oldTheme, ThemeManager.Theme newTheme)
    {
        OldTheme = oldTheme;
        NewTheme = newTheme;
    }
}
