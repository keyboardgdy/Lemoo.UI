using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Lemoo.UI.Models.Icons;

namespace Lemoo.UI.Services
{
    /// <summary>
    /// 基于 JSON 元数据的图标注册表
    /// 从 IconMetadata.json 加载图标信息，支持中英文搜索和多语言
    /// </summary>
    public static class IconMetadataRegistry
    {
        private static Dictionary<IconKind, IconInfo> _iconCache = new();
        private static Dictionary<string, List<IconInfo>> _categoryCache = new();
        private static Dictionary<string, IconMetadataItem> _metadataCache = new();
        private static bool _isInitialized = false;
        private static readonly object _lock = new();

        /// <summary>
        /// 初始化图标注册表，从 IconMetadata.json 加载数据
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized) return;

            lock (_lock)
            {
                if (_isInitialized) return;

                _iconCache.Clear();
                _categoryCache.Clear();
                _metadataCache.Clear();

                // 加载 JSON 元数据
                var metadata = LoadIconMetadata();
                if (metadata != null)
                {
                    LoadFromMetadata(metadata);
                }
                else
                {
                    // 如果 JSON 加载失败，回退到反射方式
                    LoadFromReflection();
                }

                _isInitialized = true;
            }
        }

        /// <summary>
        /// 从 IconMetadata.json 加载元数据
        /// </summary>
        private static IconMetadataContainer? LoadIconMetadata()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();

                // 首先尝试从嵌入资源加载（这是最可靠的方式）
                var resourceStream = assembly.GetManifestResourceStream("Lemoo.UI.Models.Icons.IconMetadata.json");
                if (resourceStream != null)
                {
                    using var reader = new StreamReader(resourceStream);
                    var json = reader.ReadToEnd();
                    System.Diagnostics.Debug.WriteLine("Successfully loaded IconMetadata.json from embedded resources");
                    return JsonSerializer.Deserialize<IconMetadataContainer>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                // 如果嵌入资源失败，尝试从文件系统加载
                // 尝试多个可能的路径
                var possiblePaths = new[]
                {
                    Path.Combine(AppContext.BaseDirectory, "Models", "Icons", "IconMetadata.json"),
                    Path.Combine(AppContext.BaseDirectory, "..", "Models", "Icons", "IconMetadata.json"),
                    Path.Combine(AppContext.BaseDirectory, "..", "..", "Models", "Icons", "IconMetadata.json"),
                };

                foreach (var path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        var json = File.ReadAllText(path);
                        System.Diagnostics.Debug.WriteLine($"Successfully loaded IconMetadata.json from: {path}");
                        return JsonSerializer.Deserialize<IconMetadataContainer>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                }

                System.Diagnostics.Debug.WriteLine("IconMetadata.json not found in any location");
            }
            catch (Exception ex)
            {
                // 静默失败，回退到反射方式
                System.Diagnostics.Debug.WriteLine($"Failed to load IconMetadata.json: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// 解析 Unicode 字符串
        /// 支持格式："\uE72B" 或 "E72B" 或实际的 Unicode 字符
        /// </summary>
        private static string ParseUnicodeString(string? unicodeString, string? unicode)
        {
            // 优先使用 UnicodeString
            if (!string.IsNullOrEmpty(unicodeString))
            {
                // 处理 "\\uXXXX" 格式（JSON 中的转义 Unicode）
                if (unicodeString.StartsWith("\\u") && unicodeString.Length == 6)
                {
                    var hexCode = unicodeString.Substring(2);
                    if (int.TryParse(hexCode, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var codePoint))
                    {
                        return char.ConvertFromUtf32(codePoint);
                    }
                }

                // 处理 "uXXXX" 格式（JSON 中的 unicode_string 字段）
                if (unicodeString.StartsWith("u") && unicodeString.Length == 5)
                {
                    var hexCode = unicodeString.Substring(1);
                    if (int.TryParse(hexCode, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var codePoint))
                    {
                        return char.ConvertFromUtf32(codePoint);
                    }
                }

                // 如果已经是单个字符，直接返回
                if (unicodeString.Length == 1)
                {
                    return unicodeString;
                }
            }

            // 回退到使用 Unicode 字段
            if (!string.IsNullOrEmpty(unicode) && unicode.Length == 4)
            {
                if (int.TryParse(unicode, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var codePoint))
                {
                    return char.ConvertFromUtf32(codePoint);
                }
            }

            // 默认返回空字符
            return "\u0000";
        }

        /// <summary>
        /// 获取诊断信息（用于调试）
        /// </summary>
        public static string GetDiagnosticInfo()
        {
            EnsureInitialized();

            var info = new System.Text.StringBuilder();
            info.AppendLine($"=== IconMetadataRegistry 诊断信息 ===");
            info.AppendLine($"已初始化: {_isInitialized}");
            info.AppendLine($"缓存图标数量: {_iconCache.Count}");
            info.AppendLine($"分类数量: {_categoryCache.Count}");

            // 获取前5个图标作为示例
            info.AppendLine("\n前5个图标示例:");
            int count = 0;
            foreach (var icon in _iconCache.Values.OrderBy(x => x.Name).Take(5))
            {
                info.AppendLine($"  [{count + 1}] {icon.Kind}: {icon.Name} = Glyph: '{icon.Glyph}' (U+{(int)icon.Glyph[0]:X4})");
                count++;
            }

            // 测试一些常见的图标
            info.AppendLine("\n测试常见图标:");
            var testKinds = new[] { IconKind.Back, IconKind.Forward, IconKind.Search, IconKind.Home, IconKind.Settings };
            foreach (var kind in testKinds)
            {
                if (_iconCache.TryGetValue(kind, out var icon))
                {
                    info.AppendLine($"  {kind}: '{icon.Glyph}' (U+{(int)icon.Glyph[0]:X4})");
                }
                else
                {
                    info.AppendLine($"  {kind}: 未找到");
                }
            }

            return info.ToString();
        }

        /// <summary>
        /// 从 JSON 元数据构建缓存
        /// </summary>
        private static void LoadFromMetadata(IconMetadataContainer metadata)
        {
            foreach (var iconData in metadata.Icons)
            {
                // 解析 IconKind 枚举值
                if (Enum.TryParse<IconKind>(iconData.EnumName ?? iconData.Name, true, out var kind))
                {
                    var info = new IconInfo
                    {
                        Kind = kind,
                        // 修复：正确解析 Unicode 字符串
                        Glyph = ParseUnicodeString(iconData.UnicodeString, iconData.Unicode),
                        Name = iconData.Name,
                        Category = iconData.Category,  // 保留原始分类键
                        Keywords = iconData.Keywords ?? Array.Empty<string>()
                    };

                    _iconCache[kind] = info;

                    // 构建分类缓存 - 使用原始分类键
                    var categoryKey = iconData.Category;
                    if (!_categoryCache.ContainsKey(categoryKey))
                    {
                        _categoryCache[categoryKey] = new List<IconInfo>();
                    }
                    _categoryCache[categoryKey].Add(info);

                    // 缓存元数据项
                    _metadataCache[kind.ToString()] = iconData;
                }
            }
        }

        /// <summary>
        /// 回退方案：从反射加载（兼容旧代码）
        /// </summary>
        private static void LoadFromReflection()
        {
            foreach (IconKind kind in Enum.GetValues<IconKind>())
            {
                var field = kind.GetType().GetField(kind.ToString());
                var attribute = field?.GetCustomAttributes(typeof(IconDataAttribute), false)
                                       .FirstOrDefault() as IconDataAttribute;

                if (attribute != null)
                {
                    var info = new IconInfo
                    {
                        Kind = kind,
                        Glyph = attribute.Glyph,
                        Name = attribute.Name,
                        Category = attribute.Category,
                        Keywords = GetDefaultKeywords(kind, attribute.Name)
                    };

                    _iconCache[kind] = info;

                    if (!_categoryCache.ContainsKey(info.Category))
                    {
                        _categoryCache[info.Category] = new List<IconInfo>();
                    }
                    _categoryCache[info.Category].Add(info);
                }
            }
        }

        /// <summary>
        /// 获取默认关键字
        /// </summary>
        private static string[] GetDefaultKeywords(IconKind kind, string name)
        {
            var keywords = new List<string>
            {
                name.ToLower(),
                kind.ToString().ToLower()
            };

            var category = GetCategoryFromKind(kind);
            if (!string.IsNullOrEmpty(category))
            {
                keywords.Add(category.ToLower());
            }

            return keywords.ToArray();
        }

        /// <summary>
        /// 从 IconKind 获取分类
        /// </summary>
        private static string GetCategoryFromKind(IconKind kind)
        {
            var field = kind.GetType().GetField(kind.ToString());
            var attribute = field?.GetCustomAttributes(typeof(IconDataAttribute), false)
                                   .FirstOrDefault() as IconDataAttribute;
            return attribute?.Category ?? "Uncategorized";
        }

        /// <summary>
        /// 获取指定图标的信息
        /// </summary>
        public static IconInfo? GetIcon(IconKind kind)
        {
            EnsureInitialized();
            return _iconCache.GetValueOrDefault(kind);
        }

        /// <summary>
        /// 获取所有图标
        /// </summary>
        public static IEnumerable<IconInfo> GetAllIcons()
        {
            EnsureInitialized();
            return _iconCache.Values.OrderBy(x => x.Name);
        }

        /// <summary>
        /// 根据分类获取图标
        /// </summary>
        public static IEnumerable<IconInfo> GetIconsByCategory(string category)
        {
            EnsureInitialized();
            return _categoryCache.GetValueOrDefault(category, new List<IconInfo>())
                                .OrderBy(x => x.Name);
        }

        /// <summary>
        /// 获取所有分类
        /// </summary>
        public static IEnumerable<IconCategoryInfo> GetCategories()
        {
            EnsureInitialized();
            return _categoryCache.OrderBy(x => GetCategoryPriority(x.Key))
                                .Select(x => new IconCategoryInfo
                                {
                                    Key = x.Key,
                                    DisplayName = GetCategoryDisplayName(x.Key),
                                    Count = x.Value.Count,
                                    Priority = GetCategoryPriority(x.Key)
                                });
        }

        /// <summary>
        /// 搜索图标（支持中英文关键词）
        /// </summary>
        public static IEnumerable<IconInfo> SearchIcons(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<IconInfo>();

            EnsureInitialized();
            var term = searchTerm.ToLower();

            return _iconCache.Values
                .Where(x => x.KeywordsLower.Any(k => k.Contains(term)) ||
                           x.NameLower.Contains(term) ||
                           x.CategoryLower.Contains(term) ||
                           x.Kind.ToString().ToLower().Contains(term))
                .OrderBy(x => x.Name);
        }

        /// <summary>
        /// 获取图标的元数据
        /// </summary>
        public static IconMetadataItem? GetMetadata(IconKind kind)
        {
            EnsureInitialized();
            return _metadataCache.GetValueOrDefault(kind.ToString());
        }

        /// <summary>
        /// 获取图标的字形字符
        /// </summary>
        public static string GetGlyph(IconKind kind)
        {
            return GetIcon(kind)?.Glyph ?? "\u0000";
        }

        /// <summary>
        /// 确保注册表已初始化
        /// </summary>
        private static void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                Initialize();
            }
        }

        /// <summary>
        /// 获取分类的显示名称
        /// </summary>
        private static string GetCategoryDisplayName(string categoryKey)
        {
            return categoryKey switch
            {
                "navigation" => "导航",
                "actions" => "操作",
                "media" => "媒体",
                "communication" => "通信",
                "files" => "文件",
                "status" => "状态",
                "ui" => "界面",
                "development" => "开发",
                "security" => "安全",
                _ => categoryKey
            };
        }

        /// <summary>
        /// 获取分类的排序优先级
        /// </summary>
        private static int GetCategoryPriority(string categoryKey)
        {
            return categoryKey switch
            {
                "navigation" => 1,
                "actions" => 2,
                "media" => 3,
                "communication" => 4,
                "files" => 5,
                "status" => 6,
                "ui" => 7,
                "development" => 8,
                "security" => 9,
                _ => 100
            };
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public static void ClearCache()
        {
            _iconCache.Clear();
            _categoryCache.Clear();
            _metadataCache.Clear();
            _isInitialized = false;
        }
    }

    #region JSON 数据模型

    /// <summary>
    /// IconMetadata.json 容器
    /// </summary>
    internal class IconMetadataContainer
    {
        public List<IconMetadataItem> Icons { get; set; } = new();
    }

    /// <summary>
    /// 图标元数据项
    /// </summary>
    public class IconMetadataItem
    {
        public string Glyph { get; set; } = string.Empty;
        public string Unicode { get; set; } = string.Empty;
        public string? UnicodeString { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? EnumName { get; set; }
        public string Category { get; set; } = string.Empty;
        public string[]? Keywords { get; set; }
        public I18nInfo? I18n { get; set; }
    }

    /// <summary>
    /// 国际化信息
    /// </summary>
    public class I18nInfo
    {
        public string? En { get; set; }
        public string? Zh { get; set; }
    }

    #endregion
}
