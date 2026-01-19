using System.Windows;
using System.Windows.Controls;

namespace Lemoo.UI.Controls.Data
{
    /// <summary>
    /// 数据网格分页控件
    /// </summary>
    public class DataGridPager : Control
    {
        #region Fields

        private Button? _btnFirst;
        private Button? _btnPrevious;
        private Button? _btnNext;
        private Button? _btnLast;

        #endregion

        #region Constructor

        static DataGridPager()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGridPager),
                new FrameworkPropertyMetadata(typeof(DataGridPager)));
        }

        public DataGridPager()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 获取或设置数据网格
        /// </summary>
        public DataGridEx DataGrid
        {
            get => (DataGridEx)GetValue(DataGridProperty);
            set => SetValue(DataGridProperty, value);
        }

        public static readonly DependencyProperty DataGridProperty =
            DependencyProperty.Register(nameof(DataGrid), typeof(DataGridEx), typeof(DataGridPager),
                new PropertyMetadata(null, OnDataGridChanged));

        /// <summary>
        /// 获取或设置当前页码
        /// </summary>
        public int CurrentPage
        {
            get => (int)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(nameof(CurrentPage), typeof(int), typeof(DataGridPager),
                new PropertyMetadata(1, OnCurrentPageChanged));

        /// <summary>
        /// 获取或设置总页数
        /// </summary>
        public int TotalPages
        {
            get => (int)GetValue(TotalPagesProperty);
            set => SetValue(TotalPagesProperty, value);
        }

        public static readonly DependencyProperty TotalPagesProperty =
            DependencyProperty.Register(nameof(TotalPages), typeof(int), typeof(DataGridPager),
                new PropertyMetadata(1));

        /// <summary>
        /// 获取或设置总记录数
        /// </summary>
        public int TotalItems
        {
            get => (int)GetValue(TotalItemsProperty);
            set => SetValue(TotalItemsProperty, value);
        }

        public static readonly DependencyProperty TotalItemsProperty =
            DependencyProperty.Register(nameof(TotalItems), typeof(int), typeof(DataGridPager),
                new PropertyMetadata(0));

        /// <summary>
        /// 获取或设置页大小
        /// </summary>
        public int PageSize
        {
            get => (int)GetValue(PageSizeProperty);
            set => SetValue(PageSizeProperty, value);
        }

        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register(nameof(PageSize), typeof(int), typeof(DataGridPager),
                new PropertyMetadata(50));

        /// <summary>
        /// 获取或设置是否显示页大小选择器
        /// </summary>
        public bool ShowPageSizeSelector
        {
            get => (bool)GetValue(ShowPageSizeSelectorProperty);
            set => SetValue(ShowPageSizeSelectorProperty, value);
        }

        public static readonly DependencyProperty ShowPageSizeSelectorProperty =
            DependencyProperty.Register(nameof(ShowPageSizeSelector), typeof(bool), typeof(DataGridPager),
                new PropertyMetadata(true));

        /// <summary>
        /// 获取或设置可用的页大小列表
        /// </summary>
        public int[] AvailablePageSizes
        {
            get => (int[])GetValue(AvailablePageSizesProperty);
            set => SetValue(AvailablePageSizesProperty, value);
        }

        public static readonly DependencyProperty AvailablePageSizesProperty =
            DependencyProperty.Register(nameof(AvailablePageSizes), typeof(int[]), typeof(DataGridPager),
                new PropertyMetadata(new[] { 10, 20, 50, 100, 200 }));

        #endregion

        #region Event Handlers

        private static void OnDataGridChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGridPager pager && e.NewValue is DataGridEx grid)
            {
                pager.UpdateFromDataGrid();
            }
        }

        private static void OnCurrentPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGridPager pager)
            {
                pager.UpdateButtonStates();
                pager.DataGrid?.GoToPage((int)e.NewValue);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _btnFirst = GetTemplateChild("PART_ButtonFirst") as Button;
            _btnPrevious = GetTemplateChild("PART_ButtonPrevious") as Button;
            _btnNext = GetTemplateChild("PART_ButtonNext") as Button;
            _btnLast = GetTemplateChild("PART_ButtonLast") as Button;

            if (_btnFirst != null) _btnFirst.Click += OnFirstPageClick;
            if (_btnPrevious != null) _btnPrevious.Click += OnPreviousPageClick;
            if (_btnNext != null) _btnNext.Click += OnNextPageClick;
            if (_btnLast != null) _btnLast.Click += OnLastPageClick;

            UpdateButtonStates();
        }

        private void OnFirstPageClick(object sender, RoutedEventArgs e)
        {
            GoToFirstPage();
        }

        private void OnPreviousPageClick(object sender, RoutedEventArgs e)
        {
            GoToPreviousPage();
        }

        private void OnNextPageClick(object sender, RoutedEventArgs e)
        {
            GoToNextPage();
        }

        private void OnLastPageClick(object sender, RoutedEventArgs e)
        {
            GoToLastPage();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 跳转到第一页
        /// </summary>
        public void GoToFirstPage()
        {
            CurrentPage = 1;
        }

        /// <summary>
        /// 跳转到上一页
        /// </summary>
        public void GoToPreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
            }
        }

        /// <summary>
        /// 跳转到下一页
        /// </summary>
        public void GoToNextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
            }
        }

        /// <summary>
        /// 跳转到最后一页
        /// </summary>
        public void GoToLastPage()
        {
            CurrentPage = TotalPages;
        }

        /// <summary>
        /// 从数据网格更新分页信息
        /// </summary>
        public void UpdateFromDataGrid()
        {
            if (DataGrid != null)
            {
                CurrentPage = DataGrid.CurrentPage;
                TotalPages = DataGrid.TotalPages;
                TotalItems = DataGrid.Items?.Count ?? 0;
                PageSize = DataGrid.PageSize;
                UpdateButtonStates();
            }
        }

        #endregion

        #region Private Methods

        private void UpdateButtonStates()
        {
            if (_btnFirst != null) _btnFirst.IsEnabled = CurrentPage > 1;
            if (_btnPrevious != null) _btnPrevious.IsEnabled = CurrentPage > 1;
            if (_btnNext != null) _btnNext.IsEnabled = CurrentPage < TotalPages;
            if (_btnLast != null) _btnLast.IsEnabled = CurrentPage < TotalPages;
        }

        #endregion
    }
}
