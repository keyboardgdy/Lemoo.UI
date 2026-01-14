using Lemoo.UI.Abstractions;
using Lemoo.UI.WPF.Models;
using Lemoo.UI.WPF.Abstractions;

namespace Lemoo.UI.WPF.Services;

/// <summary>
/// 导航服务实现
/// </summary>
public class NavigationService : INavigationService
{
    private readonly IPageRegistry _pageRegistry;

    public NavigationService(IPageRegistry pageRegistry)
    {
        _pageRegistry = pageRegistry ?? throw new ArgumentNullException(nameof(pageRegistry));
    }

    /// <summary>
    /// 构建导航树并更新主视图模型
    /// </summary>
    public void BuildNavigationTree(IMainViewModel mainViewModel, IEnumerable<NavigationItemMetadata> navItems)
    {
        if (mainViewModel == null)
            throw new ArgumentNullException(nameof(mainViewModel));

        // 清空现有导航项
        mainViewModel.NavigationItems.Clear();
        mainViewModel.BottomNavigationItems.Clear();

        // 按模块和Order排序
        var sortedItems = navItems
            .Where(item => item.IsEnabled)
            .OrderBy(item => item.Module)
            .ThenBy(item => item.Order)
            .ToList();

        // 构建导航树
        var rootItems = sortedItems.Where(item => string.IsNullOrEmpty(item.ParentPageKey)).ToList();
        var childItems = sortedItems.Where(item => !string.IsNullOrEmpty(item.ParentPageKey))
            .GroupBy(item => item.ParentPageKey!)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var rootItem in rootItems)
        {
            var navItem = CreateNavigationItem(rootItem);

            // 添加子项
            if (childItems.TryGetValue(rootItem.PageKey, out var children))
            {
                foreach (var child in children.OrderBy(c => c.Order))
                {
                    navItem.Children.Add(CreateNavigationItem(child));
                }
            }

            mainViewModel.NavigationItems.Add(navItem);
        }
    }

    /// <summary>
    /// 从页面注册表构建导航树
    /// </summary>
    public void BuildNavigationTreeFromRegistry(IMainViewModel mainViewModel, IPageRegistry pageRegistry)
    {
        if (pageRegistry == null)
            throw new ArgumentNullException(nameof(pageRegistry));

        var allPages = pageRegistry.GetAllPages().Values;
        BuildNavigationTree(mainViewModel, allPages);
    }

    private static NavigationItem CreateNavigationItem(NavigationItemMetadata metadata)
    {
        return new NavigationItem
        {
            Title = metadata.Title,
            Icon = metadata.Icon,
            PageKey = metadata.PageKey
        };
    }
}

