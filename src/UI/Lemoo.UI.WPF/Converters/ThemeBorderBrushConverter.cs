using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Lemoo.UI.Helpers;

namespace Lemoo.UI.WPF.Converters;

/// <summary>
/// 主题边框画刷多值转换器 - 根据主题是否选中返回对应的边框颜色
/// </summary>
public class ThemeBorderBrushConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length >= 2 && values[0] is ThemeInfo selectedTheme && values[1] is ThemeInfo currentTheme)
        {
            if (selectedTheme.ThemeType == currentTheme.ThemeType)
            {
                // 选中时返回主题强调色
                return Application.Current.TryFindResource("InputFocusBorderBrush") as Brush
                       ?? new SolidColorBrush(Color.FromRgb(0, 173, 181));
            }
        }

        // 未选中时返回普通边框颜色
        return Application.Current.TryFindResource("InputBorderBrush") as Brush
               ?? new SolidColorBrush(Color.FromRgb(58, 63, 70));
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
