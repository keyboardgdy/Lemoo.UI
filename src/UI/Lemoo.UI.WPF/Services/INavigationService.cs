using Lemoo.UI.Abstractions;
using Lemoo.UI.WPF.Models;
using Lemoo.UI.WPF.Abstractions;

namespace Lemoo.UI.WPF.Services;

/// <summary>
/// 导航服务接口
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// 构建导航树并更新主视图模型
    /// </summary>
    /// <param name="mainViewModel">主视图模型</param>
    /// <param name="navItems">导航项元数据集合</param>
    void BuildNavigationTree(IMainViewModel mainViewModel, IEnumerable<NavigationItemMetadata> navItems);

    /// <summary>
    /// 从页面注册表构建导航树
    /// </summary>
    /// <param name="mainViewModel">主视图模型</param>
    /// <param name="pageRegistry">页面注册表</param>
    void BuildNavigationTreeFromRegistry(IMainViewModel mainViewModel, IPageRegistry pageRegistry);
}
