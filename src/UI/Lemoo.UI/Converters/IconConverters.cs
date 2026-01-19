using System;
using System.Globalization;
using System.Windows.Data;
using Lemoo.UI.Models.Icons;
using Lemoo.UI.Services;

namespace Lemoo.UI.Converters
{
    /// <summary>
    /// IconKind 到字形字符的转换器
    /// 优化：使用单例模式，减少内存占用
    /// </summary>
    public class IconKindToGlyphConverter : IValueConverter
    {
        private static readonly Lazy<IconService> _iconService = new(() => new IconService());

        /// <summary>
        /// 获取共享实例
        /// </summary>
        public static IconKindToGlyphConverter Instance { get; } = new IconKindToGlyphConverter();

        public IconKindToGlyphConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IconKind kind)
            {
                return _iconService.Value.GetGlyph(kind);
            }
            return "\u0000";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// IconSize 到字体大小的转换器
    /// 优化：使用单例模式，减少内存占用
    /// </summary>
    public class IconSizeToFontSizeConverter : IValueConverter
    {
        private static readonly Lazy<IconService> _iconService = new(() => new IconService());
        private static readonly Dictionary<IconSize, double> _sizeMap = new()
        {
            { IconSize.ExtraSmall, 12 },
            { IconSize.Small, 16 },
            { IconSize.Normal, 20 },
            { IconSize.Medium, 24 },
            { IconSize.Large, 32 },
            { IconSize.ExtraLarge, 48 }
        };

        /// <summary>
        /// 获取共享实例
        /// </summary>
        public static IconSizeToFontSizeConverter Instance { get; } = new IconSizeToFontSizeConverter();

        public IconSizeToFontSizeConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 直接使用字典查找，避免调用 IconService
            if (value is IconSize size)
            {
                return _sizeMap.GetValueOrDefault(size, 20.0);
            }
            return 20.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 图标可见性转换器（当图标为 None 时隐藏）
    /// </summary>
    public class IconKindToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IconKind kind && kind != IconKind.None)
            {
                return System.Windows.Visibility.Visible;
            }
            return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 图标布尔值反转转换器
    /// </summary>
    public class IconBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
            return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Windows.Visibility visibility)
            {
                return visibility == System.Windows.Visibility.Visible;
            }
            return false;
        }
    }
}
