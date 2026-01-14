using System.Reflection;

namespace Lemoo.Core.Abstractions.Module;

/// <summary>
/// 模块发现策略接口
/// </summary>
public interface IModuleDiscoveryStrategy
{
    /// <summary>
    /// 发现模块程序集
    /// </summary>
    Task<IEnumerable<Assembly>> DiscoverModulesAsync(CancellationToken cancellationToken = default);
}
