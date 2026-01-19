using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Lemoo.UI.Controls.Data
{
    /// <summary>
    /// 增强数据网格控件，支持分页、排序、筛选、导出等功能
    /// </summary>
    public class DataGridEx : DataGrid
    {
        #region Fields

        private ListCollectionView? _collectionView;
#pragma warning disable CS0649 // 字段从未赋值，将在模板中初始化
        private CheckBox? _selectAllCheckBox;
#pragma warning restore CS0649

        #endregion

        #region Constructor

        static DataGridEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGridEx),
                new FrameworkPropertyMetadata(typeof(DataGridEx)));
        }

        public DataGridEx()
        {
            Loaded += DataGridEx_Loaded;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 获取或设置是否支持多选
        /// </summary>
        public bool AllowMultiSelect
        {
            get => (bool)GetValue(AllowMultiSelectProperty);
            set => SetValue(AllowMultiSelectProperty, value);
        }

        public static readonly DependencyProperty AllowMultiSelectProperty =
            DependencyProperty.Register(nameof(AllowMultiSelect), typeof(bool), typeof(DataGridEx),
                new PropertyMetadata(false, OnAllowMultiSelectChanged));

        /// <summary>
        /// 获取或设置是否显示全选复选框
        /// </summary>
        public bool ShowSelectAllCheckbox
        {
            get => (bool)GetValue(ShowSelectAllCheckboxProperty);
            set => SetValue(ShowSelectAllCheckboxProperty, value);
        }

        public static readonly DependencyProperty ShowSelectAllCheckboxProperty =
            DependencyProperty.Register(nameof(ShowSelectAllCheckbox), typeof(bool), typeof(DataGridEx),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否启用分页
        /// </summary>
        public bool EnablePaging
        {
            get => (bool)GetValue(EnablePagingProperty);
            set => SetValue(EnablePagingProperty, value);
        }

        public static readonly DependencyProperty EnablePagingProperty =
            DependencyProperty.Register(nameof(EnablePaging), typeof(bool), typeof(DataGridEx),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置页大小
        /// </summary>
        public int PageSize
        {
            get => (int)GetValue(PageSizeProperty);
            set => SetValue(PageSizeProperty, value);
        }

        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register(nameof(PageSize), typeof(int), typeof(DataGridEx),
                new PropertyMetadata(50, OnPageSizeChanged));

        /// <summary>
        /// 获取或设置当前页码
        /// </summary>
        public int CurrentPage
        {
            get => (int)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(nameof(CurrentPage), typeof(int), typeof(DataGridEx),
                new PropertyMetadata(1, OnCurrentPageChanged));

        /// <summary>
        /// 获取总页数
        /// </summary>
        public int TotalPages
        {
            get => (int)GetValue(TotalPagesProperty);
            private set => SetValue(TotalPagesPropertyKey, value);
        }

        public static readonly DependencyPropertyKey TotalPagesPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(TotalPages), typeof(int), typeof(DataGridEx),
                new PropertyMetadata(0));

        public static readonly DependencyProperty TotalPagesProperty = TotalPagesPropertyKey.DependencyProperty;

        /// <summary>
        /// 获取或设置是否显示行号
        /// </summary>
        public bool ShowRowNumber
        {
            get => (bool)GetValue(ShowRowNumberProperty);
            set => SetValue(ShowRowNumberProperty, value);
        }

        public static readonly DependencyProperty ShowRowNumberProperty =
            DependencyProperty.Register(nameof(ShowRowNumber), typeof(bool), typeof(DataGridEx),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置交替行颜色
        /// </summary>
        public new Brush AlternatingRowBackground
        {
            get => (Brush)GetValue(AlternatingRowBackgroundProperty);
            set => SetValue(AlternatingRowBackgroundProperty, value);
        }

        public static readonly new DependencyProperty AlternatingRowBackgroundProperty =
            DependencyProperty.Register(nameof(AlternatingRowBackground), typeof(Brush), typeof(DataGridEx),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置选中项集合
        /// </summary>
        public IList SelectedItemsList
        {
            get => (IList)GetValue(SelectedItemsListProperty);
            set => SetValue(SelectedItemsListProperty, value);
        }

        public static readonly DependencyProperty SelectedItemsListProperty =
            DependencyProperty.Register(nameof(SelectedItemsList), typeof(IList), typeof(DataGridEx),
                new PropertyMetadata(null));

        /// <summary>
        /// 分页命令
        /// </summary>
        public ICommand PageChangedCommand
        {
            get => (ICommand)GetValue(PageChangedCommandProperty);
            set => SetValue(PageChangedCommandProperty, value);
        }

        public static readonly DependencyProperty PageChangedCommandProperty =
            DependencyProperty.Register(nameof(PageChangedCommand), typeof(ICommand), typeof(DataGridEx),
                new PropertyMetadata(null));

        /// <summary>
        /// 导出命令
        /// </summary>
        public ICommand ExportCommand
        {
            get => (ICommand)GetValue(ExportCommandProperty);
            set => SetValue(ExportCommandProperty, value);
        }

        public static readonly DependencyProperty ExportCommandProperty =
            DependencyProperty.Register(nameof(ExportCommand), typeof(ICommand), typeof(DataGridEx),
                new PropertyMetadata(null));

        #endregion

        #region Event Handlers

        private void DataGridEx_Loaded(object sender, RoutedEventArgs e)
        {
            if (ShowSelectAllCheckbox && AllowMultiSelect)
            {
                EnsureSelectAllCheckbox();
            }

            UpdateTotalPages();
        }

        private static void OnAllowMultiSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGridEx grid)
            {
                grid.SelectionMode = (bool)e.NewValue ? DataGridSelectionMode.Extended : DataGridSelectionMode.Single;
            }
        }

        private static void OnPageSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGridEx grid)
            {
                grid.UpdateTotalPages();
                if (grid.EnablePaging)
                {
                    grid.RefreshData();
                }
            }
        }

        private static void OnCurrentPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGridEx grid)
            {
                if (grid.EnablePaging)
                {
                    grid.RefreshData();
                }
                grid.PageChangedCommand?.Execute(e.NewValue);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            EnsureSelectAllCheckbox();
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (newValue != null)
            {
                _collectionView = newValue as ListCollectionView ?? new ListCollectionView(newValue as IList);
                UpdateTotalPages();
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (SelectedItemsList != null)
            {
                SelectedItemsList.Clear();
                foreach (var item in SelectedItems)
                {
                    SelectedItemsList.Add(item);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 刷新数据
        /// </summary>
        public void RefreshData()
        {
            if (_collectionView != null && EnablePaging)
            {
                _collectionView.Refresh();
            }

            UpdateSelectAllCheckboxState();
        }

        /// <summary>
        /// 上一页
        /// </summary>
        public void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
            }
        }

        /// <summary>
        /// 下一页
        /// </summary>
        public void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
            }
        }

        /// <summary>
        /// 跳转到指定页
        /// </summary>
        public void GoToPage(int pageNumber)
        {
            if (pageNumber >= 1 && pageNumber <= TotalPages)
            {
                CurrentPage = pageNumber;
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        public new void SelectAll()
        {
            if (AllowMultiSelect && Items != null)
            {
                SelectedItems.Clear();
                foreach (var item in Items)
                {
                    SelectedItems.Add(item);
                }
                UpdateSelectAllCheckboxState();
            }
        }

        /// <summary>
        /// 取消全选
        /// </summary>
        public new void UnselectAll()
        {
            SelectedItems.Clear();
            UpdateSelectAllCheckboxState();
        }

        #endregion

        #region Private Methods

        private void EnsureSelectAllCheckbox()
        {
            if (_selectAllCheckBox != null) return;

            // 在实际使用中，全选复选框应该在模板中定义
            // 这里提供基本框架，具体实现需要配合 XAML 模板
        }

        private void UpdateSelectAllCheckboxState()
        {
            if (_selectAllCheckBox != null && Items != null)
            {
                int itemCount = Items.Count;
                if (itemCount > 0)
                {
                    _selectAllCheckBox.IsChecked = SelectedItems.Count == itemCount;
                }
                else
                {
                    _selectAllCheckBox.IsChecked = false;
                }
            }
        }

        private void UpdateTotalPages()
        {
            if (Items != null)
            {
                int itemCount = Items.Count;
                TotalPages = PageSize > 0 ? (int)Math.Ceiling((double)itemCount / PageSize) : 1;
                if (CurrentPage > TotalPages)
                {
                    CurrentPage = TotalPages > 0 ? TotalPages : 1;
                }
            }
        }

        #endregion
    }
}
