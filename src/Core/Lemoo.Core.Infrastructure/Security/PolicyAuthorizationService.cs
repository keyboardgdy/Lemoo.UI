using Lemoo.Core.Abstractions.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lemoo.Core.Infrastructure.Security;

/// <summary>
/// 基于策略的授权服务实现
/// </summary>
public class PolicyAuthorizationService : IAuthorizationService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PolicyAuthorizationService> _logger;
    private readonly Dictionary<string, List<string>> _policyPermissions = new();

    public PolicyAuthorizationService(
        ICurrentUserService currentUserService,
        IConfiguration configuration,
        ILogger<PolicyAuthorizationService> logger)
    {
        _currentUserService = currentUserService;
        _configuration = configuration;
        _logger = logger;
        LoadPolicies();
    }

    public async Task<bool> HasPermissionAsync(
        string permission, 
        string? userId = null, 
        CancellationToken cancellationToken = default)
    {
        userId ??= _currentUserService.UserId;
        if (string.IsNullOrWhiteSpace(userId))
            return false;

        // TODO: 实际应用中应该查询用户权限数据库
        // 这里简化处理，从配置或内存中检查
        
        var userPermissions = await GetUserPermissionsAsync(userId, cancellationToken);
        return userPermissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<bool> IsInRoleAsync(
        string role, 
        string? userId = null, 
        CancellationToken cancellationToken = default)
    {
        userId ??= _currentUserService.UserId;
        if (string.IsNullOrWhiteSpace(userId))
            return false;

        if (userId == _currentUserService.UserId)
        {
            return _currentUserService.IsInRole(role);
        }

        // TODO: 实际应用中应该查询用户角色数据库
        return await Task.FromResult(false);
    }

    public async Task<bool> AuthorizeAsync(
        string resource, 
        string action, 
        string? userId = null, 
        CancellationToken cancellationToken = default)
    {
        userId ??= _currentUserService.UserId;
        if (string.IsNullOrWhiteSpace(userId))
            return false;

        // 构建权限字符串: resource:action
        var permission = $"{resource}:{action}";
        return await HasPermissionAsync(permission, userId, cancellationToken);
    }

    private async Task<IReadOnlyList<string>> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken)
    {
        // TODO: 实际应用中应该从数据库查询
        // 这里简化处理，返回空列表
        return await Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
    }

    private void LoadPolicies()
    {
        var policiesSection = _configuration.GetSection("Lemoo:Authorization:Policies");
        foreach (var policy in policiesSection.GetChildren())
        {
            var permissions = policy.GetSection("Permissions").Get<string[]>() ?? Array.Empty<string>();
            _policyPermissions[policy.Key] = permissions.ToList();
        }
    }
}

