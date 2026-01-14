using Microsoft.Extensions.DependencyInjection;

namespace Lemoo.UI.Abstractions;

/// <summary>
/// 模块UI接口
/// </summary>
public interface IModuleUI
{
    /// <summary>
    /// 模块名称（对应后端模块名称）
    /// </summary>
    string ModuleName { get; }

    /// <summary>
    /// 注册UI组件（页面、视图模型等）
    /// </summary>
    void RegisterUI(IServiceCollection services);

    /// <summary>
    /// 获取导航项配置
    /// </summary>
    IReadOnlyList<NavigationItemMetadata> GetNavigationItems();
}
