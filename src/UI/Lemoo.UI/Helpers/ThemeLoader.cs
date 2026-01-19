using System;
using System.Collections.Generic;
using System.Windows;
using System.Diagnostics;

namespace Lemoo.UI.Helpers;

/// <summary>
/// 主题资源加载器 - 统一管理资源加载顺序
/// 解决手动维护加载顺序的问题，通过配置化方式定义资源层次
/// </summary>
public static class ThemeLoader
{
    /// <summary>
    /// 资源层类型 - 定义资源的优先级和用途
    /// </summary>
    public enum ResourceLayer
    {
        /// <summary>
        /// 设计基线 - 颜色无关的通用设计规则（字体、间距、阴影、动画）
        /// </summary>
        DesignBaseline,

        /// <summary>
        /// 基础主题 - 默认配色方案（色板、语义令牌、组件画刷）
        /// </summary>
        BaseTheme,

        /// <summary>
        /// 主题覆盖 - 特定主题的配色覆盖
        /// </summary>
        ThemeOverride,

        /// <summary>
        /// 控件样式 - 使用主题颜色的控件样式定义
        /// </summary>
        ControlStyles
    }

    /// <summary>
    /// 资源定义 - 描述单个资源文件
    /// </summary>
    public class ResourceDefinition
    {
        /// <summary>
        /// 资源层类型
        /// </summary>
        public ResourceLayer Layer { get; set; }

        /// <summary>
        /// 资源 URI
        /// </summary>
        public Uri Uri { get; set; } = null!;

        /// <summary>
        /// 资源描述（用于调试和文档）
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 是否为基础主题资源（不随主题变化）
        /// </summary>
        public bool IsBaseResource { get; set; }

        public ResourceDefinition(ResourceLayer layer, Uri uri, string description, bool isBaseResource = false)
        {
            Layer = layer;
            Uri = uri;
            Description = description;
            IsBaseResource = isBaseResource;
        }
    }

    /// <summary>
    /// 主题资源配置 - 定义一个主题需要的所有资源
    /// </summary>
    public class ThemeResources
    {
        /// <summary>
        /// 主题名称
        /// </summary>
        public string ThemeName { get; set; } = string.Empty;

        /// <summary>
        /// 资源定义列表（按加载顺序）
        /// </summary>
        public List<ResourceDefinition> Resources { get; set; } = new();

        /// <summary>
        /// 主题描述
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }

    #region 资源 URI 常量

    // 设计基线资源
    private const string DesignBaselineBase = "pack://application:,,,/Lemoo.UI;component/Styles/Design";

    private static readonly string[] DesignBaselineResources = new[]
    {
        $"{DesignBaselineBase}/Typography.xaml",
        $"{DesignBaselineBase}/Spacing.xaml",
        $"{DesignBaselineBase}/Shadows.xaml",
        $"{DesignBaselineBase}/Animations.xaml"
    };

    // 基础主题资源
    private const string BaseThemeBase = "pack://application:,,,/Lemoo.UI;component/Themes/Base";

    private static readonly string[] BaseThemeResources = new[]
    {
        $"{BaseThemeBase}/ColorPalette.xaml",
        $"{BaseThemeBase}/SemanticTokens.xaml",
        $"{BaseThemeBase}/ComponentBrushes.xaml"
    };

    // Win11 控件样式资源
    private const string ControlStylesBase = "pack://application:,,,/Lemoo.UI;component/Styles/Win11";
    private const string Win11ControlsUri = $"{ControlStylesBase}/Win11.Controls.xaml";

    #endregion

    /// <summary>
    /// 获取主题资源配置
    /// </summary>
    /// <param name="themeName">主题名称（如 "Base", "Dark", "Light"）</param>
    /// <param name="isBaseTheme">是否为基础主题（Base 主题不覆盖自身）</param>
    /// <returns>主题资源配置</returns>
    public static ThemeResources GetThemeResources(string themeName, bool isBaseTheme = false)
    {
        var resources = new ThemeResources
        {
            ThemeName = themeName,
            Description = isBaseTheme ? "基础主题（默认配色）" : $"{themeName} 主题"
        };

        // 1. 设计基线（颜色无关的通用设计规则）
        AddDesignBaselineResources(resources);

        // 2. 基础主题（基础色板/语义色/组件基准样式）
        AddBaseThemeResources(resources);

        // 3. 当前主题覆盖（如果不是 Base 主题）
        if (!isBaseTheme)
        {
            AddThemeOverrideResources(resources, themeName);
        }

        // 4. Win11 控件样式（使用主题后的颜色）
        AddControlStylesResources(resources);

        return resources;
    }

    /// <summary>
    /// 添加设计基线资源
    /// </summary>
    private static void AddDesignBaselineResources(ThemeResources resources)
    {
        var descriptions = new Dictionary<string, string>
        {
            ["Typography.xaml"] = "字体排版定义（字体家族、字号、行高等）",
            ["Spacing.xaml"] = "间距系统定义（4px 网格、Padding、Margin）",
            ["Shadows.xaml"] = "阴影效果定义（阴影深度、模糊半径）",
            ["Animations.xaml"] = "动画定义（持续时间、缓动函数）"
        };

        foreach (var resourceUri in DesignBaselineResources)
        {
            var fileName = System.IO.Path.GetFileName(resourceUri);
            resources.Resources.Add(new ResourceDefinition(
                ResourceLayer.DesignBaseline,
                new Uri(resourceUri, UriKind.Absolute),
                $"设计基线 - {descriptions.GetValueOrDefault(fileName, fileName)}",
                true
            ));
        }

        Debug.WriteLine($"ThemeLoader: 添加设计基线资源 ({DesignBaselineResources.Length} 个文件)");
    }

    /// <summary>
    /// 添加基础主题资源
    /// </summary>
    private static void AddBaseThemeResources(ThemeResources resources)
    {
        var descriptions = new Dictionary<string, string>
        {
            ["ColorPalette.xaml"] = "基础色板定义（Palette.* 颜色）",
            ["SemanticTokens.xaml"] = "语义令牌定义（workbench.*, button.* 等）",
            ["ComponentBrushes.xaml"] = "组件画刷定义（Color → Brush 映射）"
        };

        foreach (var resourceUri in BaseThemeResources)
        {
            var fileName = System.IO.Path.GetFileName(resourceUri);
            resources.Resources.Add(new ResourceDefinition(
                ResourceLayer.BaseTheme,
                new Uri(resourceUri, UriKind.Absolute),
                $"基础主题 - {descriptions.GetValueOrDefault(fileName, fileName)}",
                true
            ));
        }

        Debug.WriteLine($"ThemeLoader: 添加基础主题资源 ({BaseThemeResources.Length} 个文件)");
    }

    /// <summary>
    /// 添加主题覆盖资源
    /// </summary>
    private static void AddThemeOverrideResources(ThemeResources resources, string themeName)
    {
        var themeBase = $"pack://application:,,,/Lemoo.UI;component/Themes/{themeName}";

        var overrideResources = new[]
        {
            $"{themeBase}/ColorPalette.xaml",
            $"{themeBase}/SemanticTokens.xaml",
            $"{themeBase}/ComponentBrushes.xaml"
        };

        var descriptions = new Dictionary<string, string>
        {
            ["ColorPalette.xaml"] = "主题色板覆盖（覆盖 Base 的色板）",
            ["SemanticTokens.xaml"] = "主题语义令牌覆盖（覆盖 Base 的语义令牌）",
            ["ComponentBrushes.xaml"] = "主题组件画刷覆盖（覆盖 Base 的画刷）"
        };

        foreach (var resourceUri in overrideResources)
        {
            var fileName = System.IO.Path.GetFileName(resourceUri);
            resources.Resources.Add(new ResourceDefinition(
                ResourceLayer.ThemeOverride,
                new Uri(resourceUri, UriKind.Absolute),
                $"{themeName} 主题 - {descriptions.GetValueOrDefault(fileName, fileName)}",
                false
            ));
        }

        Debug.WriteLine($"ThemeLoader: 添加 {themeName} 主题覆盖资源 ({overrideResources.Length} 个文件)");
    }

    /// <summary>
    /// 添加控件样式资源
    /// </summary>
    private static void AddControlStylesResources(ThemeResources resources)
    {
        resources.Resources.Add(new ResourceDefinition(
            ResourceLayer.ControlStyles,
            new Uri(Win11ControlsUri, UriKind.Absolute),
            "Win11 控件样式（包含所有控件的样式定义）",
            true
        ));

        Debug.WriteLine($"ThemeLoader: 添加 Win11 控件样式资源");
    }

    /// <summary>
    /// 创建主题资源字典
    /// </summary>
    /// <param name="themeName">主题名称</param>
    /// <param name="isBaseTheme">是否为基础主题</param>
    /// <returns>完整的主题资源字典</returns>
    public static ResourceDictionary CreateThemeDictionary(string themeName, bool isBaseTheme = false)
    {
        var config = GetThemeResources(themeName, isBaseTheme);
        var themeDict = new ResourceDictionary();

        Debug.WriteLine($"ThemeLoader: 开始创建 {themeName} 主题资源字典");
        Debug.WriteLine($"  资源文件数量: {config.Resources.Count}");

        // 按顺序加载所有资源
        foreach (var resource in config.Resources)
        {
            try
            {
                var resourceDict = new ResourceDictionary { Source = resource.Uri };
                themeDict.MergedDictionaries.Add(resourceDict);
                Debug.WriteLine($"  ✓ [{resource.Layer}] {resource.Description}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"  ✗ [{resource.Layer}] 加载失败: {resource.Description}");
                Debug.WriteLine($"    错误: {ex.Message}");

                // 对于主题覆盖资源，如果文件不存在，可以选择跳过或抛出异常
                if (resource.Layer == ResourceLayer.ThemeOverride)
                {
                    Debug.WriteLine($"    警告: 主题覆盖资源 {resource.Uri} 加载失败，将使用基础主题配色");
                    // 继续加载，不中断
                }
                else
                {
                    // 对于关键资源（设计基线、基础主题、控件样式），抛出异常
                    throw new InvalidOperationException(
                        $"加载关键资源失败: {resource.Description}。主题无法正常工作。", ex);
                }
            }
        }

        Debug.WriteLine($"ThemeLoader: {themeName} 主题资源字典创建完成");
        return themeDict;
    }

    /// <summary>
    /// 验证资源配置是否有效
    /// </summary>
    /// <param name="config">主题资源配置</param>
    /// <returns>验证结果</returns>
    public static (bool IsValid, List<string> Errors) ValidateThemeResources(ThemeResources config)
    {
        var errors = new List<string>();

        // 检查是否有资源
        if (config.Resources.Count == 0)
        {
            errors.Add("主题没有配置任何资源");
            return (false, errors);
        }

        // 检查资源层次顺序（必须是：DesignBaseline → BaseTheme → ThemeOverride → ControlStyles）
        ResourceLayer? previousLayer = null;
        foreach (var resource in config.Resources)
        {
            if (previousLayer.HasValue && resource.Layer < previousLayer.Value)
            {
                errors.Add($"资源层次顺序错误: {resource.Description} (Layer: {resource.Layer}) " +
                          $"应该在 {previousLayer.Value} 之前");
            }
            previousLayer = resource.Layer;
        }

        // 检查必须的资源层
        var requiredLayers = new[] { ResourceLayer.DesignBaseline, ResourceLayer.BaseTheme, ResourceLayer.ControlStyles };
        var existingLayers = new HashSet<ResourceLayer>(config.Resources.Select(r => r.Layer));

        foreach (var requiredLayer in requiredLayers)
        {
            if (!existingLayers.Contains(requiredLayer))
            {
                errors.Add($"缺少必需的资源层: {requiredLayer}");
            }
        }

        return (errors.Count == 0, errors);
    }

    /// <summary>
    /// 获取资源层次描述（用于调试和文档）
    /// </summary>
    /// <returns>资源层次结构的文本描述</returns>
    public static string GetResourceLayerDescription()
    {
        return @"
========================================
主题资源层次结构（按加载顺序）
========================================

1. 设计基线 (DesignBaseline)
   - Typography.xaml     字体排版定义
   - Spacing.xaml        间距系统定义
   - Shadows.xaml        阴影效果定义
   - Animations.xaml     动画定义

   特点：颜色无关，所有主题共享

2. 基础主题 (BaseTheme)
   - ColorPalette.xaml   基础色板定义
   - SemanticTokens.xaml 语义令牌定义
   - ComponentBrushes.xaml 组件画刷定义

   特点：提供默认配色方案

3. 主题覆盖 (ThemeOverride) [可选]
   - ColorPalette.xaml   主题色板覆盖
   - SemanticTokens.xaml 主题语义令牌覆盖
   - ComponentBrushes.xaml 主题组件画刷覆盖

   特点：覆盖 Base 的配色，实现主题差异化

4. 控件样式 (ControlStyles)
   - Win11.Controls.xaml Win11 控件样式合并入口

   特点：使用主题后的颜色定义控件外观

========================================
加载顺序的重要性
========================================

后加载的资源会覆盖先加载的同名资源。
因此：
- 设计基线最先加载（被主题颜色覆盖）
- 基础主题次之（被主题覆盖覆盖）
- 主题覆盖再次（最终生效的颜色）
- 控件样式最后（使用最终颜色）

========================================";
    }

    /// <summary>
    /// 获取所有可用主题列表
    /// </summary>
    /// <returns>主题名称列表</returns>
    public static readonly string[] AvailableThemes = new[]
    {
        "Base",
        "Dark",
        "Light",
        "NeonCyberpunk",
        "Aurora",
        "SunsetTropics",
        "RoseQuartz",
        "DeepOcean",
        "Sakura",
        "MidnightVelvet",
        "ForestWhisper"
    };

    /// <summary>
    /// 检查主题是否可用
    /// </summary>
    /// <param name="themeName">主题名称</param>
    /// <returns>如果主题可用返回 true</returns>
    public static bool IsThemeAvailable(string themeName)
    {
        return Array.Exists(AvailableThemes, t => t.Equals(themeName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 获取主题资源统计信息
    /// </summary>
    /// <param name="themeName">主题名称</param>
    /// <param name="isBaseTheme">是否为基础主题</param>
    /// <returns>统计信息</returns>
    public static string GetThemeStatistics(string themeName, bool isBaseTheme = false)
    {
        var config = GetThemeResources(themeName, isBaseTheme);

        var stats = $@"
========================================
主题资源统计: {themeName}
========================================

资源文件总数: {config.Resources.Count}

按层次分类：
";

        var grouped = config.Resources.GroupBy(r => r.Layer);
        foreach (var group in grouped.OrderBy(g => g.Key))
        {
            stats += $"\n{group.Key} ({group.Count()} 个文件):\n";
            foreach (var resource in group)
            {
                stats += $"  - {resource.Description}\n";
            }
        }

        stats += "\n========================================\n";

        return stats;
    }
}
