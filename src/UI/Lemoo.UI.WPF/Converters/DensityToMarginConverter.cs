using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lemoo.UI.WPF.Converters
{
    /// <summary>
    /// 密度值转Margin转换器（用于定位滑块按钮）
    /// </summary>
    public class DensityToMarginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 &&
                values[0] is double density &&
                values[1] is double trackWidth)
            {
                const double thumbWidth = 14.0; // 滑块宽度
                var position = (density / 100.0) * (trackWidth - thumbWidth);
                return new Thickness(position, 0, 0, 0);
            }
            return new Thickness(0, 0, 0, 0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
