namespace Lemoo.UI.Models.Icons
{
    /// <summary>
    /// 图标信息
    /// </summary>
    public class IconInfo
    {
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
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置图标的分类
        /// </summary>
        public string Category { get; set; } = "Uncategorized";

        /// <summary>
        /// 获取或设置搜索关键字
        /// </summary>
        public string[] Keywords { get; set; } = Array.Empty<string>();
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
