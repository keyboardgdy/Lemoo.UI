using System;
using System.Collections.Generic;
using System.Linq;
using Lemoo.UI.Models.Icons;

namespace Lemoo.UI.Services
{
    /// <summary>
    /// 图标注册表，提供图标信息的查询和管理功能
    /// </summary>
    public static class IconRegistry
    {
        private static Dictionary<IconKind, IconInfo> _iconCache = new();
        private static Dictionary<string, List<IconInfo>> _categoryCache = new();
        private static bool _isInitialized = false;

        /// <summary>
        /// 初始化图标注册表
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized) return;

            _iconCache.Clear();
            _categoryCache.Clear();

            // 从 IconKind 枚举中读取图标信息
            foreach (IconKind kind in Enum.GetValues<IconKind>())
            {
                var info = GetIconInfoFromEnum(kind);
                _iconCache[kind] = info;

                if (!_categoryCache.ContainsKey(info.Category))
                {
                    _categoryCache[info.Category] = new List<IconInfo>();
                }
                _categoryCache[info.Category].Add(info);
            }

            _isInitialized = true;
        }

        /// <summary>
        /// 从枚举获取图标信息
        /// </summary>
        private static IconInfo GetIconInfoFromEnum(IconKind kind)
        {
            var field = kind.GetType().GetField(kind.ToString());
            var attribute = field?.GetCustomAttributes(typeof(IconDataAttribute), false)
                                   .FirstOrDefault() as IconDataAttribute;

            if (attribute != null)
            {
                return new IconInfo
                {
                    Kind = kind,
                    Glyph = attribute.Glyph,
                    Name = attribute.Name,
                    Category = attribute.Category,
                    Keywords = GetDefaultKeywords(kind, attribute.Name)
                };
            }

            return new IconInfo
            {
                Kind = kind,
                Glyph = "\u0000",
                Name = kind.ToString(),
                Category = "Uncategorized"
            };
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

            // 添加分类作为关键字
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
        /// <param name="kind">图标类型</param>
        /// <returns>图标信息，如果不存在则返回 null</returns>
        public static IconInfo? GetIcon(IconKind kind)
        {
            EnsureInitialized();
            return _iconCache.GetValueOrDefault(kind);
        }

        /// <summary>
        /// 获取所有图标
        /// </summary>
        /// <returns>所有图标信息的集合</returns>
        public static IEnumerable<IconInfo> GetAllIcons()
        {
            EnsureInitialized();
            return _iconCache.Values.OrderBy(x => x.Name);
        }

        /// <summary>
        /// 根据分类获取图标
        /// </summary>
        /// <param name="category">分类名称</param>
        /// <returns>该分类下的所有图标</returns>
        public static IEnumerable<IconInfo> GetIconsByCategory(string category)
        {
            EnsureInitialized();
            return _categoryCache.GetValueOrDefault(category, new List<IconInfo>())
                                .OrderBy(x => x.Name);
        }

        /// <summary>
        /// 获取所有分类
        /// </summary>
        /// <returns>所有分类的信息</returns>
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
        /// 搜索图标
        /// </summary>
        /// <param name="searchTerm">搜索关键词</param>
        /// <returns>匹配的图标列表</returns>
        public static IEnumerable<IconInfo> SearchIcons(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<IconInfo>();

            EnsureInitialized();
            var term = searchTerm.ToLower();

            return _iconCache.Values
                .Where(x => x.Keywords.Any(k => k.Contains(term)) ||
                           x.Name.ToLower().Contains(term) ||
                           x.Kind.ToString().ToLower().Contains(term))
                .OrderBy(x => x.Name);
        }

        /// <summary>
        /// 获取图标的字形字符
        /// </summary>
        /// <param name="kind">图标类型</param>
        /// <returns>字形的 Unicode 字符</returns>
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
        private static string GetCategoryDisplayName(string category)
        {
            return category switch
            {
                "Navigation" => "导航",
                "Action" => "操作",
                "File" => "文件",
                "Status" => "状态",
                _ => category
            };
        }

        /// <summary>
        /// 获取分类的排序优先级
        /// </summary>
        private static int GetCategoryPriority(string category)
        {
            return category switch
            {
                "Navigation" => 1,
                "Action" => 2,
                "File" => 3,
                "Status" => 4,
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
            _isInitialized = false;
        }
    }
}
