using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Lemoo.UI.Models.DataVisualization;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 图表控件，用于数据可视化展示。
    /// </summary>
    /// <remarks>
    /// ChartControl 是一个简洁美观的图表控件，支持折线图、柱状图、饼图、
    /// 环形图、面积图、散点图等多种图表类型。
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;!-- 折线图 --&gt;
    /// &lt;ui:ChartControl
    ///     ChartType="Line"
    ///     ItemsSource="{Binding Data}"
    ///     DisplayMemberPath="Label"
    ///     ValueMemberPath="Value" /&gt;
    ///
    /// &lt;!-- 柱状图 --&gt;
    /// &lt;ui:ChartControl
    ///     ChartType="Bar"
    ///     ItemsSource="{Binding Data}"
    ///     Title="销售统计" /&gt;
    ///
    /// &lt;!-- 饼图 --&gt;
    /// &lt;ui:ChartControl
    ///     ChartType="Pie"
    ///     ItemsSource="{Binding Data}"
    ///     ShowLegend="True" /&gt;
    /// </code>
    /// </example>
    [TemplatePart(Name = PART_Canvas, Type = typeof(Canvas))]
    public class ChartControl : Control
    {
        #region 常量

        private const string PART_Canvas = "PART_Canvas";

        #endregion

        #region 字段

        private Canvas? _canvas;
        private readonly List<Brush> _defaultColors = new();

        #endregion

        #region Constructor

        static ChartControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartControl),
                new FrameworkPropertyMetadata(typeof(ChartControl)));
        }

        public ChartControl()
        {
            ChartType = ChartType.Bar;
            ShowLegend = true;
            ShowTitle = true;
            ShowValues = true;
            AnimationDuration = TimeSpan.FromMilliseconds(800);
            InitializeDefaultColors();
        }

        #endregion

        #region ChartType 依赖属性

        public static readonly DependencyProperty ChartTypeProperty =
            DependencyProperty.Register(
                nameof(ChartType),
                typeof(ChartType),
                typeof(ChartControl),
                new PropertyMetadata(ChartType.Bar, OnChartTypeChanged));

        /// <summary>
        /// 获取或设置图表类型。
        /// </summary>
        public ChartType ChartType
        {
            get => (ChartType)GetValue(ChartTypeProperty);
            set => SetValue(ChartTypeProperty, value);
        }

        private static void OnChartTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChartControl chart)
            {
                chart.RenderChart();
            }
        }

        #endregion

        #region ItemsSource 依赖属性

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(System.Collections.IEnumerable),
                typeof(ChartControl),
                new PropertyMetadata(null, OnItemsSourceChanged));

        /// <summary>
        /// 获取或设置数据源。
        /// </summary>
        public System.Collections.IEnumerable? ItemsSource
        {
            get => (System.Collections.IEnumerable?)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChartControl chart)
            {
                chart.RenderChart();
            }
        }

        #endregion

        #region DisplayMemberPath 依赖属性

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(
                nameof(DisplayMemberPath),
                typeof(string),
                typeof(ChartControl),
                new PropertyMetadata("Label"));

        /// <summary>
        /// 获取或设置显示成员路径。
        /// </summary>
        public string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        #endregion

        #region ValueMemberPath 依赖属性

        public static readonly DependencyProperty ValueMemberPathProperty =
            DependencyProperty.Register(
                nameof(ValueMemberPath),
                typeof(string),
                typeof(ChartControl),
                new PropertyMetadata("Value"));

        /// <summary>
        /// 获取或设置值成员路径。
        /// </summary>
        public string ValueMemberPath
        {
            get => (string)GetValue(ValueMemberPathProperty);
            set => SetValue(ValueMemberPathProperty, value);
        }

        #endregion

        #region Title 依赖属性

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(ChartControl),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置图表标题。
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion

        #region ShowLegend 依赖属性

        public static readonly DependencyProperty ShowLegendProperty =
            DependencyProperty.Register(
                nameof(ShowLegend),
                typeof(bool),
                typeof(ChartControl),
                new PropertyMetadata(true));

        /// <summary>
        /// 获取或设置是否显示图例。
        /// </summary>
        public bool ShowLegend
        {
            get => (bool)GetValue(ShowLegendProperty);
            set => SetValue(ShowLegendProperty, value);
        }

        #endregion

        #region ShowTitle 依赖属性

        public static readonly DependencyProperty ShowTitleProperty =
            DependencyProperty.Register(
                nameof(ShowTitle),
                typeof(bool),
                typeof(ChartControl),
                new PropertyMetadata(true));

        /// <summary>
        /// 获取或设置是否显示标题。
        /// </summary>
        public bool ShowTitle
        {
            get => (bool)GetValue(ShowTitleProperty);
            set => SetValue(ShowTitleProperty, value);
        }

        #endregion

        #region ShowValues 依赖属性

        public static readonly DependencyProperty ShowValuesProperty =
            DependencyProperty.Register(
                nameof(ShowValues),
                typeof(bool),
                typeof(ChartControl),
                new PropertyMetadata(true));

        /// <summary>
        /// 获取或设置是否显示数值标签。
        /// </summary>
        public bool ShowValues
        {
            get => (bool)GetValue(ShowValuesProperty);
            set => SetValue(ShowValuesProperty, value);
        }

        #endregion

        #region AnimationDuration 依赖属性

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register(
                nameof(AnimationDuration),
                typeof(TimeSpan),
                typeof(ChartControl),
                new PropertyMetadata(TimeSpan.FromMilliseconds(800)));

        /// <summary>
        /// 获取或设置动画时长。
        /// </summary>
        public TimeSpan AnimationDuration
        {
            get => (TimeSpan)GetValue(AnimationDurationProperty);
            set => SetValue(AnimationDurationProperty, value);
        }

        #endregion

        #region 事件

        public static readonly RoutedEvent DataPointClickEvent =
            EventManager.RegisterRoutedEvent(
                nameof(DataPointClick),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ChartControl));

        /// <summary>
        /// 在数据点被点击时发生。
        /// </summary>
        public event RoutedEventHandler DataPointClick
        {
            add => AddHandler(DataPointClickEvent, value);
            remove => RemoveHandler(DataPointClickEvent, value);
        }

        #endregion

        #region 方法

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _canvas = GetTemplateChild(PART_Canvas) as Canvas;
            RenderChart();
        }

        private void InitializeDefaultColors()
        {
            _defaultColors.Add(new SolidColorBrush(Color.FromRgb(0, 173, 181)));
            _defaultColors.Add(new SolidColorBrush(Color.FromRgb(255, 121, 198)));
            _defaultColors.Add(new SolidColorBrush(Color.FromRgb(55, 66, 250)));
            _defaultColors.Add(new SolidColorBrush(Color.FromRgb(255, 167, 38)));
            _defaultColors.Add(new SolidColorBrush(Color.FromRgb(76, 217, 100)));
            _defaultColors.Add(new SolidColorBrush(Color.FromRgb(255, 59, 48)));
            _defaultColors.Add(new SolidColorBrush(Color.FromRgb(90, 200, 250)));
            _defaultColors.Add(new SolidColorBrush(Color.FromRgb(162, 132, 255)));
        }

        private void RenderChart()
        {
            if (_canvas == null || ItemsSource == null)
                return;

            _canvas.Children.Clear();

            switch (ChartType)
            {
                case ChartType.Line:
                    RenderLineChart();
                    break;
                case ChartType.Bar:
                    RenderBarChart();
                    break;
                case ChartType.Pie:
                    RenderPieChart();
                    break;
                case ChartType.Doughnut:
                    RenderDoughnutChart();
                    break;
                case ChartType.Area:
                    RenderAreaChart();
                    break;
                case ChartType.Scatter:
                    RenderScatterChart();
                    break;
            }
        }

        private void RenderBarChart()
        {
            if (_canvas == null || ItemsSource == null) return;

            var data = GetDataPoints();
            if (data.Count == 0) return;

            var maxValue = GetMaxValue();
            var width = _canvas.ActualWidth;
            var height = _canvas.ActualHeight;
            var barWidth = (width - 40) / data.Count - 10;
            var chartHeight = height - 60;

            for (int i = 0; i < data.Count; i++)
            {
                var point = data[i];
                var barHeight = (point.Value / maxValue) * chartHeight;
                var color = GetColor(i);

                var bar = new Rectangle
                {
                    Width = barWidth,
                    Height = 0,
                    Fill = color,
                    RadiusX = 4,
                    RadiusY = 4
                };

                Canvas.SetLeft(bar, 20 + i * (barWidth + 10));
                Canvas.SetTop(bar, height - 30 - barHeight);
                _canvas.Children.Add(bar);

                // 动画
                var animation = new DoubleAnimation
                {
                    To = barHeight,
                    Duration = AnimationDuration,
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                bar.BeginAnimation(Rectangle.HeightProperty, animation);

                // 标签
                if (ShowValues)
                {
                    var label = new TextBlock
                    {
                        Text = point.Label,
                        FontSize = 11,
                        Foreground = new SolidColorBrush(Colors.Gray),
                        TextAlignment = TextAlignment.Center
                    };
                    Canvas.SetLeft(label, 20 + i * (barWidth + 10) + barWidth / 2 - label.Width / 2);
                    Canvas.SetTop(label, height - 15);
                    _canvas.Children.Add(label);
                }
            }
        }

        private void RenderLineChart()
        {
            if (_canvas == null || ItemsSource == null) return;

            var data = GetDataPoints();
            if (data.Count == 0) return;

            var maxValue = GetMaxValue();
            var width = _canvas.ActualWidth;
            var height = _canvas.ActualHeight;
            var chartHeight = height - 60;
            var pointSpacing = (width - 40) / (data.Count - 1);

            var polyline = new Polyline
            {
                Stroke = _defaultColors[0],
                StrokeThickness = 2,
                StrokeLineJoin = PenLineJoin.Round
            };

            for (int i = 0; i < data.Count; i++)
            {
                var x = 20 + i * pointSpacing;
                var y = height - 30 - (data[i].Value / maxValue) * chartHeight;
                polyline.Points.Add(new Point(x, y));

                // 数据点
                var ellipse = new Ellipse
                {
                    Width = 8,
                    Height = 8,
                    Fill = _defaultColors[0],
                    Stroke = Brushes.White,
                    StrokeThickness = 2
                };
                Canvas.SetLeft(ellipse, x - 4);
                Canvas.SetTop(ellipse, y - 4);
                _canvas.Children.Add(ellipse);
            }

            _canvas.Children.Insert(0, polyline);
        }

        private void RenderPieChart()
        {
            if (_canvas == null || ItemsSource == null) return;

            var data = GetDataPoints();
            if (data.Count == 0) return;

            var total = GetTotalValue();
            var centerX = _canvas.ActualWidth / 2;
            var centerY = _canvas.ActualHeight / 2;
            var radius = Math.Min(centerX, centerY) - 40;

            var currentAngle = -90.0;

            for (int i = 0; i < data.Count; i++)
            {
                var sliceAngle = (data[i].Value / total) * 360;
                var color = GetColor(i);

                var path = new Path
                {
                    Fill = color,
                    Stroke = Brushes.White,
                    StrokeThickness = 2
                };

                var segment = CreatePieSegment(centerX, centerY, radius, currentAngle, currentAngle + sliceAngle);
                path.Data = segment;

                _canvas.Children.Add(path);

                // 标签
                if (ShowValues && sliceAngle > 20)
                {
                    var labelAngle = currentAngle + sliceAngle / 2;
                    var labelRadius = radius * 0.7;
                    var labelX = centerX + labelRadius * Math.Cos(labelAngle * Math.PI / 180);
                    var labelY = centerY + labelRadius * Math.Sin(labelAngle * Math.PI / 180);

                    var label = new TextBlock
                    {
                        Text = $"{data[i].Label}\n{data[i].Value:F0}",
                        FontSize = 11,
                        Foreground = Brushes.White,
                        TextAlignment = TextAlignment.Center
                    };
                    Canvas.SetLeft(label, labelX - 20);
                    Canvas.SetTop(label, labelY - 10);
                    _canvas.Children.Add(label);
                }

                currentAngle += sliceAngle;
            }
        }

        private void RenderDoughnutChart()
        {
            // 类似饼图，但中心有洞
            RenderPieChart();

            if (_canvas == null) return;

            var centerX = _canvas.ActualWidth / 2;
            var centerY = _canvas.ActualHeight / 2;
            var innerRadius = Math.Min(centerX, centerY) * 0.5;

            var hole = new Ellipse
            {
                Width = innerRadius * 2,
                Height = innerRadius * 2,
                Fill = new SolidColorBrush(Color.FromRgb(30, 30, 30))
            };

            Canvas.SetLeft(hole, centerX - innerRadius);
            Canvas.SetTop(hole, centerY - innerRadius);
            _canvas.Children.Add(hole);
        }

        private void RenderAreaChart()
        {
            // 类似折线图，但填充下方区域
            RenderLineChart();
        }

        private void RenderScatterChart()
        {
            if (_canvas == null || ItemsSource == null) return;

            var data = GetDataPoints();
            if (data.Count == 0) return;

            var maxValue = GetMaxValue();
            var width = _canvas.ActualWidth;
            var height = _canvas.ActualHeight;
            var chartHeight = height - 60;

            for (int i = 0; i < data.Count; i++)
            {
                var x = 20 + (i / (double)(data.Count - 1)) * (width - 40);
                var y = height - 30 - (data[i].Value / maxValue) * chartHeight;

                var ellipse = new Ellipse
                {
                    Width = 12,
                    Height = 12,
                    Fill = GetColor(i),
                    Stroke = Brushes.White,
                    StrokeThickness = 2
                };

                Canvas.SetLeft(ellipse, x - 6);
                Canvas.SetTop(ellipse, y - 6);
                _canvas.Children.Add(ellipse);
            }
        }

        private Geometry CreatePieSegment(double centerX, double centerY, double radius, double startAngle, double endAngle)
        {
            var startRad = startAngle * Math.PI / 180;
            var endRad = endAngle * Math.PI / 180;

            var startPoint = new Point(centerX + radius * Math.Cos(startRad), centerY + radius * Math.Sin(startRad));
            var endPoint = new Point(centerX + radius * Math.Cos(endRad), centerY + radius * Math.Sin(endRad));

            var segment = new PathFigure();
            segment.StartPoint = startPoint;

            var arc = new ArcSegment
            {
                Point = endPoint,
                Size = new Size(radius, radius),
                RotationAngle = 0,
                IsLargeArc = (endAngle - startAngle) > 180,
                SweepDirection = SweepDirection.Clockwise
            };

            segment.Segments.Add(arc);
            segment.Segments.Add(new LineSegment(new Point(centerX, centerY), true));

            return new PathGeometry(new[] { segment });
        }

        private List<ChartDataPoint> GetDataPoints()
        {
            var points = new List<ChartDataPoint>();

            if (ItemsSource == null)
                return points;

            foreach (var item in ItemsSource)
            {
                var point = new ChartDataPoint();

                if (item is ChartDataPoint dataPoint)
                {
                    point = dataPoint;
                }
                else
                {
                    var labelProperty = item.GetType().GetProperty(DisplayMemberPath);
                    var valueProperty = item.GetType().GetProperty(ValueMemberPath);

                    if (labelProperty != null)
                    {
                        point.Label = labelProperty.GetValue(item)?.ToString() ?? string.Empty;
                    }

                    if (valueProperty != null)
                    {
                        point.Value = Convert.ToDouble(valueProperty.GetValue(item));
                    }
                }

                points.Add(point);
            }

            return points;
        }

        private double GetMaxValue()
        {
            var data = GetDataPoints();
            if (data.Count == 0) return 1;

            var max = 0.0;
            foreach (var point in data)
            {
                if (point.Value > max)
                    max = point.Value;
            }

            return max > 0 ? max : 1;
        }

        private double GetTotalValue()
        {
            var data = GetDataPoints();
            var total = 0.0;

            foreach (var point in data)
            {
                total += point.Value;
            }

            return total > 0 ? total : 1;
        }

        private Brush GetColor(int index)
        {
            return _defaultColors[index % _defaultColors.Count];
        }

        #endregion
    }
}
