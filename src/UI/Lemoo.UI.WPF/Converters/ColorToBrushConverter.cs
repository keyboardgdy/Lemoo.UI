using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Lemoo.UI.WPF.Converters;

/// <summary>
/// 颜色值到画刷的转换器
/// </summary>
public class ColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Color color)
        {
            return new SolidColorBrush(color);
        }
        return Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
