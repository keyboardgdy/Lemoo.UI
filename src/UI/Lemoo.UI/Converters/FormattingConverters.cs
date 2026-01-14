using System;
using System.Globalization;
using System.Windows.Data;

namespace Lemoo.UI.Converters
{
    /// <summary>
    /// 格式化字符串的转换器。
    /// </summary>
    public class StringFormatConverter : IValueConverter
    {
        /// <summary>
        /// 获取或设置格式字符串。
        /// </summary>
        public string? FormatString { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var format = parameter as string ?? FormatString;

            if (string.IsNullOrEmpty(format))
                return value;

            try
            {
                return string.Format(culture, format, value);
            }
            catch
            {
                return value;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 格式化日期时间的转换器。
    /// </summary>
    public class DateTimeFormatConverter : IValueConverter
    {
        /// <summary>
        /// 获取或设置格式字符串。
        /// </summary>
        public string? FormatString { get; set; } = "g";

        /// <summary>
        /// 获取或设置为空时的返回值。
        /// </summary>
        public object? NullValue { get; set; } = null;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return NullValue;

            DateTime dateTime;
            if (value is DateTime dt)
            {
                dateTime = dt;
            }
            else if (value is DateTimeOffset dto)
            {
                dateTime = dto.DateTime;
            }
            else if (value is string str && DateTime.TryParse(str, out dateTime))
            {
                // 已解析
            }
            else
            {
                return value;
            }

            var format = parameter as string ?? FormatString;
            return dateTime.ToString(format, culture);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string str && DateTime.TryParse(str, culture, DateTimeStyles.None, out var result))
            {
                return result;
            }

            return value;
        }
    }

    /// <summary>
    /// 转换对象类型的转换器。
    /// </summary>
    public class ObjectTypeConverter : IValueConverter
    {
        /// <summary>
        /// 获取或设置目标类型。
        /// </summary>
        public Type? TargetType { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var type = parameter as Type ?? TargetType;
            if (type == null)
                return value;

            if (value == null)
            {
                // 检查是否为值类型
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }

            try
            {
                return System.Convert.ChangeType(value, type, culture);
            }
            catch
            {
                return value;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
