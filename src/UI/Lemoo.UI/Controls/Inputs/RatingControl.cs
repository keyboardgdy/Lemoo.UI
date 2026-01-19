using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Lemoo.UI.Controls.Inputs
{
    /// <summary>
    /// 评分图标类型
    /// </summary>
    public enum RatingIconType
    {
        /// <summary>
        /// 星形
        /// </summary>
        Star,

        /// <summary>
        /// 心形
        /// </summary>
        Heart,

        /// <summary>
        /// 点赞
        /// </summary>
        Thumb,

        /// <summary>
        /// 表情
        /// </summary>
        Emoji,

        /// <summary>
        /// 自定义
        /// </summary>
        Custom
    }

    /// <summary>
    /// 填充模式
    /// </summary>
    public enum RatingFillMode
    {
        /// <summary>
        /// 完全填充
        /// </summary>
        Full,

        /// <summary>
        /// 部分填充
        /// </summary>
        Partial,

        /// <summary>
        /// 精确填充
        /// </summary>
        Precise
    }

    /// <summary>
    /// 评分控件，支持多种图标、分数精度和自定义填充
    /// </summary>
    public class RatingControl : Control
    {
        #region Fields

        private ObservableCollection<RatingItem> _items;
        private bool _isPreviewing;

        #endregion

        #region Constructor

        static RatingControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RatingControl),
                new FrameworkPropertyMetadata(typeof(RatingControl)));
        }

        public RatingControl()
        {
            _items = new ObservableCollection<RatingItem>();
            UpdateItems();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 获取或设置当前评分值
        /// </summary>
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(RatingControl),
                new PropertyMetadata(0.0, OnValueChanged));

        /// <summary>
        /// 获取或设置最大评分值
        /// </summary>
        public int MaxValue
        {
            get => (int)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(int), typeof(RatingControl),
                new PropertyMetadata(5, OnMaxValueChanged));

        /// <summary>
        /// 获取或设置评分精度
        /// </summary>
        public double Precision
        {
            get => (double)GetValue(PrecisionProperty);
            set => SetValue(PrecisionProperty, value);
        }

        public static readonly DependencyProperty PrecisionProperty =
            DependencyProperty.Register(nameof(Precision), typeof(double), typeof(RatingControl),
                new PropertyMetadata(1.0));

        /// <summary>
        /// 获取或设置图标类型
        /// </summary>
        public RatingIconType IconType
        {
            get => (RatingIconType)GetValue(IconTypeProperty);
            set => SetValue(IconTypeProperty, value);
        }

        public static readonly DependencyProperty IconTypeProperty =
            DependencyProperty.Register(nameof(IconType), typeof(RatingIconType), typeof(RatingControl),
                new PropertyMetadata(RatingIconType.Star, OnIconTypeChanged));

        /// <summary>
        /// 获取或设置填充模式
        /// </summary>
        public RatingFillMode FillMode
        {
            get => (RatingFillMode)GetValue(FillModeProperty);
            set => SetValue(FillModeProperty, value);
        }

        public static readonly DependencyProperty FillModeProperty =
            DependencyProperty.Register(nameof(FillMode), typeof(RatingFillMode), typeof(RatingControl),
                new PropertyMetadata(RatingFillMode.Full));

        /// <summary>
        /// 获取或设置图标大小
        /// </summary>
        double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(RatingControl),
                new PropertyMetadata(24.0));

        /// <summary>
        /// 获取或设置未激活颜色
        /// </summary>
        public Brush InactiveColor
        {
            get => (Brush)GetValue(InactiveColorProperty);
            set => SetValue(InactiveColorProperty, value);
        }

        public static readonly DependencyProperty InactiveColorProperty =
            DependencyProperty.Register(nameof(InactiveColor), typeof(Brush), typeof(RatingControl),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(224, 224, 224))));

        /// <summary>
        /// 获取或设置激活颜色
        /// </summary>
        public Brush ActiveColor
        {
            get => (Brush)GetValue(ActiveColorProperty);
            set => SetValue(ActiveColorProperty, value);
        }

        public static readonly DependencyProperty ActiveColorProperty =
            DependencyProperty.Register(nameof(ActiveColor), typeof(Brush), typeof(RatingControl),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 193, 7))));

        /// <summary>
        /// 获取或设置悬停颜色
        /// </summary>
        public Brush HoverColor
        {
            get => (Brush)GetValue(HoverColorProperty);
            set => SetValue(HoverColorProperty, value);
        }

        public static readonly DependencyProperty HoverColorProperty =
            DependencyProperty.Register(nameof(HoverColor), typeof(Brush), typeof(RatingControl),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 213, 79))));

        /// <summary>
        /// 获取或设置是否启用悬停预览
        /// </summary>
        public bool EnablePreview
        {
            get => (bool)GetValue(EnablePreviewProperty);
            set => SetValue(EnablePreviewProperty, value);
        }

        public static readonly DependencyProperty EnablePreviewProperty =
            DependencyProperty.Register(nameof(EnablePreview), typeof(bool), typeof(RatingControl),
                new PropertyMetadata(true));

        /// <summary>
        /// 获取或设置是否只读
        /// </summary>
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(RatingControl),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否显示数值
        /// </summary>
        public bool ShowValue
        {
            get => (bool)GetValue(ShowValueProperty);
            set => SetValue(ShowValueProperty, value);
        }

        public static readonly DependencyProperty ShowValueProperty =
            DependencyProperty.Register(nameof(ShowValue), typeof(bool), typeof(RatingControl),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置评分项集合
        /// </summary>
        public ObservableCollection<RatingItem> Items => _items;

        /// <summary>
        /// 评分改变命令
        /// </summary>
        public ICommand ValueChangedCommand
        {
            get => (ICommand)GetValue(ValueChangedCommandProperty);
            set => SetValue(ValueChangedCommandProperty, value);
        }

        public static readonly DependencyProperty ValueChangedCommandProperty =
            DependencyProperty.Register(nameof(ValueChangedCommand), typeof(ICommand), typeof(RatingControl),
                new PropertyMetadata(null));

        #endregion

        #region Event Handlers

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RatingControl control)
            {
                control.UpdateItems();
                control.ValueChangedCommand?.Execute(e.NewValue);
            }
        }

        private static void OnMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RatingControl control)
            {
                control.UpdateItems();
            }
        }

        private static void OnIconTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RatingControl control)
            {
                control.UpdateItems();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 设置评分值
        /// </summary>
        public void SetValue(double value)
        {
            double clampedValue = Math.Clamp(value, 0, MaxValue);
            double roundedValue = Math.Round(clampedValue / Precision) * Precision;
            Value = roundedValue;
        }

        /// <summary>
        /// 清空评分
        /// </summary>
        public void ClearValue()
        {
            Value = 0;
        }

        #endregion

        #region Internal Methods

        internal void OnItemHover(int index, bool isHovering)
        {
            if (!EnablePreview || IsReadOnly) return;

            _isPreviewing = isHovering;

            if (isHovering)
            {
                UpdateItemsForPreview(index + 1);
            }
            else
            {
                UpdateItems();
            }
        }

        internal void OnItemClick(int index)
        {
            if (IsReadOnly) return;

            SetValue(index + 1);
        }

        #endregion

        #region Private Methods

        private void UpdateItems()
        {
            _items.Clear();

            for (int i = 0; i < MaxValue; i++)
            {
                double itemValue = i + 1;
                double fillRatio = 0;

                if (Value >= itemValue)
                {
                    fillRatio = 1.0;
                }
                else if (Value > i)
                {
                    fillRatio = Value - i;
                }

                _items.Add(new RatingItem
                {
                    Index = i,
                    FillRatio = fillRatio,
                    IsHovered = false
                });
            }
        }

        private void UpdateItemsForPreview(double previewValue)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                double itemValue = i + 1;
                double fillRatio = 0;

                if (previewValue >= itemValue)
                {
                    fillRatio = 1.0;
                }
                else if (previewValue > i)
                {
                    fillRatio = previewValue - i;
                }

                item.FillRatio = fillRatio;
            }
        }

        #endregion
    }

    /// <summary>
    /// 评分项
    /// </summary>
    public class RatingItem
    {
        /// <summary>
        /// 获取或设置索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 获取或设置填充比例（0-1）
        /// </summary>
        public double FillRatio { get; set; }

        /// <summary>
        /// 获取或设置是否悬停
        /// </summary>
        public bool IsHovered { get; set; }
    }
}
