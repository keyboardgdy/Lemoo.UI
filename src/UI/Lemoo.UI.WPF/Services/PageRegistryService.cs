using System.Windows.Controls;
using Lemoo.Core.Abstractions.UI;
using Microsoft.Extensions.DependencyInjection;

namespace Lemoo.UI.WPF.Services;

/// <summary>
/// 页面注册服务实现
/// </summary>
public class PageRegistryService : IPageRegistry
{
    private readonly Dictionary<string, Type> _pageTypes = new();
    private readonly Dictionary<string, NavigationItemMetadata> _metadata = new();

    public PageRegistryService()
    {
    }

    /// <summary>
    /// 注册页面
    /// </summary>
    public void RegisterPage(string pageKey, Type pageType, NavigationItemMetadata metadata)
    {
        if (string.IsNullOrWhiteSpace(pageKey))
            throw new ArgumentException("PageKey cannot be null or empty", nameof(pageKey));
        
        if (pageType == null)
            throw new ArgumentNullException(nameof(pageType));
        
        if (metadata == null)
            throw new ArgumentNullException(nameof(metadata));

        _pageTypes[pageKey] = pageType;
        _metadata[pageKey] = metadata;
    }

    /// <summary>
    /// 根据页面键获取页面类型
    /// </summary>
    public Type? GetPageType(string pageKey)
    {
        return _pageTypes.TryGetValue(pageKey, out var type) ? type : null;
    }

    /// <summary>
    /// 创建页面实例
    /// </summary>
    public object? CreatePage(string pageKey, IServiceProvider serviceProvider)
    {
        if (!_pageTypes.TryGetValue(pageKey, out var pageType))
            return null;

        try
        {
            // 使用依赖注入创建页面实例
            return ActivatorUtilities.CreateInstance(serviceProvider, pageType);
        }
        catch (Exception ex)
        {
            // 记录错误但不抛出异常，返回null让调用者处理
            System.Diagnostics.Debug.WriteLine($"创建页面失败 {pageKey}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 根据模块名称获取所有页面
    /// </summary>
    public IEnumerable<NavigationItemMetadata> GetPagesByModule(string moduleName)
    {
        return _metadata.Values.Where(p => p.Module == moduleName);
    }

    /// <summary>
    /// 获取所有已注册的页面元数据
    /// </summary>
    public IReadOnlyDictionary<string, NavigationItemMetadata> GetAllPages()
    {
        return _metadata.AsReadOnly();
    }
}

