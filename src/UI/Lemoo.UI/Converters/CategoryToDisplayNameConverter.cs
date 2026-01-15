using System.Globalization;
using System.Windows.Data;
using Lemoo.UI.Models;
using Lemoo.UI.Services;

namespace Lemoo.UI.Converters;

/// <summary>
/// 控件分类到显示名称的转换器
/// </summary>
public class CategoryToDisplayNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ControlCategory category)
        {
            return ControlRegistry.GetCategoryDisplayName(category);
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
