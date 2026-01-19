using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lemoo.UI.Converters
{
    /// <summary>
    /// 空值到可见性转换器。
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 当值为 null 时的可见性
        /// </summary>
        public Visibility NullValue { get; set; } = Visibility.Collapsed;

        /// <summary>
        /// 当值不为 null 时的可见性
        /// </summary>
        public Visibility NonNullValue { get; set; } = Visibility.Visible;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? NullValue : NonNullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
