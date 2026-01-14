using System;
using System.Globalization;
using System.Windows.Data;

namespace Lemoo.UI.Converters
{
    /// <summary>
    /// 将两个值相加的转换器。
    /// </summary>
    public class MathAddConverter : IValueConverter
    {
        /// <summary>
        /// 获取或设置要添加的值。
        /// </summary>
        public double Addend { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var addend = Addend;
            if (parameter != null)
            {
                double.TryParse(parameter.ToString(), out addend);
            }

            if (value is double d)
                return d + addend;

            if (value is int i)
                return i + addend;

            if (value is float f)
                return f + addend;

            if (value is decimal dec)
                return dec + (decimal)addend;

            if (value is long l)
                return l + (long)addend;

            if (double.TryParse(value.ToString(), out var result))
                return result + addend;

            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var addend = Addend;
            if (parameter != null)
            {
                double.TryParse(parameter.ToString(), out addend);
            }

            if (value is double d)
                return d - addend;

            if (value is int i)
                return i - (int)addend;

            if (value is float f)
                return f - addend;

            if (double.TryParse(value.ToString(), out var result))
                return result - addend;

            return value;
        }
    }

    /// <summary>
    /// 将值乘以指定倍数的转换器。
    /// </summary>
    public class MathMultiplyConverter : IValueConverter
    {
        /// <summary>
        /// 获取或设置乘数。
        /// </summary>
        public double Factor { get; set; } = 1.0;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var factor = Factor;
            if (parameter != null)
            {
                double.TryParse(parameter.ToString(), out factor);
            }

            if (value is double d)
                return d * factor;

            if (value is int i)
                return i * factor;

            if (value is float f)
                return f * factor;

            if (value is decimal dec)
                return dec * (decimal)factor;

            if (value is long l)
                return l * (long)factor;

            if (double.TryParse(value.ToString(), out var result))
                return result * factor;

            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var factor = Factor;
            if (parameter != null)
            {
                double.TryParse(parameter.ToString(), out factor);
            }

            if (factor == 0)
                return value;

            if (value is double d)
                return d / factor;

            if (value is int i)
                return i / factor;

            if (value is float f)
                return f / factor;

            if (double.TryParse(value.ToString(), out var result))
                return result / factor;

            return value;
        }
    }

    /// <summary>
    /// 将值除以指定除数的转换器。
    /// </summary>
    public class MathDivideConverter : IValueConverter
    {
        /// <summary>
        /// 获取或设置除数。
        /// </summary>
        public double Divisor { get; set; } = 1.0;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var divisor = Divisor;
            if (parameter != null)
            {
                double.TryParse(parameter.ToString(), out divisor);
            }

            if (divisor == 0)
                return value;

            if (value is double d)
                return d / divisor;

            if (value is int i)
                return i / divisor;

            if (value is float f)
                return f / divisor;

            if (value is decimal dec)
                return dec / (decimal)divisor;

            if (value is long l)
                return l / (long)divisor;

            if (double.TryParse(value.ToString(), out var result))
                return result / divisor;

            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var divisor = Divisor;
            if (parameter != null)
            {
                double.TryParse(parameter.ToString(), out divisor);
            }

            if (value is double d)
                return d * divisor;

            if (value is int i)
                return i * divisor;

            if (value is float f)
                return f * divisor;

            if (double.TryParse(value.ToString(), out var result))
                return result * divisor;

            return value;
        }
    }
}
