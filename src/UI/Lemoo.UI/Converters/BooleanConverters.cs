using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lemoo.UI.Converters
{
    /// <summary>
    /// 将布尔值转换为对象的转换器。
    /// </summary>
    public class BoolToObjectConverter : IValueConverter
    {
        /// <summary>
        /// 获取或设置为 true 时返回的值。
        /// </summary>
        public object? TrueValue { get; set; }

        /// <summary>
        /// 获取或设置为 false 时返回的值。
        /// </summary>
        public object? FalseValue { get; set; }

        /// <summary>
        /// 获取或设置为 null 时返回的值。
        /// </summary>
        public object? NullValue { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return NullValue ?? FalseValue;

            bool boolValue;
            if (value is bool b)
            {
                boolValue = b;
            }
            else if (bool.TryParse(value.ToString(), out boolValue))
            {
                // 已解析
            }
            else
            {
                return FalseValue;
            }

            return boolValue ? TrueValue : FalseValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            return Equals(value, TrueValue);
        }
    }

    /// <summary>
    /// 反转布尔值的转换器。
    /// </summary>
    public class InverseBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return true;

            if (value is bool b)
                return !b;

            if (bool.TryParse(value.ToString(), out var result))
                return !result;

            return true;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return true;

            if (value is bool b)
                return !b;

            if (bool.TryParse(value.ToString(), out var result))
                return !result;

            return true;
        }
    }

    /// <summary>
    /// 将 null 值转换为布尔值的转换器。
    /// </summary>
    public class NullToBoolConverter : IValueConverter
    {
        /// <summary>
        /// 获取或设置 null 时返回的值。
        /// </summary>
        public bool NullValue { get; set; } = true;

        /// <summary>
        /// 获取或设置非 null 时返回的值。
        /// </summary>
        public bool NonNullValue { get; set; } = false;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value == null ? NullValue : NonNullValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // 无法可靠地转换回来
            return DependencyProperty.UnsetValue;
        }
    }

    /// <summary>
    /// 将布尔值转换为枚举值的转换器。
    /// </summary>
    public class EnumToBooleanConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            return value.Equals(parameter);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Binding.DoNothing;

            return (bool)value ? parameter : Binding.DoNothing;
        }
    }
}
