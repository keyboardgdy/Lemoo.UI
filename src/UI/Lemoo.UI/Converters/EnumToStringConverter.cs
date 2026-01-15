using System.Globalization;
using System.Windows.Data;

namespace Lemoo.UI.Converters;

/// <summary>
/// 枚举到字符串的转换器
/// </summary>
public class EnumToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return string.Empty;

        var enumType = value.GetType();
        if (!enumType.IsEnum) return value.ToString() ?? string.Empty;

        // 获取枚举的Display名称或描述
        var field = enumType.GetField(value.ToString() ?? string.Empty);
        if (field != null)
        {
            // 可以在这里添加Display特性支持
            return value.ToString() ?? string.Empty;
        }

        return value.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue && targetType.IsEnum)
        {
            return Enum.Parse(targetType, stringValue);
        }
        return Binding.DoNothing;
    }
}
