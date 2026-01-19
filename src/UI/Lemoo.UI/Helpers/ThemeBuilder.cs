using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Lemoo.UI.Helpers;

/// <summary>
/// 主题开发辅助工具
/// 用于快速创建新主题、同步语义令牌、生成主题模板
/// </summary>
public static class ThemeBuilder
{
    /// <summary>
    /// 从源主题复制语义令牌到目标主题
    /// </summary>
    public static void CopySemanticTokens(string sourceTheme, string targetTheme)
    {
        var sourceUri = new Uri($"pack://application:,,,/Lemoo.UI;component/Themes/{sourceTheme}/{sourceTheme}.xaml");
        var sourceDict = new ResourceDictionary { Source = sourceUri };

        var tokens = new Dictionary<string, Color>();

        // 收集所有颜色令牌
        foreach (var key in sourceDict.Keys)
        {
            if (key is string stringKey && sourceDict[stringKey] is Color color)
            {
                // 只复制语义令牌，不复制 Palette 调色板
                if (!stringKey.StartsWith("Palette."))
                {
                    tokens[stringKey] = color;
                }
            }
        }

        System.Diagnostics.Debug.WriteLine($"从 {sourceTheme} 复制了 {tokens.Count} 个语义令牌");
        return;
    }

    /// <summary>
    /// 生成新主题模板文件
    /// </summary>
    public static string GenerateThemeTemplate(string themeName, string baseTheme = "Base")
    {
        var template = new StringBuilder();

        // SemanticTokens.xaml 模板
        template.AppendLine("<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");
        template.AppendLine("                    xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">");
        template.AppendLine();
        template.AppendLine($"    <!-- ============================================");
        template.AppendLine($"         {themeName} 主题");
        template.AppendLine($"         基于 {baseTheme} 主题模板生成");
        template.AppendLine($"         TODO: 调整颜色值以匹配主题风格");
        template.AppendLine($"         ============================================ -->");
        template.AppendLine();

        // 从基础主题获取所有令牌
        var baseUri = new Uri($"pack://application:,,,/Lemoo.UI;component/Themes/{baseTheme}/SemanticTokens.xaml");
        var baseDict = new ResourceDictionary { Source = baseUri };

        var tokens = new List<(string category, string key, Color color)>();
        string currentCategory = string.Empty;

        // 收集令牌并按分类排序
        foreach (var key in baseDict.Keys)
        {
            if (key is string stringKey && baseDict[stringKey] is Color color && !stringKey.StartsWith("Palette."))
            {
                var category = GetTokenCategory(stringKey);
                tokens.Add((category, stringKey, color));
            }
        }

        // 按分类生成
        var lastCategory = string.Empty;
        foreach (var (category, key, color) in tokens)
        {
            if (category != lastCategory)
            {
                template.AppendLine($"    <!-- {category} -->");
                lastCategory = category;
            }
            template.AppendLine($"    <Color x:Key=\"{key}\">#{color.R:X2}{color.G:X2}{color.B:X2}</Color> <!-- TODO: 调整颜色 -->");
        }

        template.AppendLine();
        template.AppendLine("</ResourceDictionary>");

        return template.ToString();
    }

    /// <summary>
    /// 生成主题对比报告
    /// </summary>
    public static string CompareThemes(List<string> themes)
    {
        var report = new StringBuilder();
        report.AppendLine("=== 主题对比报告 ===");
        report.AppendLine();

        var allTokens = new Dictionary<string, Dictionary<string, Color>>();

        // 加载所有主题的令牌
        foreach (var theme in themes)
        {
            try
            {
                var uri = new Uri($"pack://application:,,,/Lemoo.UI;component/Themes/{theme}/{theme}.xaml");
                var dict = new ResourceDictionary { Source = uri };

                allTokens[theme] = new Dictionary<string, Color>();
                foreach (var key in dict.Keys)
                {
                    if (key is string stringKey && dict[stringKey] is Color color && !stringKey.StartsWith("Palette."))
                    {
                        allTokens[theme][stringKey] = color;
                    }
                }
            }
            catch (Exception ex)
            {
                report.AppendLine($"警告: 无法加载主题 {theme}: {ex.Message}");
            }
        }

        // 找出所有唯一的令牌
        var uniqueTokens = new HashSet<string>();
        foreach (var themeTokens in allTokens.Values)
        {
            foreach (var token in themeTokens.Keys)
            {
                uniqueTokens.Add(token);
            }
        }

        // 检查每个令牌在所有主题中是否存在
        report.AppendLine("缺失令牌检查:");
        foreach (var theme in themes)
        {
            if (!allTokens.ContainsKey(theme)) continue;

            var missing = new List<string>();
            foreach (var token in uniqueTokens)
            {
                if (!allTokens[theme].ContainsKey(token))
                {
                    missing.Add(token);
                }
            }

            if (missing.Count > 0)
            {
                report.AppendLine($"  {theme}: 缺失 {missing.Count} 个令牌");
                foreach (var token in missing.Take(5))
                {
                    report.AppendLine($"    - {token}");
                }
                if (missing.Count > 5)
                {
                    report.AppendLine($"    ... 还有 {missing.Count - 5} 个");
                }
            }
            else
            {
                report.AppendLine($"  {theme}: ✓ 完整");
            }
        }

        report.AppendLine();
        report.AppendLine("=== 对比完成 ===");

        return report.ToString();
    }

    /// <summary>
    /// 导出主题为 JSON 格式（用于主题编辑器）
    /// </summary>
    public static string ExportThemeToJson(string themeName)
    {
        var json = new StringBuilder();
        json.AppendLine("{");
        json.AppendLine($"  \"themeName\": \"{themeName}\",");

        var uri = new Uri($"pack://application:,,,/Lemoo.UI;component/Themes/{themeName}/{themeName}.xaml");
        var dict = new ResourceDictionary { Source = uri };

        json.AppendLine("  \"semanticTokens\": {");

        var tokens = new List<string>();
        foreach (var key in dict.Keys)
        {
            if (key is string stringKey && dict[stringKey] is Color color && !stringKey.StartsWith("Palette."))
            {
                tokens.Add($"    \"{stringKey}\": \"#{color.R:X2}{color.G:X2}{color.B:X2}\"");
            }
        }

        json.AppendLine(string.Join(",\n", tokens));
        json.AppendLine("  }");
        json.AppendLine("}");

        return json.ToString();
    }

    /// <summary>
    /// 生成主题预览色板
    /// </summary>
    public static string GenerateColorPalette(string themeName)
    {
        var palette = new StringBuilder();
        palette.AppendLine($"=== {themeName} 色板 ===");
        palette.AppendLine();

        var uri = new Uri($"pack://application:,,,/Lemoo.UI;component/Themes/{themeName}/ColorPalette.xaml");
        var dict = new ResourceDictionary { Source = uri };

        var colors = new List<string>();
        foreach (var key in dict.Keys)
        {
            if (key is string stringKey && dict[stringKey] is Color color)
            {
                colors.Add($"{stringKey}: #{color.R:X2}{color.G:X2}{color.B:X2}");
            }
        }

        colors.Sort();
        foreach (var color in colors)
        {
            palette.AppendLine(color);
        }

        return palette.ToString();
    }

    /// <summary>
    /// 验证主题颜色对比度（WCAG AA 标准）
    /// </summary>
    public static Dictionary<string, string> ValidateContrast(string themeName)
    {
        var issues = new Dictionary<string, string>();

        try
        {
            var uri = new Uri($"pack://application:,,,/Lemoo.UI;component/Themes/{themeName}/{themeName}.xaml");
            var dict = new ResourceDictionary { Source = uri };

            // 检查关键对比度组合
            var checks = new[]
            {
                (background: "workbench.background", foreground: "text.primary", name: "主文本"),
                (background: "button.background", foreground: "button.foreground", name: "按钮文本"),
                (background: "input.background", foreground: "input.foreground", name: "输入框文本"),
                (background: "sidebar.selectedBackground", foreground: "sidebar.selectedForeground", name: "侧边栏选中项")
            };

            foreach (var (bgKey, fgKey, name) in checks)
            {
                if (dict[bgKey] is Color bgColor && dict[fgKey] is Color fgColor)
                {
                    var ratio = CalculateContrastRatio(bgColor, fgColor);
                    if (ratio < 4.5)
                    {
                        issues[$"{name} (对比度: {ratio:F2})"] =
                            $"背景 #{bgColor.R:X2}{bgColor.G:X2}{bgColor.B:X2} vs 前景 #{fgColor.R:X2}{fgColor.G:X2}{fgColor.B:X2}";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            issues["错误"] = ex.Message;
        }

        return issues;
    }

    /// <summary>
    /// 计算颜色对比度（WCAG 2.0）
    /// </summary>
    private static double CalculateContrastRatio(Color foreground, Color background)
    {
        double GetLuminance(Color c)
        {
            var sr = c.R / 255.0;
            var sg = c.G / 255.0;
            var sb = c.B / 255.0;

            var r = sr <= 0.03928 ? sr / 12.92 : Math.Pow((sr + 0.055) / 1.055, 2.4);
            var g = sg <= 0.03928 ? sg / 12.92 : Math.Pow((sg + 0.055) / 1.055, 2.4);
            var b = sb <= 0.03928 ? sb / 12.92 : Math.Pow((sb + 0.055) / 1.055, 2.4);

            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        var l1 = GetLuminance(foreground);
        var l2 = GetLuminance(background);

        var lighter = Math.Max(l1, l2);
        var darker = Math.Min(l1, l2);

        return (lighter + 0.05) / (darker + 0.05);
    }

    /// <summary>
    /// 获取令牌所属分类
    /// </summary>
    private static string GetTokenCategory(string token)
    {
        if (token.Contains("workbench")) return "工作台";
        if (token.Contains("titleBar")) return "标题栏";
        if (token.Contains("sidebar")) return "侧边栏";
        if (token.Contains("menu") && !token.Contains("menuItem")) return "菜单";
        if (token.Contains("menuItem")) return "菜单项";
        if (token.Contains("button")) return "按钮";
        if (token.Contains("tab")) return "标签页";
        if (token.Contains("input")) return "输入框";
        if (token.Contains("separator")) return "分隔符";
        if (token.Contains("scrollbar")) return "滚动条";
        if (token.Contains("statusBar")) return "状态栏";
        if (token.Contains("notification")) return "通知";
        if (token.Contains("dropdown")) return "下拉菜单";
        if (token.Contains("card")) return "卡片";
        if (token.Contains("listItem")) return "列表项";
        if (token.Contains("popup")) return "弹出层";
        if (token.Contains("toggleSwitch")) return "开关";
        if (token.Contains("numericUpDown")) return "NumericUpDown";
        if (token.Contains("text.")) return "文本";
        if (token.Contains("border")) return "边框";
        if (token.Contains("shadow")) return "阴影";
        if (token.Contains("semantic")) return "语义色";
        return "其他";
    }

    /// <summary>
    /// 生成主题补丁文件（用于同步缺失的令牌）
    /// </summary>
    public static string GeneratePatchFile(string targetTheme, string referenceTheme = "Base")
    {
        var patch = new StringBuilder();

        try
            {
            var targetUri = new Uri($"pack://application:,,,/Lemoo.UI;component/Themes/{targetTheme}/{targetTheme}.xaml");
            var targetDict = new ResourceDictionary { Source = targetUri };

            var refUri = new Uri($"pack://application:,,,/Lemoo.UI;component/Themes/{referenceTheme}/{referenceTheme}.xaml");
            var refDict = new ResourceDictionary { Source = refUri };

            var targetTokens = new HashSet<string>();
            foreach (var key in targetDict.Keys)
            {
                if (key is string stringKey && targetDict[stringKey] is Color && !stringKey.StartsWith("Palette."))
                {
                    targetTokens.Add(stringKey);
                }
            }

            var missingTokens = new List<(string category, string key, Color color)>();
            foreach (var key in refDict.Keys)
            {
                if (key is string stringKey && refDict[stringKey] is Color color &&
                    !stringKey.StartsWith("Palette.") && !targetTokens.Contains(stringKey))
                {
                    var category = GetTokenCategory(stringKey);
                    missingTokens.Add((category, stringKey, color));
                }
            }

            if (missingTokens.Count == 0)
            {
                patch.AppendLine($"<!-- {targetTheme} 主题已是完整的，无需补丁 -->");
            }
            else
            {
                patch.AppendLine($"<!-- ==========================================");
                patch.AppendLine($"     {targetTheme} 主题补丁");
                patch.AppendLine($"     基于 {referenceTheme} 主题生成");
                patch.AppendLine($"     缺失 {missingTokens.Count} 个语义令牌");
                patch.AppendLine($"     ============================================ -->");
                patch.AppendLine();

                var lastCategory = string.Empty;
                foreach (var (category, key, color) in missingTokens)
                {
                    if (category != lastCategory)
                    {
                        patch.AppendLine($"    <!-- {category} -->");
                        lastCategory = category;
                    }
                    patch.AppendLine($"    <Color x:Key=\"{key}\">#{color.R:X2}{color.G:X2}{color.B:X2}</Color> <!-- 从 {referenceTheme} 复制，建议调整颜色 -->");
                }
            }
        }
        catch (Exception ex)
        {
            patch.AppendLine($"<!-- 生成补丁失败: {ex.Message} -->");
        }

        return patch.ToString();
    }
}
