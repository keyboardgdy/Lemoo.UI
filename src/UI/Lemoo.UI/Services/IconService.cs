using System;
using System.Collections.Generic;
using System.Linq;
using Lemoo.UI.Models.Icons;

namespace Lemoo.UI.Services
{
    /// <summary>
    /// 图标服务接口
    /// </summary>
    public interface IIconService
    {
        /// <summary>
        /// 获取图标信息
        /// </summary>
        IconInfo? GetIcon(IconKind kind);

        /// <summary>
        /// 获取所有图标
        /// </summary>
        IEnumerable<IconInfo> GetAllIcons();

        /// <summary>
        /// 按分类获取图标
        /// </summary>
        IEnumerable<IconInfo> GetIconsByCategory(string category);

        /// <summary>
        /// 获取所有分类
        /// </summary>
        IEnumerable<IconCategoryInfo> GetCategories();

        /// <summary>
        /// 搜索图标
        /// </summary>
        IEnumerable<IconInfo> SearchIcons(string searchTerm);

        /// <summary>
        /// 获取图标字形
        /// </summary>
        string GetGlyph(IconKind kind);

        /// <summary>
        /// 获取图标尺寸值（像素）
        /// </summary>
        double GetIconSize(IconSize size);
    }

    /// <summary>
    /// 图标服务实现
    /// </summary>
    public class IconService : IIconService
    {
        private readonly Dictionary<IconSize, double> _sizeMap = new()
        {
            { IconSize.ExtraSmall, 12 },
            { IconSize.Small, 16 },
            { IconSize.Normal, 20 },
            { IconSize.Medium, 24 },
            { IconSize.Large, 32 },
            { IconSize.ExtraLarge, 48 }
        };

        /// <summary>
        /// 初始化图标服务
        /// </summary>
        public IconService()
        {
            IconMetadataRegistry.Initialize();
        }

        /// <inheritdoc/>
        public IconInfo? GetIcon(IconKind kind)
        {
            return IconMetadataRegistry.GetIcon(kind);
        }

        /// <inheritdoc/>
        public IEnumerable<IconInfo> GetAllIcons()
        {
            return IconMetadataRegistry.GetAllIcons();
        }

        /// <inheritdoc/>
        public IEnumerable<IconInfo> GetIconsByCategory(string category)
        {
            return IconMetadataRegistry.GetIconsByCategory(category);
        }

        /// <inheritdoc/>
        public IEnumerable<IconCategoryInfo> GetCategories()
        {
            return IconMetadataRegistry.GetCategories();
        }

        /// <inheritdoc/>
        public IEnumerable<IconInfo> SearchIcons(string searchTerm)
        {
            return IconMetadataRegistry.SearchIcons(searchTerm);
        }

        /// <inheritdoc/>
        public string GetGlyph(IconKind kind)
        {
            return IconMetadataRegistry.GetGlyph(kind);
        }

        /// <inheritdoc/>
        public double GetIconSize(IconSize size)
        {
            return _sizeMap.GetValueOrDefault(size, 20);
        }
    }
}
