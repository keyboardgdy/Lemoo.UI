using System;
using System.Globalization;
using System.Windows.Data;

namespace Lemoo.UI.Converters
{
    /// <summary>
    /// 数值减法转换器。
    /// </summary>
    public class SubtractConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue && parameter is string paramString && double.TryParse(paramString, out var subtractValue))
            {
                return doubleValue - subtractValue;
            }

            if (value is int intValue && parameter is string paramInt && int.TryParse(paramInt, out var subtractInt))
            {
                return intValue - subtractInt;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
