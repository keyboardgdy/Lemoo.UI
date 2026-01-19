using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Lemoo.UI.Controls.Layout
{
    /// <summary>
    /// 分割视图方向
    /// </summary>
    public enum SplitViewOrientation
    {
        /// <summary>
        /// 水平分割
        /// </summary>
        Horizontal,

        /// <summary>
        /// 垂直分割
        /// </summary>
        Vertical
    }

    /// <summary>
    /// 分割器手柄类型
    /// </summary>
    public enum SplitterHandleType
    {
        /// <summary>
        /// 线条
        /// </summary>
        Line,

        /// <summary>
        /// 手柄
        /// </summary>
        Handle,

        /// <summary>
        /// 双箭头
        /// </summary>
        DoubleArrow
    }

    /// <summary>
    /// 增强分割视图控件，支持可拖拽、可嵌套、可折叠的分割面板
    /// </summary>
    public class SplitViewEx : Control
    {
        #region Fields

        private Grid? _mainGrid;
        private FrameworkElement? _splitter;
        private Thumb? _dragThumb;
        private bool _isDragging;
        private Point _startPoint;
        private double _startPanel1Size;
        private double _startPanel2Size;

        #endregion

        #region Constructor

        static SplitViewEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitViewEx),
                new FrameworkPropertyMetadata(typeof(SplitViewEx)));
        }

        public SplitViewEx()
        {
            Loaded += SplitViewEx_Loaded;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 获取或设置第一个面板内容
        /// </summary>
        public object Panel1Content
        {
            get => GetValue(Panel1ContentProperty);
            set => SetValue(Panel1ContentProperty, value);
        }

        public static readonly DependencyProperty Panel1ContentProperty =
            DependencyProperty.Register(nameof(Panel1Content), typeof(object), typeof(SplitViewEx),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置第二个面板内容
        /// </summary>
        public object Panel2Content
        {
            get => GetValue(Panel2ContentProperty);
            set => SetValue(Panel2ContentProperty, value);
        }

        public static readonly DependencyProperty Panel2ContentProperty =
            DependencyProperty.Register(nameof(Panel2Content), typeof(object), typeof(SplitViewEx),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置第一个面板的最小尺寸
        /// </summary>
        public double Panel1MinSize
        {
            get => (double)GetValue(Panel1MinSizeProperty);
            set => SetValue(Panel1MinSizeProperty, value);
        }

        public static readonly DependencyProperty Panel1MinSizeProperty =
            DependencyProperty.Register(nameof(Panel1MinSize), typeof(double), typeof(SplitViewEx),
                new PropertyMetadata(100.0));

        /// <summary>
        /// 获取或设置第二个面板的最小尺寸
        /// </summary>
        public double Panel2MinSize
        {
            get => (double)GetValue(Panel2MinSizeProperty);
            set => SetValue(Panel2MinSizeProperty, value);
        }

        public static readonly DependencyProperty Panel2MinSizeProperty =
            DependencyProperty.Register(nameof(Panel2MinSize), typeof(double), typeof(SplitViewEx),
                new PropertyMetadata(100.0));

        /// <summary>
        /// 获取或设置第一个面板的最大尺寸
        /// </summary>
        public double Panel1MaxSize
        {
            get => (double)GetValue(Panel1MaxSizeProperty);
            set => SetValue(Panel1MaxSizeProperty, value);
        }

        public static readonly DependencyProperty Panel1MaxSizeProperty =
            DependencyProperty.Register(nameof(Panel1MaxSize), typeof(double), typeof(SplitViewEx),
                new PropertyMetadata(double.PositiveInfinity));

        /// <summary>
        /// 获取或设置第二个面板的最大尺寸
        /// </summary>
        public double Panel2MaxSize
        {
            get => (double)GetValue(Panel2MaxSizeProperty);
            set => SetValue(Panel2MaxSizeProperty, value);
        }

        public static readonly DependencyProperty Panel2MaxSizeProperty =
            DependencyProperty.Register(nameof(Panel2MaxSize), typeof(double), typeof(SplitViewEx),
                new PropertyMetadata(double.PositiveInfinity));

        /// <summary>
        /// 获取或设置分割方向
        /// </summary>
        public SplitViewOrientation Orientation
        {
            get => (SplitViewOrientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(SplitViewOrientation), typeof(SplitViewEx),
                new PropertyMetadata(SplitViewOrientation.Horizontal, OnOrientationChanged));

        /// <summary>
        /// 获取或设置分割器厚度
        /// </summary>
        public double SplitterThickness
        {
            get => (double)GetValue(SplitterThicknessProperty);
            set => SetValue(SplitterThicknessProperty, value);
        }

        public static readonly DependencyProperty SplitterThicknessProperty =
            DependencyProperty.Register(nameof(SplitterThickness), typeof(double), typeof(SplitViewEx),
                new PropertyMetadata(4.0));

        /// <summary>
        /// 获取或设置分割器悬停厚度
        /// </summary>
        public double SplitterHoverThickness
        {
            get => (double)GetValue(SplitterHoverThicknessProperty);
            set => SetValue(SplitterHoverThicknessProperty, value);
        }

        public static readonly DependencyProperty SplitterHoverThicknessProperty =
            DependencyProperty.Register(nameof(SplitterHoverThickness), typeof(double), typeof(SplitViewEx),
                new PropertyMetadata(8.0));

        /// <summary>
        /// 获取或设置分割器手柄类型
        /// </summary>
        public SplitterHandleType HandleType
        {
            get => (SplitterHandleType)GetValue(HandleTypeProperty);
            set => SetValue(HandleTypeProperty, value);
        }

        public static readonly DependencyProperty HandleTypeProperty =
            DependencyProperty.Register(nameof(HandleType), typeof(SplitterHandleType), typeof(SplitViewEx),
                new PropertyMetadata(SplitterHandleType.Handle));

        /// <summary>
        /// 获取或设置分割器颜色
        /// </summary>
        public Brush SplitterBrush
        {
            get => (Brush)GetValue(SplitterBrushProperty);
            set => SetValue(SplitterBrushProperty, value);
        }

        public static readonly DependencyProperty SplitterBrushProperty =
            DependencyProperty.Register(nameof(SplitterBrush), typeof(Brush), typeof(SplitViewEx),
                new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// 获取或设置分割器悬停颜色
        /// </summary>
        public Brush SplitterHoverBrush
        {
            get => (Brush)GetValue(SplitterHoverBrushProperty);
            set => SetValue(SplitterHoverBrushProperty, value);
        }

        public static readonly DependencyProperty SplitterHoverBrushProperty =
            DependencyProperty.Register(nameof(SplitterHoverBrush), typeof(Brush), typeof(SplitViewEx),
                new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// 获取或设置第一个面板是否可折叠
        /// </summary>
        public bool IsPanel1Collapsible
        {
            get => (bool)GetValue(IsPanel1CollapsibleProperty);
            set => SetValue(IsPanel1CollapsibleProperty, value);
        }

        public static readonly DependencyProperty IsPanel1CollapsibleProperty =
            DependencyProperty.Register(nameof(IsPanel1Collapsible), typeof(bool), typeof(SplitViewEx),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置第二个面板是否可折叠
        /// </summary>
        public bool IsPanel2Collapsible
        {
            get => (bool)GetValue(IsPanel2CollapsibleProperty);
            set => SetValue(IsPanel2CollapsibleProperty, value);
        }

        public static readonly DependencyProperty IsPanel2CollapsibleProperty =
            DependencyProperty.Register(nameof(IsPanel2Collapsible), typeof(bool), typeof(SplitViewEx),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置第一个面板是否展开
        /// </summary>
        public bool IsPanel1Expanded
        {
            get => (bool)GetValue(IsPanel1ExpandedProperty);
            set => SetValue(IsPanel1ExpandedProperty, value);
        }

        public static readonly DependencyProperty IsPanel1ExpandedProperty =
            DependencyProperty.Register(nameof(IsPanel1Expanded), typeof(bool), typeof(SplitViewEx),
                new PropertyMetadata(true, OnPanel1ExpandedChanged));

        /// <summary>
        /// 获取或设置第二个面板是否展开
        /// </summary>
        public bool IsPanel2Expanded
        {
            get => (bool)GetValue(IsPanel2ExpandedProperty);
            set => SetValue(IsPanel2ExpandedProperty, value);
        }

        public static readonly DependencyProperty IsPanel2ExpandedProperty =
            DependencyProperty.Register(nameof(IsPanel2Expanded), typeof(bool), typeof(SplitViewEx),
                new PropertyMetadata(true, OnPanel2ExpandedChanged));

        /// <summary>
        /// 获取或设置分割比例（0-1之间）
        /// </summary>
        public double SplitRatio
        {
            get => (double)GetValue(SplitRatioProperty);
            set => SetValue(SplitRatioProperty, value);
        }

        public static readonly DependencyProperty SplitRatioProperty =
            DependencyProperty.Register(nameof(SplitRatio), typeof(double), typeof(SplitViewEx),
                new PropertyMetadata(0.5, OnSplitRatioChanged));

        /// <summary>
        /// 是否可调整大小
        /// </summary>
        public bool CanResize
        {
            get => (bool)GetValue(CanResizeProperty);
            set => SetValue(CanResizeProperty, value);
        }

        public static readonly DependencyProperty CanResizeProperty =
            DependencyProperty.Register(nameof(CanResize), typeof(bool), typeof(SplitViewEx),
                new PropertyMetadata(true));

        /// <summary>
        /// 分割调整完成命令
        /// </summary>
        public ICommand SplitChangedCommand
        {
            get => (ICommand)GetValue(SplitChangedCommandProperty);
            set => SetValue(SplitChangedCommandProperty, value);
        }

        public static readonly DependencyProperty SplitChangedCommandProperty =
            DependencyProperty.Register(nameof(SplitChangedCommand), typeof(ICommand), typeof(SplitViewEx),
                new PropertyMetadata(null));

        #endregion

        #region Event Handlers

        private void SplitViewEx_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSplitViewLayout();
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SplitViewEx splitView)
            {
                splitView.UpdateSplitViewLayout();
            }
        }

        private static void OnPanel1ExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SplitViewEx splitView)
            {
                splitView.UpdatePanelVisibility();
            }
        }

        private static void OnPanel2ExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SplitViewEx splitView)
            {
                splitView.UpdatePanelVisibility();
            }
        }

        private static void OnSplitRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SplitViewEx splitView)
            {
                splitView.UpdateSplitterPosition();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _mainGrid = GetTemplateChild("PART_MainGrid") as Grid;
            _splitter = GetTemplateChild("PART_Splitter") as FrameworkElement;
            _dragThumb = GetTemplateChild("PART_DragThumb") as Thumb;

            if (_dragThumb != null)
            {
                _dragThumb.DragStarted += OnDragStarted;
                _dragThumb.DragDelta += OnDragDelta;
                _dragThumb.DragCompleted += OnDragCompleted;
            }

            UpdateSplitViewLayout();
        }

        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            _isDragging = true;
            _startPoint = Mouse.GetPosition(this);

            if (Orientation == SplitViewOrientation.Horizontal)
            {
                _startPanel1Size = _mainGrid?.ColumnDefinitions[0].ActualWidth ?? 0;
                _startPanel2Size = _mainGrid?.ColumnDefinitions[2].ActualWidth ?? 0;
            }
            else
            {
                _startPanel1Size = _mainGrid?.RowDefinitions[0].ActualHeight ?? 0;
                _startPanel2Size = _mainGrid?.RowDefinitions[2].ActualHeight ?? 0;
            }
        }

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!_isDragging || _mainGrid == null) return;

            Point currentPoint = Mouse.GetPosition(this);
            double delta = 0;

            if (Orientation == SplitViewOrientation.Horizontal)
            {
                delta = currentPoint.X - _startPoint.X;
                double newPanel1Size = _startPanel1Size + delta;
                double newPanel2Size = _startPanel2Size - delta;

                // 检查最小/最大尺寸限制
                if (newPanel1Size >= Panel1MinSize && newPanel1Size <= Panel1MaxSize &&
                    newPanel2Size >= Panel2MinSize && newPanel2Size <= Panel2MaxSize)
                {
                    _mainGrid.ColumnDefinitions[0].Width = new GridLength(newPanel1Size);
                    _mainGrid.ColumnDefinitions[2].Width = new GridLength(newPanel2Size);

                    // 更新分割比例
                    double totalSize = newPanel1Size + newPanel2Size;
                    if (totalSize > 0)
                    {
                        SplitRatio = newPanel1Size / totalSize;
                    }
                }
            }
            else
            {
                delta = currentPoint.Y - _startPoint.Y;
                double newPanel1Size = _startPanel1Size + delta;
                double newPanel2Size = _startPanel2Size - delta;

                // 检查最小/最大尺寸限制
                if (newPanel1Size >= Panel1MinSize && newPanel1Size <= Panel1MaxSize &&
                    newPanel2Size >= Panel2MinSize && newPanel2Size <= Panel2MaxSize)
                {
                    _mainGrid.RowDefinitions[0].Height = new GridLength(newPanel1Size);
                    _mainGrid.RowDefinitions[2].Height = new GridLength(newPanel2Size);

                    // 更新分割比例
                    double totalSize = newPanel1Size + newPanel2Size;
                    if (totalSize > 0)
                    {
                        SplitRatio = newPanel1Size / totalSize;
                    }
                }
            }

            e.Handled = true;
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            _isDragging = false;
            SplitChangedCommand?.Execute(SplitRatio);
            e.Handled = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 折叠第一个面板
        /// </summary>
        public void CollapsePanel1()
        {
            IsPanel1Expanded = false;
        }

        /// <summary>
        /// 展开第一个面板
        /// </summary>
        public void ExpandPanel1()
        {
            IsPanel1Expanded = true;
        }

        /// <summary>
        /// 折叠第二个面板
        /// </summary>
        public void CollapsePanel2()
        {
            IsPanel2Expanded = false;
        }

        /// <summary>
        /// 展开第二个面板
        /// </summary>
        public void ExpandPanel2()
        {
            IsPanel2Expanded = true;
        }

        /// <summary>
        /// 切换第一个面板的展开/折叠状态
        /// </summary>
        public void TogglePanel1()
        {
            IsPanel1Expanded = !IsPanel1Expanded;
        }

        /// <summary>
        /// 切换第二个面板的展开/折叠状态
        /// </summary>
        public void TogglePanel2()
        {
            IsPanel2Expanded = !IsPanel2Expanded;
        }

        /// <summary>
        /// 重置分割比例
        /// </summary>
        public void ResetSplitRatio()
        {
            SplitRatio = 0.5;
        }

        #endregion

        #region Private Methods

        private void UpdateSplitViewLayout()
        {
            if (_mainGrid == null) return;

            if (Orientation == SplitViewOrientation.Horizontal)
            {
                _mainGrid.RowDefinitions.Clear();
                _mainGrid.ColumnDefinitions.Clear();
                _mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                _mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                _mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
            else
            {
                _mainGrid.RowDefinitions.Clear();
                _mainGrid.ColumnDefinitions.Clear();
                _mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                _mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                _mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }

            UpdateSplitterPosition();
            UpdatePanelVisibility();
        }

        private void UpdateSplitterPosition()
        {
            if (_mainGrid == null) return;

            double ratio = Math.Clamp(SplitRatio, 0.0, 1.0);

            if (Orientation == SplitViewOrientation.Horizontal)
            {
                double totalWidth = _mainGrid.ActualWidth - SplitterThickness;
                if (totalWidth > 0)
                {
                    double panel1Width = totalWidth * ratio;
                    double panel2Width = totalWidth * (1 - ratio);

                    _mainGrid.ColumnDefinitions[0].Width = new GridLength(panel1Width);
                    _mainGrid.ColumnDefinitions[2].Width = new GridLength(panel2Width);
                }
            }
            else
            {
                double totalHeight = _mainGrid.ActualHeight - SplitterThickness;
                if (totalHeight > 0)
                {
                    double panel1Height = totalHeight * ratio;
                    double panel2Height = totalHeight * (1 - ratio);

                    _mainGrid.RowDefinitions[0].Height = new GridLength(panel1Height);
                    _mainGrid.RowDefinitions[2].Height = new GridLength(panel2Height);
                }
            }
        }

        private void UpdatePanelVisibility()
        {
            if (_mainGrid == null) return;

            if (Orientation == SplitViewOrientation.Horizontal)
            {
                _mainGrid.ColumnDefinitions[0].Width = IsPanel1Expanded
                    ? new GridLength(_mainGrid.ColumnDefinitions[0].ActualWidth > 0 ? _mainGrid.ColumnDefinitions[0].ActualWidth : 1, GridUnitType.Star)
                    : new GridLength(0);
                _mainGrid.ColumnDefinitions[2].Width = IsPanel2Expanded
                    ? new GridLength(_mainGrid.ColumnDefinitions[2].ActualWidth > 0 ? _mainGrid.ColumnDefinitions[2].ActualWidth : 1, GridUnitType.Star)
                    : new GridLength(0);
            }
            else
            {
                _mainGrid.RowDefinitions[0].Height = IsPanel1Expanded
                    ? new GridLength(_mainGrid.RowDefinitions[0].ActualHeight > 0 ? _mainGrid.RowDefinitions[0].ActualHeight : 1, GridUnitType.Star)
                    : new GridLength(0);
                _mainGrid.RowDefinitions[2].Height = IsPanel2Expanded
                    ? new GridLength(_mainGrid.RowDefinitions[2].ActualHeight > 0 ? _mainGrid.RowDefinitions[2].ActualHeight : 1, GridUnitType.Star)
                    : new GridLength(0);
            }
        }

        #endregion
    }
}
