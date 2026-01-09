using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lemoo.UI.Converters;

/// <summary>
/// 布尔值到可见性转换器
/// </summary>
[ValueConversion(typeof(bool), typeof(Visibility))]
public class BoolToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// 将布尔值转换为可见性
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            // 支持反向转换（通过parameter）
            if (parameter?.ToString() == "Inverse")
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    /// <summary>
    /// 将可见性转换回布尔值
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            return visibility == Visibility.Visible;
        }
        return false;
    }
}

