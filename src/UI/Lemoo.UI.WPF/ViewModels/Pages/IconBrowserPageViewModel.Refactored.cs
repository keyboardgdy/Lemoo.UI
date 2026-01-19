using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lemoo.UI.Models.Icons;
using Lemoo.UI.Services;
using Lemoo.UI.WPF.Services;

namespace Lemoo.UI.WPF.ViewModels.Pages
{
    /// <summary>
    /// 图标浏览器页面视图模型（重构版）
    /// 符合WPF MVVM最佳实践
    /// </summary>
    public partial class IconBrowserPageViewModelRefactored : ObservableObject
    {
        private readonly IClipboardService _clipboardService;
        private readonly INotificationService _notificationService;

        private ObservableCollection<IconItemViewModel> _allIcons;
        private ObservableCollection<IconItemViewModel> _filteredIcons;
        private ObservableCollection<IconCategoryItemViewModel> _categories;
        private IconSize _currentIconSize;
        private IconCategoryItemViewModel? _selectedCategory;
        private string _searchText = string.Empty;
        private IconItemViewModel? _selectedIcon;

        /// <summary>
        /// 构造函数
        /// </summary>
        public IconBrowserPageViewModelRefactored(
            IClipboardService clipboardService,
            INotificationService notificationService)
        {
            _clipboardService = clipboardService;
            _notificationService = notificationService;

            // 确保图标注册表已初始化
            IconMetadataRegistry.Initialize();

            _allIcons = new ObservableCollection<IconItemViewModel>();
            _filteredIcons = new ObservableCollection<IconItemViewModel>();
            _categories = new ObservableCollection<IconCategoryItemViewModel>();
            _currentIconSize = IconSize.Normal;

            LoadIcons();
            InitializeCategories();
            UpdateFilteredIcons();
        }

        #region 属性

        /// <summary>
        /// 搜索文本
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        /// <summary>
        /// 所有图标数量
        /// </summary>
        public int TotalIconCount => _allIcons.Count;

        /// <summary>
        /// 当前显示的图标数量
        /// </summary>
        public int DisplayedIconCount => _filteredIcons.Count;

        /// <summary>
        /// 过滤后的图标列表
        /// </summary>
        public ObservableCollection<IconItemViewModel> FilteredIcons
        {
            get => _filteredIcons;
            private set
            {
                _filteredIcons = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayedIconCount));
            }
        }

        /// <summary>
        /// 分类列表
        /// </summary>
        public ObservableCollection<IconCategoryItemViewModel> Categories => _categories;

        /// <summary>
        /// 当前选中的图标
        /// </summary>
        public IconItemViewModel? SelectedIcon
        {
            get => _selectedIcon;
            set
            {
                if (SetProperty(ref _selectedIcon, value))
                {
                    // 更新选中状态
                    foreach (var icon in _allIcons)
                    {
                        icon.IsSelected = (icon == value);
                    }

                    // 触发详情相关属性变更
                    OnPropertyChanged(nameof(DetailName));
                    OnPropertyChanged(nameof(DetailCategory));
                    OnPropertyChanged(nameof(DetailGlyph));
                    OnPropertyChanged(nameof(DetailCode));
                    OnPropertyChanged(nameof(DetailUnicode));
                    OnPropertyChanged(nameof(DetailKeywords));
                    OnPropertyChanged(nameof(HasSelectedIcon));
                }
            }
        }

        /// <summary>
        /// 当前图标尺寸
        /// </summary>
        public IconSize CurrentIconSize
        {
            get => _currentIconSize;
            private set
            {
                if (SetProperty(ref _currentIconSize, value))
                {
                    OnPropertyChanged(nameof(CurrentIconSizeName));
                }
            }
        }

        /// <summary>
        /// 当前图标尺寸名称
        /// </summary>
        public string CurrentIconSizeName => CurrentIconSize switch
        {
            IconSize.ExtraSmall => "XS",
            IconSize.Small => "S",
            IconSize.Normal => "M",
            IconSize.Medium => "L",
            IconSize.Large => "XL",
            IconSize.ExtraLarge => "XXL",
            _ => "M"
        };

        /// <summary>
        /// 是否有选中图标
        /// </summary>
        public bool HasSelectedIcon => SelectedIcon != null;

        #endregion

        #region 详情属性

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
        public string DetailUnicode
        {
            get
            {
                if (SelectedIcon != null && SelectedIcon.Glyph.Length > 0)
                {
                    int codePoint = char.ConvertToUtf32(SelectedIcon.Glyph, 0);
                    return $"U+{codePoint:X4}";
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 详情页：关键词
        /// </summary>
        public string DetailKeywords => SelectedIcon != null
            ? string.Join(", ", SelectedIcon.IconInfo.Keywords.Take(5))
            : string.Empty;

        #endregion

        #region 初始化方法

        /// <summary>
        /// 加载图标
        /// </summary>
        private void LoadIcons()
        {
            var icons = IconMetadataRegistry.GetAllIcons();
            foreach (var icon in icons)
            {
                _allIcons.Add(new IconItemViewModel(icon, _clipboardService, _notificationService));
            }
        }

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
                    icon.Name.ToLower().Contains(searchLower) ||
                    icon.Category.ToLower().Contains(searchLower) ||
                    icon.IconInfo.KeywordsLower.Any(k => k.Contains(searchLower)));
            }

            // 批量更新集合
            var result = query.ToList();
            FilteredIcons = new ObservableCollection<IconItemViewModel>(result);
            OnPropertyChanged(nameof(DisplayedIconCount));
        }

        #endregion

        #region 命令

        /// <summary>
        /// 清除搜索命令
        /// </summary>
        [RelayCommand]
        private void ClearSearch()
        {
            SearchText = string.Empty;
            UpdateFilteredIcons();
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
                CurrentIconSize = iconSize;
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
        /// 复制详情命令
        /// </summary>
        [RelayCommand]
        private void CopyDetail(string detailType)
        {
            if (SelectedIcon == null) return;

            string text = detailType switch
            {
                "Code" => DetailCode,
                "Glyph" => DetailGlyph,
                "Unicode" => DetailUnicode,
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(text))
            {
                _clipboardService.SetText(text);
                _notificationService.ShowSuccess($"已复制: {detailType}");
            }
        }

        /// <summary>
        /// 刷新图标列表（重新加载）
        /// </summary>
        [RelayCommand]
        private void Refresh()
        {
            IconMetadataRegistry.ClearCache();
            _allIcons.Clear();
            LoadIcons();
            InitializeCategories();
            UpdateFilteredIcons();
        }

        #endregion
    }
}
