namespace Lemoo.UI.Models.Icons
{
    /// <summary>
    /// 图标数据属性，用于为 IconKind 枚举值提供元数据
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class IconDataAttribute : Attribute
    {
        /// <summary>
        /// 获取图标的 Unicode 字符
        /// </summary>
        public string Glyph { get; }

        /// <summary>
        /// 获取图标的名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取图标的分类
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// 初始化 IconDataAttribute 的新实例
        /// </summary>
        /// <param name="glyph">图标的 Unicode 字符</param>
        /// <param name="name">图标的名称</param>
        /// <param name="category">图标的分类</param>
        public IconDataAttribute(string glyph, string name, string category)
        {
            Glyph = glyph;
            Name = name;
            Category = category;
        }
    }

    /// <summary>
    /// 图标尺寸枚举
    /// </summary>
    public enum IconSize
    {
        /// <summary>
        /// 超小图标 (12px)
        /// </summary>
        ExtraSmall,

        /// <summary>
        /// 小图标 (16px)
        /// </summary>
        Small,

        /// <summary>
        /// 正常图标 (20px)
        /// </summary>
        Normal,

        /// <summary>
        /// 中等图标 (24px)
        /// </summary>
        Medium,

        /// <summary>
        /// 大图标 (32px)
        /// </summary>
        Large,

        /// <summary>
        /// 超大图标 (48px)
        /// </summary>
        ExtraLarge
    }

    /// <summary>
    /// 图标变体枚举
    /// </summary>
    public enum IconVariant
    {
        /// <summary>
        /// 常规（填充）变体
        /// </summary>
        Regular,

        /// <summary>
        /// 轻量变体
        /// </summary>
        Light,

        /// <summary>
        /// 半填充变体
        /// </summary>
        SemiFilled,

        /// <summary>
        /// 轮廓变体
        /// </summary>
        Outlined
    }
}
