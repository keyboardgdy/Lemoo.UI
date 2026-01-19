using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lemoo.UI.Models.Icons;
using Lemoo.UI.Services;

namespace Lemoo.UI.WPF.ViewModels.Pages
{
    /// <summary>
    /// 图标浏览器页面视图模型
    /// </summary>
    public partial class IconBrowserPageViewModel : ObservableObject
    {
        /// <summary>
        /// 所有图标列表
        /// </summary>
        private ObservableCollection<IconInfo> _allIcons;

        /// <summary>
        /// 当前显示的图标列表
        /// </summary>
        private ObservableCollection<IconInfo> _filteredIcons;

        /// <summary>
        /// 分类列表
        /// </summary>
        private ObservableCollection<IconCategoryItemViewModel> _categories;

        /// <summary>
        /// 当前图标尺寸
        /// </summary>
        private IconSize _currentIconSize;

        /// <summary>
        /// 当前选中的分类
        /// </summary>
        private IconCategoryItemViewModel? _selectedCategory;

        /// <summary>
        /// 当前搜索文本
        /// </summary>
        private string _searchText = string.Empty;

        public IconBrowserPageViewModel()
        {
            // 确保图标注册表已初始化
            IconMetadataRegistry.Initialize();

            _allIcons = new ObservableCollection<IconInfo>(IconMetadataRegistry.GetAllIcons());
            _filteredIcons = new ObservableCollection<IconInfo>(_allIcons);
            _categories = new ObservableCollection<IconCategoryItemViewModel>();
            _currentIconSize = IconSize.Normal;

            InitializeCategories();
            UpdateFilteredIcons();
        }

        /// <summary>
        /// 获取所有图标数量
        /// </summary>
        public int TotalIconCount => _allIcons.Count;

        /// <summary>
        /// 获取当前显示的图标数量
        /// </summary>
        public int DisplayedIconCount => _filteredIcons.Count;

        /// <summary>
        /// 获取过滤后的图标列表
        /// </summary>
        public ObservableCollection<IconInfo> FilteredIcons
        {
            get => _filteredIcons;
            private set
            {
                _filteredIcons = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 获取分类列表
        /// </summary>
        public ObservableCollection<IconCategoryItemViewModel> Categories => _categories;

        /// <summary>
        /// 初始化分类列表
        /// </summary>
        private void InitializeCategories()
        {
            _categories.Clear();

            // 添加"全部"分类
            var allCategory = new IconCategoryItemViewModel
            {
                DisplayName = "全部",
                CategoryName = "All",
                IsSelected = true
            };
            _categories.Add(allCategory);

            // 添加其他分类
            var categories = IconMetadataRegistry.GetCategories()
                .OrderBy(c => c.Priority)
                .ToList();

            foreach (var category in categories)
            {
                _categories.Add(new IconCategoryItemViewModel
                {
                    DisplayName = category.DisplayName,
                    CategoryName = category.Key,
                    IsSelected = false
                });
            }

            _selectedCategory = allCategory;
        }

        /// <summary>
        /// 更新过滤后的图标列表
        /// </summary>
        private void UpdateFilteredIcons()
        {
            var query = _allIcons.AsEnumerable();

            // 按分类过滤
            if (_selectedCategory?.CategoryName != "All")
            {
                query = query.Where(icon => icon.Category == _selectedCategory!.CategoryName);
            }

            // 按搜索文本过滤（使用预先小写化的属性）
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                var searchLower = _searchText.ToLower();
                query = query.Where(icon =>
                    icon.NameLower.Contains(searchLower) ||
                    icon.CategoryLower.Contains(searchLower) ||
                    icon.KeywordsLower.Any(k => k.Contains(searchLower)));
            }

            // 批量更新集合，减少 UI 更新次数
            var result = query.ToList();
            FilteredIcons = new ObservableCollection<IconInfo>(result);
            OnPropertyChanged(nameof(DisplayedIconCount));
        }

        /// <summary>
        /// 搜索命令
        /// </summary>
        [RelayCommand]
        private void Search(string searchText)
        {
            _searchText = searchText ?? string.Empty;
            UpdateFilteredIcons();
        }

        /// <summary>
        /// 设置尺寸命令
        /// </summary>
        [RelayCommand]
        private void SetSize(string size)
        {
            if (Enum.TryParse<IconSize>(size, out var iconSize))
            {
                _currentIconSize = iconSize;
                UpdateFilteredIcons();
            }
        }

        /// <summary>
        /// 选择分类命令
        /// </summary>
        [RelayCommand]
        private void SelectCategory(IconCategoryItemViewModel category)
        {
            if (category == null) return;

            // 更新选中状态
            foreach (var cat in _categories)
            {
                cat.IsSelected = (cat == category);
            }

            _selectedCategory = category;
            UpdateFilteredIcons();
        }
    }
}
