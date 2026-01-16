using System;

namespace Lemoo.UI.Models.Icons
{
    /// <summary>
    /// 图标数据特性，用于为 IconKind 枚举值附加元数据
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class IconDataAttribute : Attribute
    {
        /// <summary>
        /// 获取或设置图标的 Unicode 字符
        /// </summary>
        public string Glyph { get; }

        /// <summary>
        /// 获取或设置图标的名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取或设置图标的分类
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
    /// 图标大小枚举
    /// </summary>
    public enum IconSize
    {
        /// <summary>
        /// 特小尺寸 (12px)
        /// </summary>
        ExtraSmall,

        /// <summary>
        /// 小尺寸 (16px)
        /// </summary>
        Small,

        /// <summary>
        /// 默认尺寸 (20px)
        /// </summary>
        Normal,

        /// <summary>
        /// 中等尺寸 (24px)
        /// </summary>
        Medium,

        /// <summary>
        /// 大尺寸 (32px)
        /// </summary>
        Large,

        /// <summary>
        /// 特大尺寸 (48px)
        /// </summary>
        ExtraLarge,
    }

    /// <summary>
    /// 图标变体样式
    /// </summary>
    public enum IconVariant
    {
        /// <summary>
        /// 填充样式
        /// </summary>
        Filled,

        /// <summary>
        /// 轻量样式
        /// </summary>
        Light,

        /// <summary>
        /// 常规样式
        /// </summary>
        Regular,

        /// <summary>
        /// 半填充样式
        /// </summary>
        SemiFilled,
    }
}
