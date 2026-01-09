using System;
using System.Globalization;
using System.Windows.Data;

namespace Lemoo.UI.Converters;

/// <summary>
/// 展开图标转换器：根据 bool IsExpanded 返回不同的 MDL2 图标字符。
/// </summary>
public class ExpandIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isExpanded)
        {
            // 展开：向下箭头，折叠：向右箭头
            return isExpanded ? "\uE70D" : "\uE76C";
        }
        return "\uE76C";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}


