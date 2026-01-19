using System.Linq;

namespace Lemoo.UI.Models.Icons
{
    /// <summary>
    /// 图标信息
    /// </summary>
    public class IconInfo
    {
        private string _name = string.Empty;
        private string _category = "Uncategorized";
        private string[] _keywords = Array.Empty<string>();
        private string _nameLower = string.Empty;
        private string _categoryLower = string.Empty;
        private string[] _keywordsLower = Array.Empty<string>();

        /// <summary>
        /// 获取或设置图标类型
        /// </summary>
        public IconKind Kind { get; set; }

        /// <summary>
        /// 获取或设置图标的 Unicode 字符
        /// </summary>
        public string Glyph { get; set; } = "\u0000";

        /// <summary>
        /// 获取或设置图标的名称
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value ?? string.Empty;
                _nameLower = _name.ToLower();
            }
        }

        /// <summary>
        /// 获取图标的名称（小写，用于搜索）
        /// </summary>
        public string NameLower => _nameLower;

        /// <summary>
        /// 获取或设置图标的分类
        /// </summary>
        public string Category
        {
            get => _category;
            set
            {
                _category = value ?? "Uncategorized";
                _categoryLower = _category.ToLower();
            }
        }

        /// <summary>
        /// 获取图标的分类（小写，用于搜索）
        /// </summary>
        public string CategoryLower => _categoryLower;

        /// <summary>
        /// 获取或设置搜索关键字
        /// </summary>
        public string[] Keywords
        {
            get => _keywords;
            set
            {
                _keywords = value;
                // 预先小写化关键词以优化搜索性能
                _keywordsLower = value != null
                    ? value.Select(k => k.ToLower()).ToArray()
                    : Array.Empty<string>();
            }
        }

        /// <summary>
        /// 获取搜索关键字（小写，用于搜索）
        /// </summary>
        public string[] KeywordsLower => _keywordsLower;
    }

    /// <summary>
    /// 图标分类信息
    /// </summary>
    public class IconCategoryInfo
    {
        /// <summary>
        /// 获取或设置分类键
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置显示名称
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置图标数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 获取或设置排序优先级
        /// </summary>
        public int Priority { get; set; }
    }
}
