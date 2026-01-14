using System.Globalization;
using System.Windows.Data;
using Lemoo.UI.Helpers;

namespace Lemoo.UI.WPF.Converters;

/// <summary>
/// 主题相等性多值转换器 - 判断两个主题是否相等
/// </summary>
public class ThemeEqualsMultiConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length >= 2 && values[0] is ThemeInfo selectedTheme && values[1] is ThemeInfo currentTheme)
        {
            return selectedTheme.ThemeType == currentTheme.ThemeType;
        }
        return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
