using Lemoo.Core.Abstractions.Deployment;
using Microsoft.Extensions.Configuration;

namespace Lemoo.Core.Infrastructure.Deployment;

/// <summary>
/// 部署模式服务实现
/// </summary>
public class DeploymentModeService : IDeploymentModeService
{
    public DeploymentMode CurrentMode { get; }
    public bool IsLocalMode => CurrentMode == DeploymentMode.Local;
    public bool IsApiMode => CurrentMode == DeploymentMode.Api;
    
    public DeploymentModeService(IConfiguration configuration)
    {
        var modeString = configuration.GetValue<string>("Lemoo:Mode") ?? "Local";
        CurrentMode = Enum.TryParse<DeploymentMode>(modeString, true, out var mode)
            ? mode
            : DeploymentMode.Local;
    }
}

