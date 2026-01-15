using System.Globalization;
using System.Windows.Data;
using Lemoo.UI.Models;

namespace Lemoo.UI.Converters;

/// <summary>
/// 分类展开状态转换器
/// 用于判断某个分类是否在展开的分类集合中
/// </summary>
public class CategoryExpandedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is HashSet<ControlCategory> expandedCategories && parameter is ControlCategory category)
        {
            return expandedCategories.Contains(category);
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
