using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.IO;
using System.Xml.Linq;

namespace Lemoo.UI.Helpers;

/// <summary>
/// 主题一致性验证工具
/// 用于检查主题的完整性和一致性，确保所有主题包含必需的语义令牌
/// </summary>
public static class ThemeValidator
{
    /// <summary>
    /// 必需的语义令牌列表（按功能分组）
    /// </summary>
    private static readonly Dictionary<string, string[]> RequiredSemanticTokens = new()
    {
        ["工作台"] = new[]
        {
            "workbench.background", "workbench.surface", "workbench.card"
        },
        ["标题栏"] = new[]
        {
            "titleBar.background", "titleBar.foreground", "titleBar.activeBackground",
            "titleBar.inactiveBackground", "titleBar.border"
        },
        ["侧边栏"] = new[]
        {
            "sidebar.background", "sidebar.foreground", "sidebar.hoverBackground",
            "sidebar.selectedBackground", "sidebar.selectedForeground", "sidebar.border"
        },
        ["菜单"] = new[]
        {
            "menu.background", "menu.foreground", "menu.border",
            "menu.hoverBackground", "menu.selectedBackground",
            "menu.selectedForeground", "menu.separator"
        },
        ["菜单项"] = new[]
        {
            "menuItem.background", "menuItem.foreground", "menuItem.hoverBackground",
            "menuItem.selectedBackground", "menuItem.selectedForeground",
            "menuItem.highlightedBackground"
        },
        ["按钮"] = new[]
        {
            "button.background", "button.foreground", "button.hoverBackground",
            "button.pressedBackground", "button.disabledBackground",
            "button.disabledForeground", "button.border"
        },
        ["标签页"] = new[]
        {
            "tab.background", "tab.activeBackground", "tab.foreground",
            "tab.activeForeground", "tab.hoverBackground", "tab.selectedBackground",
            "tab.selectedForeground", "tab.border"
        },
        ["输入框"] = new[]
        {
            "input.background", "input.foreground", "input.border",
            "input.focusBorder", "input.placeholder"
        },
        ["分隔符"] = new[] { "separator.background" },
        ["滚动条"] = new[]
        {
            "scrollbar.shadow", "scrollbar.sliderBackground",
            "scrollbar.sliderHoverBackground"
        },
        ["状态栏"] = new[]
        {
            "statusBar.background", "statusBar.foreground"
        },
        ["通知"] = new[]
        {
            "notification.background", "notification.border", "notification.foreground"
        },
        ["下拉菜单"] = new[]
        {
            "dropdown.background", "dropdown.foreground",
            "dropdown.border", "dropdown.hoverBackground"
        },
        ["卡片"] = new[]
        {
            "card.background", "card.border", "card.hoverBackground"
        },
        ["列表项"] = new[]
        {
            "listItem.background", "listItem.hoverBackground",
            "listItem.selectedBackground"
        },
        ["弹出层"] = new[]
        {
            "popup.background", "popup.border"
        },
        ["开关"] = new[]
        {
            "toggleSwitch.offBackground", "toggleSwitch.offBorder",
            "toggleSwitch.onBackground", "toggleSwitch.onBorder",
            "toggleSwitch.knob", "toggleSwitch.knobOn",
            "toggleSwitch.hoverBorder", "toggleSwitch.pressedBorder"
        },
        ["NumericUpDown"] = new[]
        {
            "numericUpDown.buttonBackground",
            "numericUpDown.buttonHoverBackground",
            "numericUpDown.buttonPressedBackground"
        },
        ["文本"] = new[]
        {
            "text.primary", "text.secondary", "text.disabled", "text.onDark"
        },
        ["边框"] = new[]
        {
            "border.default", "border.focus", "separator"
        },
        ["阴影"] = new[] { "shadow.color" },
        ["语义色"] = new[]
        {
            "semantic.success.background", "semantic.success.foreground",
            "semantic.success.border",
            "semantic.warning.background", "semantic.warning.foreground",
            "semantic.warning.border",
            "semantic.error.background", "semantic.error.foreground",
            "semantic.error.border",
            "semantic.info.background", "semantic.info.foreground",
            "semantic.info.border"
        }
    };

    /// <summary>
    /// 所有主题名称列表
    /// </summary>
    private static readonly string[] AllThemes = new[]
    {
        "Base", "Dark", "Light", "NeonCyberpunk", "Aurora", "SunsetTropics",
        "RoseQuartz", "DeepOcean", "Sakura", "MidnightVelvet", "ForestWhisper"
    };

    /// <summary>
    /// 验证结果
    /// </summary>
    public class ValidationResult
    {
        public bool IsSuccess { get; set; }
        public Dictionary<string, List<string>> MissingTokensByTheme { get; set; } = new();
        public Dictionary<string, List<string>> ExtraTokensByTheme { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public List<string> Errors { get; set; } = new();

        public string GetSummary()
        {
            var summary = new System.Text.StringBuilder();
            summary.AppendLine("=== 主题验证结果 ===");
            summary.AppendLine($"状态: {(IsSuccess ? "✓ 通过" : "✗ 失败")}");

            if (Errors.Count > 0)
            {
                summary.AppendLine($"\n错误 ({Errors.Count}):");
                foreach (var error in Errors)
                {
                    summary.AppendLine($"  ✗ {error}");
                }
            }

            if (MissingTokensByTheme.Count > 0)
            {
                summary.AppendLine($"\n缺失的语义令牌 ({MissingTokensByTheme.Count} 个主题):");
                foreach (var (theme, tokens) in MissingTokensByTheme)
                {
                    summary.AppendLine($"  {theme} ({tokens.Count} 个缺失):");
                    foreach (var token in tokens.Take(10))
                    {
                        summary.AppendLine($"    - {token}");
                    }
                    if (tokens.Count > 10)
                    {
                        summary.AppendLine($"    ... 还有 {tokens.Count - 10} 个");
                    }
                }
            }

            if (Warnings.Count > 0)
            {
                summary.AppendLine($"\n警告 ({Warnings.Count}):");
                foreach (var warning in Warnings)
                {
                    summary.AppendLine($"  ⚠ {warning}");
                }
            }

            if (IsSuccess)
            {
                summary.AppendLine("\n所有主题均包含完整的语义令牌！");
            }

            summary.AppendLine("\n=== 验证完成 ===");
            return summary.ToString();
        }
    }

    /// <summary>
    /// 验证所有主题的完整性
    /// </summary>
    public static ValidationResult ValidateAllThemes()
    {
        var result = new ValidationResult();

        // 获取所有必需的令牌
        var allRequiredTokens = RequiredSemanticTokens
            .SelectMany(kvp => kvp.Value)
            .Distinct()
            .OrderBy(t => t)
            .ToList();

        foreach (var theme in AllThemes)
        {
            try
            {
                var themeTokens = LoadThemeTokens(theme);
                var missingTokens = allRequiredTokens.Where(t => !themeTokens.Contains(t)).ToList();

                if (missingTokens.Count > 0)
                {
                    result.MissingTokensByTheme[theme] = missingTokens;
                    result.Errors.Add($"{theme} 主题缺失 {missingTokens.Count} 个语义令牌");
                }

                // 检查是否有额外的令牌（可能是拼写错误）
                var extraTokens = themeTokens
                    .Where(t => !allRequiredTokens.Contains(t) && !t.StartsWith("Palette."))
                    .ToList();

                if (extraTokens.Count > 0)
                {
                    result.ExtraTokensByTheme[theme] = extraTokens;
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"无法加载 {theme} 主题: {ex.Message}");
            }
        }

        // 检查一致性
        if (result.MissingTokensByTheme.Count > 0)
        {
            result.Warnings.Add("部分主题缺失语义令牌，建议使用 SyncThemeTokens() 方法同步");
        }

        // 检查是否所有主题的令牌数量一致
        var tokenCounts = new Dictionary<string, int>();
        foreach (var theme in AllThemes)
        {
            try
            {
                var tokens = LoadThemeTokens(theme);
                tokenCounts[theme] = tokens.Count;
            }
            catch { }
        }

        if (tokenCounts.Values.Distinct().Count() > 1)
        {
            result.Warnings.Add("不同主题的语义令牌数量不一致，可能导致主题切换时某些控件颜色异常");
        }

        result.IsSuccess = result.MissingTokensByTheme.Count == 0 && result.Errors.Count == 0;
        return result;
    }

    /// <summary>
    /// 加载主题的所有令牌名称
    /// </summary>
    private static HashSet<string> LoadThemeTokens(string themeName)
    {
        var tokens = new HashSet<string>();

        try
        {
            // 尝试从应用程序资源加载
            var uri = new Uri($"pack://application:,,,/Lemoo.UI;component/Themes/{themeName}/{themeName}.xaml");
            var dict = new ResourceDictionary { Source = uri };

            // 收集所有颜色和画刷的键名
            foreach (var key in dict.Keys)
            {
                if (key is string stringKey)
                {
                    var value = dict[stringKey];
                    if (value is Color || value is SolidColorBrush)
                    {
                        tokens.Add(stringKey);
                    }
                }
            }
        }
        catch
        {
            // 如果无法从资源加载，尝试从文件系统加载
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var xamlPath = Path.Combine(basePath, "..", "..", "..", "..", "src", "UI", "Lemoo.UI", "Themes", themeName, "SemanticTokens.xaml");

            if (File.Exists(xamlPath))
            {
                var xaml = XDocument.Load(xamlPath);
                foreach (var element in xaml.Descendants())
                {
                    if (element.Name.LocalName == "Color")
                    {
                        var keyAttribute = element.Attribute("Key");
                        if (keyAttribute != null)
                        {
                            tokens.Add(keyAttribute.Value!);
                        }
                    }
                }
            }
        }

        return tokens;
    }

    /// <summary>
    /// 生成缺失令牌的补丁文件
    /// </summary>
    public static string GenerateMissingTokensPatch(string themeName, List<string> missingTokens)
    {
        if (missingTokens.Count == 0)
            return string.Empty;

        var patch = new System.Text.StringBuilder();
        patch.AppendLine($"<!-- ==========================================");
        patch.AppendLine($"     缺失的语义令牌 - 需要添加到 {themeName}/SemanticTokens.xaml");
        patch.AppendLine($"     ============================================ -->");
        patch.AppendLine();

        // 按功能分组
        var groupedTokens = missingTokens
            .Select(t => new { Token = t, Category = GetTokenCategory(t) })
            .GroupBy(t => t.Category)
            .OrderBy(g => g.Key);

        foreach (var group in groupedTokens)
        {
            patch.AppendLine($"    <!-- {group.Key} -->");
            foreach (var item in group.OrderBy(t => t.Token))
            {
                // 生成默认颜色值（灰色）
                patch.AppendLine($"    <Color x:Key=\"{item.Token}\">#CCCCCC</Color> <!-- TODO: 设置合适的颜色值 -->");
            }
            patch.AppendLine();
        }

        return patch.ToString();
    }

    /// <summary>
    /// 获取令牌所属的功能分类
    /// </summary>
    private static string GetTokenCategory(string token)
    {
        foreach (var (category, tokens) in RequiredSemanticTokens)
        {
            if (tokens.Contains(token))
                return category;
        }
        return "其他";
    }

    /// <summary>
    /// 对比两个主题的令牌差异
    /// </summary>
    public static void CompareThemes(string theme1, string theme2)
    {
        var tokens1 = LoadThemeTokens(theme1);
        var tokens2 = LoadThemeTokens(theme2);

        var onlyIn1 = tokens1.Except(tokens2).OrderBy(t => t).ToList();
        var onlyIn2 = tokens2.Except(tokens1).OrderBy(t => t).ToList();
        var common = tokens1.Intersect(tokens2).OrderBy(t => t).ToList();

        var result = new System.Text.StringBuilder();
        result.AppendLine($"=== 主题对比: {theme1} vs {theme2} ===");
        result.AppendLine($"仅在 {theme1} 中存在 ({onlyIn1.Count} 个):");
        foreach (var token in onlyIn1.Take(20))
        {
            result.AppendLine($"  - {token}");
        }
        if (onlyIn1.Count > 20) result.AppendLine($"  ... 还有 {onlyIn1.Count - 20} 个");

        result.AppendLine($"\n仅在 {theme2} 中存在 ({onlyIn2.Count} 个):");
        foreach (var token in onlyIn2.Take(20))
        {
            result.AppendLine($"  - {token}");
        }
        if (onlyIn2.Count > 20) result.AppendLine($"  ... 还有 {onlyIn2.Count - 20} 个");

        result.AppendLine($"\n共同令牌 ({common.Count} 个)");

        System.Diagnostics.Debug.WriteLine(result.ToString());
    }

    /// <summary>
    /// 快速验证 - 只检查关键令牌
    /// </summary>
    public static bool QuickValidate()
    {
        var criticalTokens = new[]
        {
            "workbench.background", "text.primary", "text.secondary",
            "button.background", "button.foreground",
            "input.background", "input.foreground", "input.border",
            "card.background", "card.border",
            "border.default", "border.focus"
        };

        foreach (var theme in AllThemes)
        {
            try
            {
                var tokens = LoadThemeTokens(theme);
                var missing = criticalTokens.Where(t => !tokens.Contains(t)).ToList();

                if (missing.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"主题 {theme} 缺失关键令牌: {string.Join(", ", missing)}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"无法验证主题 {theme}: {ex.Message}");
                return false;
            }
        }

        return true;
    }
}
