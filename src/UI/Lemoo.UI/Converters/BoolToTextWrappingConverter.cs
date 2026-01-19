using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lemoo.UI.Converters
{
    /// <summary>
    /// 布尔值到 TextWrapping 转换器
    /// </summary>
    public class BoolToTextWrappingConverter : IValueConverter
    {
        /// <summary>
        /// 当值为 True 时的 TextWrapping
        /// </summary>
        public TextWrapping TrueValue { get; set; } = TextWrapping.Wrap;

        /// <summary>
        /// 当值为 False 时的 TextWrapping
        /// </summary>
        public TextWrapping FalseValue { get; set; } = TextWrapping.NoWrap;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? TrueValue : FalseValue;
            }
            return FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TextWrapping wrapping)
            {
                return wrapping == TextWrapping.Wrap;
            }
            return false;
        }
    }
}
