using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lemoo.UI.Models.Icons;
using Lemoo.UI.Services;

namespace Lemoo.UI.WPF.ViewModels.Pages
{
    /// <summary>
    /// 图标浏览器页面视图模型 V2
    /// 使用新的 IconMetadataRegistry，支持中英文搜索
    /// </summary>
    public partial class IconBrowserPageViewModelV2 : ObservableObject
    {
        private ObservableCollection<IconInfo> _allIcons;
        private ObservableCollection<IconInfo> _filteredIcons;
        private ObservableCollection<IconCategoryItemViewModel> _categories;
        private IconSize _currentIconSize;
        private IconCategoryItemViewModel? _selectedCategory;
        private string _searchText = string.Empty;
        private IconInfo? _selectedIcon;

        public IconBrowserPageViewModelV2()
        {
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
        /// 当前选中的图标
        /// </summary>
        public IconInfo? SelectedIcon
        {
            get => _selectedIcon;
            set
            {
                _selectedIcon = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DetailName));
                OnPropertyChanged(nameof(DetailCategory));
                OnPropertyChanged(nameof(DetailGlyph));
                OnPropertyChanged(nameof(DetailCode));
                OnPropertyChanged(nameof(DetailUnicode));
                OnPropertyChanged(nameof(DetailKeywords));
            }
        }

        /// <summary>
        /// 详情页：图标名称
        /// </summary>
        public string DetailName => SelectedIcon?.Name ?? string.Empty;

        /// <summary>
        /// 详情页：分类
        /// </summary>
        public string DetailCategory => SelectedIcon?.Category ?? string.Empty;

        /// <summary>
        /// 详情页：字形
        /// </summary>
        public string DetailGlyph => SelectedIcon?.Glyph ?? string.Empty;

        /// <summary>
        /// 详情页：代码
        /// </summary>
        public string DetailCode => SelectedIcon != null ? $"IconKind.{SelectedIcon.Kind}" : string.Empty;

        /// <summary>
        /// 详情页：Unicode
        /// </summary>
        public string DetailUnicode => SelectedIcon != null ? $"U+{SelectedIcon.Glyph.Substring(2)}" : string.Empty;

        /// <summary>
        /// 详情页：关键词
        /// </summary>
        public string DetailKeywords => SelectedIcon != null ? string.Join(", ", SelectedIcon.Keywords.Take(5)) : string.Empty;

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
                IconCount = _allIcons.Count,
                IsSelected = true
            };
            _categories.Add(allCategory);

            // 从 IconMetadataRegistry 获取分类
            var categories = IconMetadataRegistry.GetCategories().ToList();

            foreach (var category in categories)
            {
                _categories.Add(new IconCategoryItemViewModel
                {
                    DisplayName = category.DisplayName,
                    CategoryName = category.Key,
                    IconCount = category.Count,
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

            // 按搜索文本过滤（支持中英文）
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                var searchLower = _searchText.ToLower();
                query = query.Where(icon =>
                    icon.NameLower.Contains(searchLower) ||
                    icon.CategoryLower.Contains(searchLower) ||
                    icon.KeywordsLower.Any(k => k.Contains(searchLower)));
            }

            // 批量更新集合
            var result = query.ToList();
            FilteredIcons = new ObservableCollection<IconInfo>(result);
            OnPropertyChanged(nameof(DisplayedIconCount));
        }

        /// <summary>
        /// 搜索命令（支持中英文）
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

        /// <summary>
        /// 刷新图标列表（重新加载）
        /// </summary>
        [RelayCommand]
        private void Refresh()
        {
            IconMetadataRegistry.ClearCache();
            _allIcons = new ObservableCollection<IconInfo>(IconMetadataRegistry.GetAllIcons());
            InitializeCategories();
            UpdateFilteredIcons();
        }
    }

}
