using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Lemoo.UI.Controls.Layout
{
    /// <summary>
    /// 支持虚拟化的 WrapPanel 面板
    /// 通过只渲染可见项来优化大量项目的性能
    /// </summary>
    public class VirtualizingWrapPanel : VirtualizingPanel, IScrollInfo
    {
        // UI元素集合
        private readonly Size _pixelSize = new(1.0, 1.0);

        // 滚动状态
        private ScrollViewer? _scrollOwner;
        private double _offset = 0.0;
        private double _extent = 0.0;
        private double _viewport = 0.0;
        private bool _canHorizontallyScroll;
        private bool _canVerticallyScroll;

        // 布局状态
        private int _firstVisibleIndex = -1;
        private int _lastVisibleIndex = -1;
        private double _childWidth = 100.0;
        private double _childHeight = 100.0;
        private int _itemsPerRow = 1;

        public VirtualizingWrapPanel()
        {
            UseLayoutRounding = true;
            RenderOptions.SetClearTypeHint(this, ClearTypeHint.Enabled);
        }

        private IItemContainerGenerator Generator => ItemContainerGenerator;

        #region 依赖属性

        /// <summary>
        /// 子项宽度
        /// </summary>
        public double ChildWidth
        {
            get => (double)GetValue(ChildWidthProperty);
            set => SetValue(ChildWidthProperty, value);
        }

        public static readonly DependencyProperty ChildWidthProperty =
            DependencyProperty.Register(
                nameof(ChildWidth),
                typeof(double),
                typeof(VirtualizingWrapPanel),
                new PropertyMetadata(100.0, OnLayoutParameterChanged));

        /// <summary>
        /// 子项高度
        /// </summary>
        public double ChildHeight
        {
            get => (double)GetValue(ChildHeightProperty);
            set => SetValue(ChildHeightProperty, value);
        }

        public static readonly DependencyProperty ChildHeightProperty =
            DependencyProperty.Register(
                nameof(ChildHeight),
                typeof(double),
                typeof(VirtualizingWrapPanel),
                new PropertyMetadata(100.0, OnLayoutParameterChanged));

        private static void OnLayoutParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VirtualizingWrapPanel panel)
            {
                panel.InvalidateMeasure();
            }
        }

        #endregion

        #region 布局测量和排列

        protected override Size MeasureOverride(Size availableSize)
        {
            if (availableSize.Width == double.PositiveInfinity ||
                availableSize.Height == double.PositiveInfinity)
            {
                // 在无限空间中，限制面板大小
                availableSize = new(Math.Min(availableSize.Width, 4096), Math.Min(availableSize.Height, 4096));
            }

            _childWidth = ChildWidth;
            _childHeight = ChildHeight;

            // 计算每行可以容纳的项数
            double availableWidth = availableSize.Width - 4; // 减去边距
            _itemsPerRow = Math.Max(1, (int)(availableWidth / (_childWidth + 12))); // +12 是 margin

            // 更新视口大小
            _viewport = availableSize.Height;

            // 计算总范围
            int itemCount = InternalChildren.Count;
            int totalRows = (itemCount + _itemsPerRow - 1) / _itemsPerRow;
            _extent = Math.Max(0, (totalRows * (_childHeight + 12)) - 4); // -4 减去边距

            // 计算可见范围
            UpdateVisibleRange();

            // 测量可见项
            Size measuredSize = MeasureVisibleItems(availableSize);

            // 通知滚动信息变更
            if (_scrollOwner != null)
            {
                _scrollOwner.InvalidateScrollInfo();
            }

            return new Size(availableSize.Width, Math.Min(_viewport, _extent));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            // 更新可见范围
            UpdateVisibleRange();

            // 排列可见项
            ArrangeVisibleItems(finalSize);

            // 通知滚动信息变更
            if (_scrollOwner != null)
            {
                _scrollOwner.InvalidateScrollInfo();
            }

            return finalSize;
        }

        private Size MeasureVisibleItems(Size availableSize)
        {
            Size result = new(0, 0);

            for (int i = _firstVisibleIndex; i <= _lastVisibleIndex; i++)
            {
                if (i < 0 || i >= InternalChildren.Count)
                    continue;

                UIElement child = InternalChildren[i];
                if (child == null)
                    continue;

                child.Measure(new Size(_childWidth, _childHeight));
            }

            return result;
        }

        private void ArrangeVisibleItems(Size finalSize)
        {
            for (int i = _firstVisibleIndex; i <= _lastVisibleIndex; i++)
            {
                if (i < 0 || i >= InternalChildren.Count)
                    continue;

                UIElement child = InternalChildren[i];
                if (child == null)
                    continue;

                // 计算位置
                int row = i / _itemsPerRow;
                int col = i % _itemsPerRow;

                double x = 6 + col * (_childWidth + 12); // +6 是初始边距, +12 是每项的 margin
                double y = 6 + row * (_childHeight + 12) - _offset;

                Rect rect = new(x, y, _childWidth, _childHeight);
                child.Arrange(rect);
            }
        }

        private void UpdateVisibleRange()
        {
            int itemCount = InternalChildren.Count;
            if (itemCount == 0)
            {
                _firstVisibleIndex = -1;
                _lastVisibleIndex = -1;
                return;
            }

            // 计算可见的行范围
            int firstVisibleRow = (int)(_offset / (_childHeight + 12));
            int lastVisibleRow = (int)((_offset + _viewport) / (_childHeight + 12));

            // 添加缓冲区（预加载额外的一行）
            firstVisibleRow = Math.Max(0, firstVisibleRow - 1);
            lastVisibleRow = Math.Min((itemCount + _itemsPerRow - 1) / _itemsPerRow - 1, lastVisibleRow + 1);

            // 计算索引范围
            _firstVisibleIndex = firstVisibleRow * _itemsPerRow;
            _lastVisibleIndex = Math.Min(itemCount - 1, (lastVisibleRow + 1) * _itemsPerRow - 1);

            // 确保所有可见项都已生成
            RealizeVisibleItems();
        }

        private void RealizeVisibleItems()
        {
            var startPosition = Generator.GeneratorPositionFromIndex(_firstVisibleIndex);

            using (Generator.StartAt(startPosition, GeneratorDirection.Forward, true))
            {
                for (int i = _firstVisibleIndex; i <= _lastVisibleIndex; i++)
                {
                    bool isNewlyRealized;
                    UIElement? child = Generator.GenerateNext(out isNewlyRealized) as UIElement;

                    if (child != null)
                    {
                        if (isNewlyRealized)
                        {
                            AddInternalChild(child);
                        }
                        else if (!InternalChildren.Contains(child))
                        {
                            AddInternalChild(child);
                        }

                        Generator.PrepareItemContainer(child);
                    }
                }
            }

            // 清理不可见项
            CleanupItems();
        }

        private void CleanupItems()
        {
            for (int i = InternalChildren.Count - 1; i >= 0; i--)
            {
                UIElement child = InternalChildren[i];
                if (child == null)
                    continue;

                int childIndex = Generator.IndexFromGeneratorPosition(new GeneratorPosition(i, 0));

                if (childIndex < _firstVisibleIndex || childIndex > _lastVisibleIndex)
                {
                    InternalChildren.RemoveAt(i);
                }
            }
        }

        #endregion

        #region IScrollInfo 实现

        public bool CanVerticallyScroll
        {
            get => _canVerticallyScroll;
            set => _canVerticallyScroll = value;
        }

        public bool CanHorizontallyScroll
        {
            get => _canHorizontallyScroll;
            set => _canHorizontallyScroll = value;
        }

        public double ExtentWidth => 0;

        public double ExtentHeight => _extent;

        public double ViewportWidth => RenderSize.Width;

        public double ViewportHeight => _viewport;

        public double HorizontalOffset => 0;

        public double VerticalOffset => _offset;

        public ScrollViewer? ScrollOwner
        {
            get => _scrollOwner;
            set => _scrollOwner = value;
        }

        public void LineUp()
        {
            SetVerticalOffset(_offset - 16);
        }

        public void LineDown()
        {
            SetVerticalOffset(_offset + 16);
        }

        public void LineLeft()
        {
            // 不支持水平滚动
        }

        public void LineRight()
        {
            // 不支持水平滚动
        }

        public void PageUp()
        {
            SetVerticalOffset(_offset - _viewport);
        }

        public void PageDown()
        {
            SetVerticalOffset(_offset + _viewport);
        }

        public void PageLeft()
        {
            // 不支持水平滚动
        }

        public void PageRight()
        {
            // 不支持水平滚动
        }

        public void MouseWheelUp()
        {
            SetVerticalOffset(_offset - 48);
        }

        public void MouseWheelDown()
        {
            SetVerticalOffset(_offset + 48);
        }

        public void MouseWheelLeft()
        {
            // 不支持水平滚动
        }

        public void MouseWheelRight()
        {
            // 不支持水平滚动
        }

        public void SetHorizontalOffset(double offset)
        {
            // 不支持水平滚动
        }

        public void SetVerticalOffset(double offset)
        {
            offset = Math.Max(0, Math.Min(_extent - _viewport, offset));
            if (Math.Abs(offset - _offset) > 1e-6)
            {
                _offset = offset;
                InvalidateMeasure();
            }
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            // TODO: 实现将指定元素滚动到可见区域
            return rectangle;
        }

        #endregion
    }
}
