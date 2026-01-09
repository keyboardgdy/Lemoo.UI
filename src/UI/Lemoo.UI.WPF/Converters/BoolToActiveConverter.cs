using System.Globalization;
using System.Windows.Data;

namespace Lemoo.UI.WPF;

/// <summary>
/// 布尔值到激活状态的转换器
/// </summary>
public class BoolToActiveConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isActive)
        {
            return isActive ? "激活" : "停用";
        }
        return "未知";
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

