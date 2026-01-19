using System.Windows;
using System.Windows.Media;

namespace Lemoo.UI.Models.DataVisualization
{
    /// <summary>
    /// 图表类型枚举。
    /// </summary>
    public enum ChartType
    {
        /// <summary>
        /// 折线图
        /// </summary>
        Line,

        /// <summary>
        /// 柱状图
        /// </summary>
        Bar,

        /// <summary>
        /// 饼图
        /// </summary>
        Pie,

        /// <summary>
        /// 环形图
        /// </summary>
        Doughnut,

        /// <summary>
        /// 面积图
        /// </summary>
        Area,

        /// <summary>
        /// 散点图
        /// </summary>
        Scatter
    }

    /// <summary>
    /// 图表数据点。
    /// </summary>
    public class ChartDataPoint : DependencyObject
    {
        #region Label 依赖属性

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(
                nameof(Label),
                typeof(string),
                typeof(ChartDataPoint),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置数据点标签。
        /// </summary>
        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        #endregion

        #region Value 依赖属性

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(double),
                typeof(ChartDataPoint),
                new PropertyMetadata(0.0));

        /// <summary>
        /// 获取或设置数据点值。
        /// </summary>
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        #endregion

        #region Color 依赖属性

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                nameof(Color),
                typeof(Brush),
                typeof(ChartDataPoint),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置数据点颜色。
        /// </summary>
        public Brush? Color
        {
            get => (Brush?)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        #endregion

        #region Tag 依赖属性

        public static readonly DependencyProperty TagProperty =
            DependencyProperty.Register(
                nameof(Tag),
                typeof(object),
                typeof(ChartDataPoint),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置关联的自定义数据。
        /// </summary>
        public object? Tag
        {
            get => GetValue(TagProperty);
            set => SetValue(TagProperty, value);
        }

        #endregion
    }

    /// <summary>
    /// 图表数据系列。
    /// </summary>
    public class ChartSeries : DependencyObject
    {
        #region Title 依赖属性

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(ChartSeries),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置系列标题。
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion

        #region Color 依赖属性

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                nameof(Color),
                typeof(Brush),
                typeof(ChartSeries),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置系列颜色。
        /// </summary>
        public Brush? Color
        {
            get => (Brush?)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        #endregion
    }
}
