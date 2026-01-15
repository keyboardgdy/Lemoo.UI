using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lemoo.UI.Converters
{
    /// <summary>
    /// 根据资源键查找静态资源的转换器
    /// </summary>
    public class StaticResourceConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (Application.Current == null)
                return value;

            var resourceKey = value.ToString();
            if (string.IsNullOrEmpty(resourceKey))
                return value;

            // 尝试从应用资源中查找
            var resource = Application.Current.TryFindResource(resourceKey);
            return resource ?? value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 比较两个对象是否相等的转换器
    /// </summary>
    public class EqualityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null && parameter == null)
                return true;

            if (value == null || parameter == null)
                return false;

            return value.Equals(parameter);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
                return parameter;

            return Binding.DoNothing;
        }
    }

    /// <summary>
    /// 反转布尔值并转换为可见性的转换器
    /// </summary>
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; } = Visibility.Collapsed;
        public Visibility FalseValue { get; set; } = Visibility.Visible;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return TrueValue;

            bool boolValue = value is bool b && b;

            return boolValue ? TrueValue : FalseValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == FalseValue;
            }

            return false;
        }
    }
}
