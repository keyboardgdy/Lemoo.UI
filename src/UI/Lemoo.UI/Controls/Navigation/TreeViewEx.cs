using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Lemoo.UI.Controls.Navigation
{
    /// <summary>
    /// 增强树视图控件，支持拖拽、多选、虚拟化等功能
    /// </summary>
    public class TreeViewEx : TreeView
    {
        #region Fields

        private TreeViewItem? _dragItem;
        private bool _isDragging;

        #endregion

        #region Constructor

        static TreeViewEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewEx),
                new FrameworkPropertyMetadata(typeof(TreeViewEx)));
        }

        public TreeViewEx()
        {
            AllowDrop = true;
            Drop += TreeViewEx_Drop;
            DragOver += TreeViewEx_DragOver;
            DragLeave += TreeViewEx_DragLeave;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 获取或设置是否允许拖拽
        /// </summary>
        public bool AllowDragDrop
        {
            get => (bool)GetValue(AllowDragDropProperty);
            set => SetValue(AllowDragDropProperty, value);
        }

        public static readonly DependencyProperty AllowDragDropProperty =
            DependencyProperty.Register(nameof(AllowDragDrop), typeof(bool), typeof(TreeViewEx),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否允许多选
        /// </summary>
        public bool AllowMultiSelect
        {
            get => (bool)GetValue(AllowMultiSelectProperty);
            set => SetValue(AllowMultiSelectProperty, value);
        }

        public static readonly DependencyProperty AllowMultiSelectProperty =
            DependencyProperty.Register(nameof(AllowMultiSelect), typeof(bool), typeof(TreeViewEx),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否显示连接线
        /// </summary>
        public bool ShowLines
        {
            get => (bool)GetValue(ShowLinesProperty);
            set => SetValue(ShowLinesProperty, value);
        }

        public static readonly DependencyProperty ShowLinesProperty =
            DependencyProperty.Register(nameof(ShowLines), typeof(bool), typeof(TreeViewEx),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置连接线颜色
        /// </summary>
        public System.Windows.Media.Brush LineBrush
        {
            get => (System.Windows.Media.Brush)GetValue(LineBrushProperty);
            set => SetValue(LineBrushProperty, value);
        }

        public static readonly DependencyProperty LineBrushProperty =
            DependencyProperty.Register(nameof(LineBrush), typeof(System.Windows.Media.Brush), typeof(TreeViewEx),
                new PropertyMetadata(System.Windows.Media.Brushes.Gray));

        /// <summary>
        /// 获取或设置是否显示复选框
        /// </summary>
        public bool ShowCheckBoxes
        {
            get => (bool)GetValue(ShowCheckBoxesProperty);
            set => SetValue(ShowCheckBoxesProperty, value);
        }

        public static readonly DependencyProperty ShowCheckBoxesProperty =
            DependencyProperty.Register(nameof(ShowCheckBoxes), typeof(bool), typeof(TreeViewEx),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否启用搜索
        /// </summary>
        public bool EnableSearch
        {
            get => (bool)GetValue(EnableSearchProperty);
            set => SetValue(EnableSearchProperty, value);
        }

        public static readonly DependencyProperty EnableSearchProperty =
            DependencyProperty.Register(nameof(EnableSearch), typeof(bool), typeof(TreeViewEx),
                new PropertyMetadata(false));

        /// <summary>
        /// 搜索文本
        /// </summary>
        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(nameof(SearchText), typeof(string), typeof(TreeViewEx),
                new PropertyMetadata(string.Empty, OnSearchTextChanged));

        /// <summary>
        /// 节点展开命令
        /// </summary>
        public ICommand NodeExpandedCommand
        {
            get => (ICommand)GetValue(NodeExpandedCommandProperty);
            set => SetValue(NodeExpandedCommandProperty, value);
        }

        public static readonly DependencyProperty NodeExpandedCommandProperty =
            DependencyProperty.Register(nameof(NodeExpandedCommand), typeof(ICommand), typeof(TreeViewEx),
                new PropertyMetadata(null));

        /// <summary>
        /// 节点折叠命令
        /// </summary>
        public ICommand NodeCollapsedCommand
        {
            get => (ICommand)GetValue(NodeCollapsedCommandProperty);
            set => SetValue(NodeCollapsedCommandProperty, value);
        }

        public static readonly DependencyProperty NodeCollapsedCommandProperty =
            DependencyProperty.Register(nameof(NodeCollapsedCommand), typeof(ICommand), typeof(TreeViewEx),
                new PropertyMetadata(null));

        /// <summary>
        /// 节点选中命令
        /// </summary>
        public ICommand NodeSelectedCommand
        {
            get => (ICommand)GetValue(NodeSelectedCommandProperty);
            set => SetValue(NodeSelectedCommandProperty, value);
        }

        public static readonly DependencyProperty NodeSelectedCommandProperty =
            DependencyProperty.Register(nameof(NodeSelectedCommand), typeof(ICommand), typeof(TreeViewEx),
                new PropertyMetadata(null));

        /// <summary>
        /// 节点双击命令
        /// </summary>
        public ICommand NodeDoubleClickedCommand
        {
            get => (ICommand)GetValue(NodeDoubleClickedCommandProperty);
            set => SetValue(NodeDoubleClickedCommandProperty, value);
        }

        public static readonly DependencyProperty NodeDoubleClickedCommandProperty =
            DependencyProperty.Register(nameof(NodeDoubleClickedCommand), typeof(ICommand), typeof(TreeViewEx),
                new PropertyMetadata(null));

        /// <summary>
        /// 拖拽完成命令
        /// </summary>
        public ICommand DragDropCommand
        {
            get => (ICommand)GetValue(DragDropCommandProperty);
            set => SetValue(DragDropCommandProperty, value);
        }

        public static readonly DependencyProperty DragDropCommandProperty =
            DependencyProperty.Register(nameof(DragDropCommand), typeof(ICommand), typeof(TreeViewEx),
                new PropertyMetadata(null));

        /// <summary>
        /// 节点缩进宽度
        /// </summary>
        public double ItemIndent
        {
            get => (double)GetValue(ItemIndentProperty);
            set => SetValue(ItemIndentProperty, value);
        }

        public static readonly DependencyProperty ItemIndentProperty =
            DependencyProperty.Register(nameof(ItemIndent), typeof(double), typeof(TreeViewEx),
                new PropertyMetadata(24.0));

        /// <summary>
        /// 是否仅展开一个节点
        /// </summary>
        public bool IsSingleExpanded
        {
            get => (bool)GetValue(IsSingleExpandedProperty);
            set => SetValue(IsSingleExpandedProperty, value);
        }

        public static readonly DependencyProperty IsSingleExpandedProperty =
            DependencyProperty.Register(nameof(IsSingleExpanded), typeof(bool), typeof(TreeViewEx),
                new PropertyMetadata(false));

        #endregion

        #region Event Handlers

        private void TreeViewEx_Drop(object sender, DragEventArgs e)
        {
            if (!AllowDragDrop) return;

            _isDragging = false;
            _dragItem = null!;

            var targetItem = GetTreeViewItemFromEvent(sender, e);
            if (targetItem != null && e.Data.GetData(typeof(TreeViewItem)) is TreeViewItem sourceItem)
            {
                var draggedData = sourceItem.DataContext;
                var targetData = targetItem.DataContext;

                DragDropCommand?.Execute(new DragDropEventArgs
                {
                    Source = draggedData,
                    Target = targetData,
                    SourceItem = sourceItem,
                    TargetItem = targetItem
                });

                e.Handled = true;
            }
        }

        private void TreeViewEx_DragOver(object sender, DragEventArgs e)
        {
            if (!AllowDragDrop)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            var targetItem = GetTreeViewItemFromEvent(sender, e);
            if (targetItem != null && targetItem != _dragItem)
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void TreeViewEx_DragLeave(object sender, DragEventArgs e)
        {
            _isDragging = false;
        }

        private static void OnSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeViewEx treeView)
            {
                treeView.FilterNodes(e.NewValue as string ?? string.Empty);
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            if (!AllowDragDrop) return;

            var item = GetTreeViewItemFromEvent(this, e);
            if (item != null)
            {
                _dragItem = item;
                _isDragging = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isDragging && _dragItem != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(this);
                if (Math.Abs(position.X - e.GetPosition(_dragItem).X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - e.GetPosition(_dragItem).Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    DragDrop.DoDragDrop(_dragItem, _dragItem, DragDropEffects.Move);
                    _isDragging = false;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 展开所有节点
        /// </summary>
        public void ExpandAll()
        {
            ExpandAllItems(Items);
        }

        /// <summary>
        /// 折叠所有节点
        /// </summary>
        public void CollapseAll()
        {
            CollapseAllItems(Items);
        }

        /// <summary>
        /// 根据数据查找节点
        /// </summary>
        public TreeViewItem? FindItem(object data)
        {
            return FindItemByData(Items, data);
        }

        /// <summary>
        /// 选中指定数据的节点
        /// </summary>
        public bool SelectItem(object data)
        {
            var item = FindItem(data);
            if (item != null)
            {
                item.IsSelected = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 过滤节点
        /// </summary>
        public void FilterNodes(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                ClearFilter();
                return;
            }

            FilterItems(Items, searchText.ToLowerInvariant());
        }

        /// <summary>
        /// 清除过滤
        /// </summary>
        public void ClearFilter()
        {
            SetItemsVisibility(Items, true);
        }

        #endregion

        #region Private Methods

        private TreeViewItem GetTreeViewItemFromEvent(object sender, RoutedEventArgs e)
        {
            var element = e.OriginalSource as UIElement;
            while (element != null)
            {
                if (element is TreeViewItem item)
                {
                    return item;
                }
                element = VisualTreeHelper.GetParent(element) as UIElement;
            }
            return null!;
        }

        private void ExpandAllItems(IEnumerable items)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                if (ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem treeItem)
                {
                    treeItem.IsExpanded = true;
                    treeItem.UpdateLayout();

                    if (treeItem.Items != null)
                    {
                        ExpandAllItems(treeItem.Items);
                    }
                }
            }
        }

        private void CollapseAllItems(IEnumerable items)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                if (ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem treeItem)
                {
                    treeItem.IsExpanded = false;

                    if (treeItem.Items != null)
                    {
                        CollapseAllItems(treeItem.Items);
                    }
                }
            }
        }

        private TreeViewItem? FindItemByData(IEnumerable items, object data)
        {
            if (items == null) return null;

            foreach (var item in items)
            {
                if (ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem treeItem)
                {
                    if (item == data)
                    {
                        return treeItem;
                    }

                    if (treeItem.Items != null)
                    {
                        var found = FindItemByData(treeItem.Items, data);
                        if (found != null) return found;
                    }
                }
            }

            return null;
        }

        private bool FilterItems(IEnumerable items, string searchText)
        {
            bool hasVisibleChild = false;

            foreach (var item in items)
            {
                if (ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem treeItem)
                {
                    bool itemMatches = ItemMatchesSearch(item, searchText);
                    bool childMatches = treeItem.Items != null && FilterItems(treeItem.Items, searchText);

                    bool isVisible = itemMatches || childMatches;
                    treeItem.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;

                    if (isVisible && !treeItem.IsExpanded)
                    {
                        treeItem.IsExpanded = childMatches;
                    }

                    hasVisibleChild = hasVisibleChild || isVisible;
                }
            }

            return hasVisibleChild;
        }

        private bool ItemMatchesSearch(object item, string searchText)
        {
            if (item == null) return false;

            var itemText = item.ToString();
            return !string.IsNullOrWhiteSpace(itemText) && itemText.ToLowerInvariant().Contains(searchText);
        }

        private void SetItemsVisibility(IEnumerable items, bool visible)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                if (ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem treeItem)
                {
                    treeItem.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;

                    if (treeItem.Items != null)
                    {
                        SetItemsVisibility(treeItem.Items, visible);
                    }
                }
            }
        }

        #endregion

        #region Nested Types

        /// <summary>
        /// 拖拽事件参数
        /// </summary>
        public class DragDropEventArgs
        {
            public object Source { get; set; } = null!;
            public object Target { get; set; } = null!;
            public TreeViewItem SourceItem { get; set; } = null!;
            public TreeViewItem TargetItem { get; set; } = null!;
        }

        #endregion
    }
}
