using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lemoo.UI.Models;
using Lemoo.UI.Services;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Lemoo.UI.WPF.ViewModels;

/// <summary>
/// 工具箱视图模型
/// </summary>
public partial class ToolboxViewModel : ObservableObject
{
    /// <summary>
    /// 搜索关键词
    /// </summary>
    [ObservableProperty]
    private string _searchKeyword = string.Empty;

    /// <summary>
    /// 选中的控件
    /// </summary>
    [ObservableProperty]
    private ControlInfo? _selectedControl;

    /// <summary>
    /// 选中的样式变体
    /// </summary>
    [ObservableProperty]
    private ControlStyleVariant? _selectedStyleVariant;

    /// <summary>
    /// 是否正在搜索
    /// </summary>
    [ObservableProperty]
    private bool _isSearching;

    /// <summary>
    /// 所有控件（支持过滤）
    /// </summary>
    public ObservableCollection<ControlInfo> AllControls { get; } = new();

    /// <summary>
    /// 搜索结果
    /// </summary>
    public ObservableCollection<ControlInfo> SearchResults { get; } = new();

    /// <summary>
    /// 搜索结果数量
    /// </summary>
    public int SearchResultCount => SearchResults.Count;

    /// <summary>
    /// 是否有搜索结果
    /// </summary>
    public bool HasSearchResults => SearchResults.Count > 0;

    /// <summary>
    /// 按分类分组的控件集合
    /// </summary>
    public ListCollectionView GroupedControls { get; }

    /// <summary>
    /// 所有分类
    /// </summary>
    public ObservableCollection<ControlCategoryGroup> Categories { get; } = new();

    /// <summary>
    /// 展开的分类
    /// </summary>
    public HashSet<ControlCategory> ExpandedCategories { get; } = new();

    public ToolboxViewModel()
    {
        // 加载所有控件
        LoadControls();

        // 创建分组视图
        GroupedControls = new ListCollectionView(AllControls)
        {
            GroupDescriptions = { new PropertyGroupDescription(nameof(ControlInfo.Category)) }
        };

        // 初始化分类
        InitializeCategories();

        // 默认展开所有分类
        ExpandAllCategories();
    }

    /// <summary>
    /// 加载所有控件
    /// </summary>
    private void LoadControls()
    {
        var controls = ControlRegistry.GetAllControls();
        foreach (var control in controls)
        {
            AllControls.Add(control);
        }
    }

    /// <summary>
    /// 初始化分类
    /// </summary>
    private void InitializeCategories()
    {
        Categories.Clear();
        foreach (ControlCategory category in Enum.GetValues(typeof(ControlCategory)))
        {
            var controls = ControlRegistry.GetControlsByCategory(category);
            if (controls.Count > 0)
            {
                Categories.Add(new ControlCategoryGroup
                {
                    Category = category,
                    DisplayName = ControlRegistry.GetCategoryDisplayName(category),
                    Count = controls.Count
                });
            }
        }
    }

    /// <summary>
    /// 搜索控件
    /// </summary>
    [RelayCommand]
    private void Search()
    {
        if (string.IsNullOrWhiteSpace(SearchKeyword))
        {
            IsSearching = false;
            SearchResults.Clear();
            return;
        }

        IsSearching = true;
        var results = ControlRegistry.SearchControls(SearchKeyword);

        SearchResults.Clear();
        foreach (var result in results)
        {
            SearchResults.Add(result);
        }

        // 通知搜索结果数量变化
        OnPropertyChanged(nameof(SearchResultCount));
        OnPropertyChanged(nameof(HasSearchResults));
    }

    /// <summary>
    /// 清除搜索
    /// </summary>
    [RelayCommand]
    private void ClearSearch()
    {
        SearchKeyword = string.Empty;
        IsSearching = false;
        SearchResults.Clear();
    }

    /// <summary>
    /// 展开/折叠分类
    /// </summary>
    [RelayCommand]
    private void ToggleCategory(ControlCategory category)
    {
        if (ExpandedCategories.Contains(category))
        {
            ExpandedCategories.Remove(category);
        }
        else
        {
            ExpandedCategories.Add(category);
        }
    }

    /// <summary>
    /// 展开所有分类
    /// </summary>
    [RelayCommand]
    private void ExpandAllCategories()
    {
        ExpandedCategories.Clear();
        foreach (ControlCategory category in Enum.GetValues(typeof(ControlCategory)))
        {
            ExpandedCategories.Add(category);
        }
    }

    /// <summary>
    /// 折叠所有分类
    /// </summary>
    [RelayCommand]
    private void CollapseAllCategories()
    {
        ExpandedCategories.Clear();
    }

    /// <summary>
    /// 选择控件
    /// </summary>
    [RelayCommand]
    private void SelectControl(ControlInfo control)
    {
        SelectedControl = control;
        SelectedStyleVariant = control.StyleVariants?.FirstOrDefault();
    }

    /// <summary>
    /// 选择样式变体
    /// </summary>
    [RelayCommand]
    private void SelectStyleVariant(ControlStyleVariant variant)
    {
        SelectedStyleVariant = variant;
    }

    /// <summary>
    /// 获取当前要插入的XAML代码
    /// </summary>
    public string GetInsertXaml()
    {
        if (SelectedControl == null) return string.Empty;

        if (SelectedStyleVariant != null && !string.IsNullOrEmpty(SelectedStyleVariant.StyleKey))
        {
            return $"<{SelectedControl.Name} Style=\"{{StaticResource {SelectedStyleVariant.StyleKey}}}\" Content=\"{SelectedControl.DisplayName}\" />";
        }

        return SelectedControl.SampleCode;
    }

    /// <summary>
    /// 检查分类是否展开
    /// </summary>
    public bool IsCategoryExpanded(ControlCategory category)
    {
        return ExpandedCategories.Contains(category);
    }

    protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(SearchKeyword))
        {
            Search();
        }
    }
}

/// <summary>
/// 控件分类分组
/// </summary>
public class ControlCategoryGroup
{
    /// <summary>
    /// 分类
    /// </summary>
    public ControlCategory Category { get; set; }

    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 控件数量
    /// </summary>
    public int Count { get; set; }
}
