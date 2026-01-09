using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lemoo.UI.Converters;

/// <summary>
/// 字符串到可见性转换器：如果字符串为空或 null，返回 Collapsed，否则 Visible。
/// 支持通过 parameter = "Inverse" 反转逻辑。
/// </summary>
[ValueConversion(typeof(string), typeof(Visibility))]
public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isEmpty = value switch
        {
            null => true,
            string s => string.IsNullOrWhiteSpace(s),
            _ => false
        };

        bool inverse = string.Equals(parameter?.ToString(), "Inverse", StringComparison.OrdinalIgnoreCase);

        if (inverse)
        {
            return isEmpty ? Visibility.Visible : Visibility.Collapsed;
        }

        return isEmpty ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}


