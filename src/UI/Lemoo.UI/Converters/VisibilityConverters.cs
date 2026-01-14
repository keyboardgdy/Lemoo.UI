using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lemoo.UI.Converters
{
    /// <summary>
    /// 将集合计数转换为可见性的转换器。
    /// </summary>
    public class CountToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 获取或设置大于等于此数量时可见。
        /// </summary>
        public int MinCount { get; set; }

        /// <summary>
        /// 获取或设置为 0 时是否可见（反转逻辑）。
        /// </summary>
        public bool Invert { get; set; }

        /// <summary>
        /// 获取或设置不可见时的返回值。
        /// </summary>
        public Visibility HiddenValue { get; set; } = Visibility.Collapsed;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            int count = 0;

            if (value is System.Collections.ICollection collection)
            {
                count = collection.Count;
            }
            else if (value is int i)
            {
                count = i;
            }

            bool visible = Invert ? count == 0 : count > MinCount;

            return visible ? Visibility.Visible : HiddenValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 将空字符串转换为可见性的转换器。
    /// </summary>
    public class StringEmptyToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 获取或设置为空时是否可见（反转逻辑）。
        /// </summary>
        public bool Invert { get; set; }

        /// <summary>
        /// 获取或设置不可见时的返回值。
        /// </summary>
        public Visibility HiddenValue { get; set; } = Visibility.Collapsed;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string? str = value as string;
            bool isEmpty = string.IsNullOrEmpty(str);
            bool visible = Invert ? isEmpty : !isEmpty;

            return visible ? Visibility.Visible : HiddenValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 将 null 值转换为可见性的转换器。
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 获取或设置为 null 时是否可见。
        /// </summary>
        public bool NullVisible { get; set; }

        /// <summary>
        /// 获取或设置不可见时的返回值。
        /// </summary>
        public Visibility HiddenValue { get; set; } = Visibility.Collapsed;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool visible = value == null ? NullVisible : !NullVisible;
            return visible ? Visibility.Visible : HiddenValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
