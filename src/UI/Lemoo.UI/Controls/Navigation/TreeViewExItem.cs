using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lemoo.UI.Controls.Navigation
{
    /// <summary>
    /// 增强树视图项控件
    /// </summary>
    public class TreeViewExItem : TreeViewItem
    {
        #region Fields

        private TreeViewEx? _parentTreeView;

        #endregion

        #region Constructor

        static TreeViewExItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewExItem),
                new FrameworkPropertyMetadata(typeof(TreeViewExItem)));
        }

        public TreeViewExItem()
        {
            Expanded += TreeViewExItem_Expanded;
            Collapsed += TreeViewExItem_Collapsed;
            Selected += TreeViewExItem_Selected;
            MouseDoubleClick += TreeViewExItem_MouseDoubleClick;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 获取或设置图标
        /// </summary>
        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(object), typeof(TreeViewExItem),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置是否选中
        /// </summary>
        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(TreeViewExItem),
                new PropertyMetadata(false, OnIsCheckedChanged));

        /// <summary>
        /// 获取或设置是否支持三态
        /// </summary>
        public bool IsThreeState
        {
            get => (bool)GetValue(IsThreeStateProperty);
            set => SetValue(IsThreeStateProperty, value);
        }

        public static readonly DependencyProperty IsThreeStateProperty =
            DependencyProperty.Register(nameof(IsThreeState), typeof(bool), typeof(TreeViewExItem),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置节点状态
        /// </summary>
        public TreeNodeState NodeState
        {
            get => (TreeNodeState)GetValue(NodeStateProperty);
            set => SetValue(NodeStateProperty, value);
        }

        public static readonly DependencyProperty NodeStateProperty =
            DependencyProperty.Register(nameof(NodeState), typeof(TreeNodeState), typeof(TreeViewExItem),
                new PropertyMetadata(TreeNodeState.None));

        /// <summary>
        /// 获取或设置徽章计数
        /// </summary>
        public int BadgeCount
        {
            get => (int)GetValue(BadgeCountProperty);
            set => SetValue(BadgeCountProperty, value);
        }

        public static readonly DependencyProperty BadgeCountProperty =
            DependencyProperty.Register(nameof(BadgeCount), typeof(int), typeof(TreeViewExItem),
                new PropertyMetadata(0));

        /// <summary>
        /// 获取或设置徽章颜色
        /// </summary>
        public System.Windows.Media.Brush BadgeBrush
        {
            get => (System.Windows.Media.Brush)GetValue(BadgeBrushProperty);
            set => SetValue(BadgeBrushProperty, value);
        }

        public static readonly DependencyProperty BadgeBrushProperty =
            DependencyProperty.Register(nameof(BadgeBrush), typeof(System.Windows.Media.Brush), typeof(TreeViewExItem),
                new PropertyMetadata(default(System.Windows.Media.Brush)));

        #endregion

        #region Event Handlers

        private void TreeViewExItem_Expanded(object sender, RoutedEventArgs e)
        {
            _parentTreeView = ItemsControl.ItemsControlFromItemContainer(this) as TreeViewEx;

            if (_parentTreeView?.IsSingleExpanded == true)
            {
                // 折叠其他同级节点
                var parent = ItemsControl.ItemsControlFromItemContainer(this);
                if (parent != null)
                {
                    foreach (var item in parent.Items)
                    {
                        if (item != DataContext && parent.ItemContainerGenerator.ContainerFromItem(item) is TreeViewExItem siblingItem)
                        {
                            siblingItem.IsExpanded = false;
                        }
                    }
                }
            }

            _parentTreeView?.NodeExpandedCommand?.Execute(DataContext);
        }

        private void TreeViewExItem_Collapsed(object sender, RoutedEventArgs e)
        {
            _parentTreeView = ItemsControl.ItemsControlFromItemContainer(this) as TreeViewEx;
            _parentTreeView?.NodeCollapsedCommand?.Execute(DataContext);
        }

        private void TreeViewExItem_Selected(object sender, RoutedEventArgs e)
        {
            _parentTreeView = ItemsControl.ItemsControlFromItemContainer(this) as TreeViewEx;
            _parentTreeView?.NodeSelectedCommand?.Execute(DataContext);
        }

        private void TreeViewExItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _parentTreeView = ItemsControl.ItemsControlFromItemContainer(this) as TreeViewEx;
            _parentTreeView?.NodeDoubleClickedCommand?.Execute(DataContext);
        }

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeViewExItem item)
            {
                item.UpdateChildCheckState((bool)e.NewValue);
                item.UpdateParentCheckState();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 展开所有子节点
        /// </summary>
        public void ExpandAll()
        {
            IsExpanded = true;
            ExpandAllItems(Items);
        }

        /// <summary>
        /// 折叠所有子节点
        /// </summary>
        public void CollapseAll()
        {
            IsExpanded = false;
            CollapseAllItems(Items);
        }

        #endregion

        #region Private Methods

        private void ExpandAllItems(System.Collections.IEnumerable items)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                if (ItemContainerGenerator.ContainerFromItem(item) is TreeViewExItem treeItem)
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

        private void CollapseAllItems(System.Collections.IEnumerable items)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                if (ItemContainerGenerator.ContainerFromItem(item) is TreeViewExItem treeItem)
                {
                    treeItem.IsExpanded = false;

                    if (treeItem.Items != null)
                    {
                        CollapseAllItems(treeItem.Items);
                    }
                }
            }
        }

        private void UpdateChildCheckState(bool isChecked)
        {
            if (Items == null) return;

            foreach (var item in Items)
            {
                if (ItemContainerGenerator.ContainerFromItem(item) is TreeViewExItem treeItem)
                {
                    treeItem.IsChecked = isChecked;
                }
            }
        }

        private void UpdateParentCheckState()
        {
            var parent = ItemsControl.ItemsControlFromItemContainer(this) as TreeViewExItem;
            if (parent == null) return;

            bool allChecked = true;
            bool allUnchecked = true;

            foreach (var item in parent.Items)
            {
                if (parent.ItemContainerGenerator.ContainerFromItem(item) is TreeViewExItem treeItem)
                {
                    if (treeItem.IsChecked)
                    {
                        allUnchecked = false;
                    }
                    else
                    {
                        allChecked = false;
                    }

                    if (!allChecked && !allUnchecked)
                    {
                        break;
                    }
                }
            }

            if (allChecked)
            {
                parent.IsChecked = true;
            }
            else if (allUnchecked)
            {
                parent.IsChecked = false;
            }
            else if (parent.IsThreeState)
            {
                // 可以添加中间状态的逻辑
            }

            parent.UpdateParentCheckState();
        }

        #endregion
    }

    /// <summary>
    /// 树节点状态枚举
    /// </summary>
    public enum TreeNodeState
    {
        /// <summary>
        /// 无状态
        /// </summary>
        None,

        /// <summary>
        /// 进行中
        /// </summary>
        InProgress,

        /// <summary>
        /// 已完成
        /// </summary>
        Completed,

        /// <summary>
        /// 错误
        /// </summary>
        Error,

        /// <summary>
        /// 警告
        /// </summary>
        Warning
    }
}
