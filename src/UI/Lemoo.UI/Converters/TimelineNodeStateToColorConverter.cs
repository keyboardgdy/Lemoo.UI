using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Lemoo.UI.Controls;

namespace Lemoo.UI.Converters
{
    /// <summary>
    /// 时间线节点状态到颜色转换器。
    /// </summary>
    public class TimelineNodeStateToColorConverter : IValueConverter
    {
        /// <summary>
        /// 待处理状态颜色
        /// </summary>
        public Brush PendingBrush { get; set; } = new SolidColorBrush(Color.FromRgb(138, 138, 138));

        /// <summary>
        /// 进行中状态颜色
        /// </summary>
        public Brush InProgressBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0, 122, 212));

        /// <summary>
        /// 已完成状态颜色
        /// </summary>
        public Brush CompletedBrush { get; set; } = new SolidColorBrush(Color.FromRgb(16, 124, 16));

        /// <summary>
        /// 错误状态颜色
        /// </summary>
        public Brush ErrorBrush { get; set; } = new SolidColorBrush(Color.FromRgb(209, 52, 56));

        /// <summary>
        /// 警告状态颜色
        /// </summary>
        public Brush WarningBrush { get; set; } = new SolidColorBrush(Color.FromRgb(255, 140, 0));

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimelineNodeState state)
            {
                return state switch
                {
                    TimelineNodeState.Pending => PendingBrush,
                    TimelineNodeState.InProgress => InProgressBrush,
                    TimelineNodeState.Completed => CompletedBrush,
                    TimelineNodeState.Error => ErrorBrush,
                    TimelineNodeState.Warning => WarningBrush,
                    _ => PendingBrush
                };
            }

            return PendingBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
