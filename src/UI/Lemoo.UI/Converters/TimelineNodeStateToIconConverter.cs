using System;
using System.Globalization;
using System.Windows.Data;
using Lemoo.UI.Controls;

namespace Lemoo.UI.Converters
{
    /// <summary>
    /// 时间线节点状态到图标转换器。
    /// </summary>
    public class TimelineNodeStateToIconConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimelineNodeState state)
            {
                return state switch
                {
                    TimelineNodeState.Pending => "",
                    TimelineNodeState.InProgress => "",
                    TimelineNodeState.Completed => "",
                    TimelineNodeState.Error => "",
                    TimelineNodeState.Warning => "",
                    _ => ""
                };
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
