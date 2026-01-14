using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lemoo.UI.Converters
{
    /// <summary>
    /// 比较两个值是否相等的转换器。
    /// </summary>
    public class EqualToConverter : IValueConverter
    {
        /// <summary>
        /// 获取或设置比较值。
        /// </summary>
        public object? CompareValue { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var compareValue = parameter ?? CompareValue;
            if (compareValue == null)
                return value == null;

            return compareValue.Equals(value);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var compareValue = parameter ?? CompareValue;
            return (bool)value == true ? compareValue : Binding.DoNothing;
        }
    }

    /// <summary>
    /// 比较值是否大于指定值的转换器。
    /// </summary>
    public class GreaterThanConverter : IValueConverter
    {
        /// <summary>
        /// 获取或设置比较值。
        /// </summary>
        public double CompareValue { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            double compareValue = CompareValue;
            if (parameter != null)
            {
                double.TryParse(parameter.ToString(), out compareValue);
            }

            if (value == null)
                return false;

            if (value is double d)
                return d > compareValue;

            if (value is int i)
                return i > compareValue;

            if (value is float f)
                return f > compareValue;

            if (value is decimal dec)
                return (double)dec > compareValue;

            if (value is long l)
                return l > (long)compareValue;

            if (double.TryParse(value.ToString(), out var result))
                return result > compareValue;

            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 比较值是否小于指定值的转换器。
    /// </summary>
    public class LessThanConverter : IValueConverter
    {
        /// <summary>
        /// 获取或设置比较值。
        /// </summary>
        public double CompareValue { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            double compareValue = CompareValue;
            if (parameter != null)
            {
                double.TryParse(parameter.ToString(), out compareValue);
            }

            if (value == null)
                return false;

            if (value is double d)
                return d < compareValue;

            if (value is int i)
                return i < compareValue;

            if (value is float f)
                return f < compareValue;

            if (value is decimal dec)
                return (double)dec < compareValue;

            if (value is long l)
                return l < (long)compareValue;

            if (double.TryParse(value.ToString(), out var result))
                return result < compareValue;

            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 比较值是否在指定范围内的转换器。
    /// </summary>
    public class RangeConverter : IValueConverter
    {
        /// <summary>
        /// 获取或设置最小值。
        /// </summary>
        public double Minimum { get; set; }

        /// <summary>
        /// 获取或设置最大值。
        /// </summary>
        public double Maximum { get; set; }

        /// <summary>
        /// 获取或设置是否包含边界值。
        /// </summary>
        public bool Inclusive { get; set; } = true;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            double val;
            if (value is double d)
            {
                val = d;
            }
            else if (value is int i)
            {
                val = i;
            }
            else if (value is float f)
            {
                val = f;
            }
            else if (value is decimal dec)
            {
                val = (double)dec;
            }
            else if (value is long l)
            {
                val = l;
            }
            else if (!double.TryParse(value.ToString(), out val))
            {
                return false;
            }

            if (Inclusive)
            {
                return val >= Minimum && val <= Maximum;
            }
            else
            {
                return val > Minimum && val < Maximum;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
