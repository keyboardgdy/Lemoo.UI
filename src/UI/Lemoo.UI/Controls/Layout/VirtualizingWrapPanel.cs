using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Lemoo.UI.Controls.Layout
{
    /// <summary>
    /// 虚拟化 WrapPanel
    /// 基于 WPF 官方样本实现，支持大量项目的虚拟化显示
    /// </summary>
    public class VirtualizingWrapPanel : VirtualizingPanel, IScrollInfo
    {
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
                new PropertyMetadata(100.0, OnLayoutChanged));

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
                new PropertyMetadata(100.0, OnLayoutChanged));

        /// <summary>
        /// 项目间距（水平）
        /// </summary>
        public double ItemHorizontalSpacing
        {
            get => (double)GetValue(ItemHorizontalSpacingProperty);
            set => SetValue(ItemHorizontalSpacingProperty, value);
        }

        public static readonly DependencyProperty ItemHorizontalSpacingProperty =
            DependencyProperty.Register(
                nameof(ItemHorizontalSpacing),
                typeof(double),
                typeof(VirtualizingWrapPanel),
                new PropertyMetadata(6.0, OnLayoutChanged));

        /// <summary>
        /// 项目间距（垂直）
        /// </summary>
        public double ItemVerticalSpacing
        {
            get => (double)GetValue(ItemVerticalSpacingProperty);
            set => SetValue(ItemVerticalSpacingProperty, value);
        }

        public static readonly DependencyProperty ItemVerticalSpacingProperty =
            DependencyProperty.Register(
                nameof(ItemVerticalSpacing),
                typeof(double),
                typeof(VirtualizingWrapPanel),
                new PropertyMetadata(6.0, OnLayoutChanged));

        private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VirtualizingWrapPanel panel)
            {
                panel.InvalidateMeasure();
            }
        }

        #endregion

        #region 字段

        private ScrollViewer? _scrollOwner;
        private double _offsetY = 0.0;
        private double _extentHeight = 0.0;
        private double _viewportHeight = 0.0;
        private bool _canVerticallyScroll = true;
        private bool _canHorizontallyScroll;

        // 缓存计算结果
        private int _itemsPerRow;
        private int _totalRows;

        #endregion

        #region 构造函数

        public VirtualizingWrapPanel()
        {
            UseLayoutRounding = true;
            RenderOptions.SetClearTypeHint(this, ClearTypeHint.Enabled);
        }

        #endregion

        #region 辅助方法

        private ItemsControl? ItemsOwner => ItemsControl.GetItemsOwner(this);

        private int GetItemCount()
        {
            var owner = ItemsOwner;
            if (owner == null || owner.Items == null)
            {
                System.Diagnostics.Debug.WriteLine($"[VirtualizingWrapPanel] GetItemCount: owner is null or Items is null");
                return 0;
            }
            int count = owner.Items.Count;
            System.Diagnostics.Debug.WriteLine($"[VirtualizingWrapPanel] GetItemCount: {count}");
            return count;
        }

        /// <summary>
        /// 计算布局参数
        /// </summary>
        private void CalculateLayout(double availableWidth)
        {
            double childWidth = ChildWidth;
            double hSpacing = ItemHorizontalSpacing;

            double actualChildWidth = childWidth + hSpacing;

            // 计算每行可以放多少个项目
            _itemsPerRow = Math.Max(1, (int)((availableWidth - hSpacing) / actualChildWidth));

            int itemCount = GetItemCount();
            _totalRows = itemCount > 0 ? (itemCount + _itemsPerRow - 1) / _itemsPerRow : 0;
        }

        /// <summary>
        /// 获取指定索引的项目位置
        /// </summary>
        private (int row, int col, double x, double y) GetItemPosition(int index)
        {
            int row = index / _itemsPerRow;
            int col = index % _itemsPerRow;

            double actualChildWidth = ChildWidth + ItemHorizontalSpacing;
            double actualChildHeight = ChildHeight + ItemVerticalSpacing;

            double x = ItemHorizontalSpacing + col * actualChildWidth;
            double y = ItemVerticalSpacing + row * actualChildHeight;

            return (row, col, x, y);
        }

        #endregion

        #region 测量和排列

        protected override Size MeasureOverride(Size availableSize)
        {
            // 处理无限大小
            if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
            {
                availableSize = new Size(
                    double.IsInfinity(availableSize.Width) ? 4096 : availableSize.Width,
                    double.IsInfinity(availableSize.Height) ? 4096 : availableSize.Height);
            }

            int itemCount = GetItemCount();

            // 调试输出
            System.Diagnostics.Debug.WriteLine($"[VirtualizingWrapPanel] MeasureOverride: ItemCount={itemCount}, AvailableSize={availableSize}");

            // 计算布局
            CalculateLayout(availableSize.Width);

            // 更新视口和范围大小
            _viewportHeight = availableSize.Height;
            double actualChildHeight = ChildHeight + ItemVerticalSpacing;
            _extentHeight = Math.Max(0, _totalRows * actualChildHeight + ItemVerticalSpacing);

            // 通知滚动信息变更
            _scrollOwner?.InvalidateScrollInfo();

            if (itemCount == 0)
            {
                return new Size(availableSize.Width, 0);
            }

            // 计算可见范围
            int firstVisibleIndex, lastVisibleIndex;
            GetVisibleRange(out firstVisibleIndex, out lastVisibleIndex);

            System.Diagnostics.Debug.WriteLine($"[VirtualizingWrapPanel] VisibleRange: {firstVisibleIndex} to {lastVisibleIndex}, ItemsPerRow={_itemsPerRow}, TotalRows={_totalRows}");

            // 生成和测量可见项目
            RealizeVisibleItems(firstVisibleIndex, lastVisibleIndex, true);

            System.Diagnostics.Debug.WriteLine($"[VirtualizingWrapPanel] After Realize: ChildrenCount={InternalChildren.Count}");

            return new Size(availableSize.Width, Math.Min(_viewportHeight, _extentHeight));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int itemCount = GetItemCount();
            if (itemCount == 0)
                return finalSize;

            // 计算可见范围
            int firstVisibleIndex, lastVisibleIndex;
            GetVisibleRange(out firstVisibleIndex, out lastVisibleIndex);

            // 排列可见项目
            RealizeVisibleItems(firstVisibleIndex, lastVisibleIndex, false);

            return finalSize;
        }

        /// <summary>
        /// 获取可见范围的项目索引
        /// </summary>
        private void GetVisibleRange(out int firstVisibleIndex, out int lastVisibleIndex)
        {
            double actualChildHeight = ChildHeight + ItemVerticalSpacing;

            int firstVisibleRow = Math.Max(0, (int)(_offsetY / actualChildHeight));
            int lastVisibleRow = Math.Min(_totalRows - 1, (int)Math.Ceiling((_offsetY + _viewportHeight) / actualChildHeight));

            firstVisibleIndex = firstVisibleRow * _itemsPerRow;
            lastVisibleIndex = Math.Min(GetItemCount() - 1, (lastVisibleRow + 1) * _itemsPerRow - 1);

            // 添加缓冲区以提高滚动体验
            int bufferRows = 1;
            firstVisibleIndex = Math.Max(0, firstVisibleIndex - bufferRows * _itemsPerRow);
            lastVisibleIndex = Math.Min(GetItemCount() - 1, lastVisibleIndex + bufferRows * _itemsPerRow);
        }

        /// <summary>
        /// 生成可见项目
        /// </summary>
        private void RealizeVisibleItems(int firstVisibleIndex, int lastVisibleIndex, bool measure)
        {
            var generator = ItemContainerGenerator;
            if (generator == null)
                return;

            int itemCount = GetItemCount();

            if (itemCount == 0 || firstVisibleIndex > lastVisibleIndex)
                return;

            // 清理不可见的项目
            CleanupItems(firstVisibleIndex, lastVisibleIndex);

            // 生成可见项目
            GeneratorPosition startPos = generator.GeneratorPositionFromIndex(firstVisibleIndex);
            // 如果 startPos.Offset != 0，实际插入位置应该是 startPos.Index + 1
            int childIndex = startPos.Offset == 0 ? startPos.Index : startPos.Index + 1;
            int visibleCount = lastVisibleIndex - firstVisibleIndex + 1;

            using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
            {
                for (int i = 0; i < visibleCount; i++)
                {
                    int itemIndex = firstVisibleIndex + i;

                    bool newlyRealized;
                    DependencyObject? containerObj = generator.GenerateNext(out newlyRealized);

                    if (!(containerObj is UIElement child))
                        continue;

                    if (newlyRealized)
                    {
                        // 将新生成的容器插入到 InternalChildren
                        if (childIndex >= InternalChildren.Count)
                        {
                            AddInternalChild(child);
                        }
                        else
                        {
                            InsertInternalChild(childIndex, child);
                        }

                        // 将容器加入逻辑树，确保资源查找与绑定能正确工作
                        try
                        {
                            AddLogicalChild(child);
                        }
                        catch
                        {
                            // ignore
                        }
                    }
                    else
                    {
                        // 已存在的容器：确保它位于正确的子索引，否则插入到正确位置
                        if (childIndex >= InternalChildren.Count || InternalChildren[childIndex] != child)
                        {
                            // 如果容器尚未作为子项或位置不正确，则插入到期望位置
                            if (VisualTreeHelper.GetParent(child) == null)
                            {
                                if (childIndex >= InternalChildren.Count)
                                    AddInternalChild(child);
                                else
                                    InsertInternalChild(childIndex, child);
                            }
                            else
                            {
                                // 如果已经在其他位置，移除并插入到正确位置
                                int existingIndex = InternalChildren.IndexOf(child);
                                if (existingIndex >= 0)
                                {
                                    RemoveInternalChildRange(existingIndex, 1);
                                    if (childIndex >= InternalChildren.Count)
                                        AddInternalChild(child);
                                    else
                                        InsertInternalChild(childIndex, child);
                                }
                            }
                        }
                    }

                    // 确保控件模板已应用，以便 DataTemplate 内的元素可见
                    (child as FrameworkElement)?.ApplyTemplate();

                    // 准备容器（应用数据绑定）——对新旧容器都调用以确保绑定/样式生效
                    try
                    {
                        generator.PrepareItemContainer(child);
                    }
                    catch
                    {
                        // 某些容器类型可能不需要或无法准备，忽略潜在异常以避免破坏布局
                    }

                    // 计算位置
                    var (row, col, x, y) = GetItemPosition(itemIndex);
                    y -= _offsetY;

                    if (measure)
                    {
                        child.Measure(new Size(ChildWidth, ChildHeight));
                    }
                    else
                    {
                        child.Arrange(new Rect(x, y, ChildWidth, ChildHeight));
                    }

                    childIndex++;
                }
            }
        }

        /// <summary>
        /// 清理不可见的项目
        /// </summary>
        private void CleanupItems(int firstVisibleIndex, int lastVisibleIndex)
        {
            var generator = ItemContainerGenerator;
            if (generator == null)
                return;

            // 遍历 InternalChildren，移除不在可见范围内的容器
            for (int i = InternalChildren.Count - 1; i >= 0; i--)
            {
                GeneratorPosition genPos = new GeneratorPosition(i, 0);
                int itemIndex = generator.IndexFromGeneratorPosition(genPos);
                if (itemIndex < firstVisibleIndex || itemIndex > lastVisibleIndex)
                {
                    // 从生成器中移除并从 InternalChildren 中移除
                    var child = InternalChildren[i];
                    try
                    {
                        RemoveLogicalChild(child);
                    }
                    catch
                    {
                        // ignore
                    }
                    generator.Remove(genPos, 1);
                    RemoveInternalChildRange(i, 1);
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

        public double ExtentHeight => _extentHeight;

        public double ViewportWidth => RenderSize.Width;

        public double ViewportHeight => _viewportHeight;

        public double HorizontalOffset => 0;

        public double VerticalOffset => _offsetY;

        public ScrollViewer? ScrollOwner
        {
            get => _scrollOwner;
            set => _scrollOwner = value;
        }

        public void LineUp() => SetVerticalOffset(_offsetY - 16);

        public void LineDown() => SetVerticalOffset(_offsetY + 16);

        public void LineLeft() { }

        public void LineRight() { }

        public void PageUp() => SetVerticalOffset(_offsetY - _viewportHeight);

        public void PageDown() => SetVerticalOffset(_offsetY + _viewportHeight);

        public void PageLeft() { }

        public void PageRight() { }

        public void MouseWheelUp() => SetVerticalOffset(_offsetY - 48);

        public void MouseWheelDown() => SetVerticalOffset(_offsetY + 48);

        public void MouseWheelLeft() { }

        public void MouseWheelRight() { }

        public void SetHorizontalOffset(double offset) { }

        public void SetVerticalOffset(double offset)
        {
            offset = Math.Max(0, Math.Min(_extentHeight - _viewportHeight, offset));
            if (Math.Abs(offset - _offsetY) > 1e-6)
            {
                _offsetY = offset;
                InvalidateArrange();
                _scrollOwner?.InvalidateScrollInfo();
            }
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            if (visual is UIElement element && InternalChildren.Contains(element))
            {
                var generator = ItemContainerGenerator;
                if (generator != null)
                {
                    int index = generator.IndexFromGeneratorPosition(new GeneratorPosition(InternalChildren.IndexOf(element), 0));
                    if (index >= 0)
                    {
                        var (row, col, x, y) = GetItemPosition(index);
                        double itemTop = y;
                        double itemBottom = y + ChildHeight;

                        // If item is above viewport, scroll up to its top.
                        if (itemTop < _offsetY)
                        {
                            SetVerticalOffset(itemTop);
                        }
                        // If item is below viewport, scroll down so its bottom is visible.
                        else if (itemBottom > _offsetY + _viewportHeight)
                        {
                            SetVerticalOffset(itemBottom - _viewportHeight);
                        }
                    }
                }
            }

            return rectangle;
        }

        #endregion
    }
}
