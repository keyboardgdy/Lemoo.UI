using System.Globalization;
using System.Windows.Data;

namespace Lemoo.UI.WPF.Converters
{
    /// <summary>
    /// 密度值转进度宽度转换器
    /// </summary>
    public class DensityToProgressConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 &&
                values[0] is double density &&
                values[1] is double trackWidth)
            {
                return (density / 100.0) * trackWidth;
            }
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
