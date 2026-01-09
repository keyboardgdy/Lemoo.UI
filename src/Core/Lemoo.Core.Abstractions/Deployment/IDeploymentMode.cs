namespace Lemoo.Core.Abstractions.Deployment;

/// <summary>
/// 部署模式枚举
/// </summary>
public enum DeploymentMode
{
    /// <summary>
    /// 本地模式 - 直接调用服务
    /// </summary>
    Local,
    
    /// <summary>
    /// API模式 - 通过HTTP调用
    /// </summary>
    Api
}

/// <summary>
/// 部署模式服务接口
/// </summary>
public interface IDeploymentModeService
{
    /// <summary>
    /// 当前部署模式
    /// </summary>
    DeploymentMode CurrentMode { get; }
    
    /// <summary>
    /// 是否为本地模式
    /// </summary>
    bool IsLocalMode { get; }
    
    /// <summary>
    /// 是否为API模式
    /// </summary>
    bool IsApiMode { get; }
}

