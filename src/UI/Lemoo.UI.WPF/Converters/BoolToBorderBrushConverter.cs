using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Lemoo.UI.WPF.Converters;

/// <summary>
/// 布尔值到边框画刷的转换器（选中时显示主题强调色边框）
/// </summary>
public class BoolToBorderBrushConverter : IValueConverter
{
    /// <summary>
    /// 转换布尔值为边框画刷
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            // 选中时使用主题强调色边框（跟随主题）
            return Application.Current.TryFindResource("AccentColorBrush") as Brush
                   ?? new SolidColorBrush(Color.FromRgb(0, 173, 181));
        }

        // 未选中时使用普通边框
        return Application.Current.TryFindResource("InputBorderBrush") as Brush
               ?? new SolidColorBrush(Color.FromRgb(58, 63, 70));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
