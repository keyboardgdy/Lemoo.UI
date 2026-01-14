namespace Lemoo.UI.Abstractions;

/// <summary>
/// 页面注册表接口
/// </summary>
public interface IPageRegistry
{
    /// <summary>
    /// 注册页面
    /// </summary>
    void RegisterPage(string pageKey, Type pageType, NavigationItemMetadata metadata);

    /// <summary>
    /// 根据页面键获取页面类型
    /// </summary>
    Type? GetPageType(string pageKey);

    /// <summary>
    /// 创建页面实例（返回 object，由实现层转换为具体类型）
    /// </summary>
    object? CreatePage(string pageKey, IServiceProvider serviceProvider);

    /// <summary>
    /// 根据模块名称获取所有页面
    /// </summary>
    IEnumerable<NavigationItemMetadata> GetPagesByModule(string moduleName);

    /// <summary>
    /// 获取所有已注册的页面元数据
    /// </summary>
    IReadOnlyDictionary<string, NavigationItemMetadata> GetAllPages();
}
