using Lemoo.Core.Abstractions.Security;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading;

namespace Lemoo.Core.Infrastructure.Security;

/// <summary>
/// 当前用户服务实现 - 基于ClaimsPrincipal（API模式）或线程本地存储（WPF模式）
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly ILogger<CurrentUserService> _logger;
    private readonly AsyncLocal<ClaimsPrincipal?> _currentUser = new();

    public CurrentUserService(ILogger<CurrentUserService> logger)
    {
        _logger = logger;
    }

    public string? UserId
    {
        get
        {
            var principal = GetPrincipal();
            return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? principal?.FindFirst("sub")?.Value;
        }
    }

    public string? UserName
    {
        get
        {
            var principal = GetPrincipal();
            return principal?.FindFirst(ClaimTypes.Name)?.Value
                ?? principal?.FindFirst("name")?.Value;
        }
    }

    public string? Email
    {
        get
        {
            var principal = GetPrincipal();
            return principal?.FindFirst(ClaimTypes.Email)?.Value
                ?? principal?.FindFirst("email")?.Value;
        }
    }

    public bool IsAuthenticated
    {
        get
        {
            var principal = GetPrincipal();
            return principal?.Identity?.IsAuthenticated ?? false;
        }
    }

    public IReadOnlyList<string> Roles
    {
        get
        {
            var principal = GetPrincipal();
            if (principal == null)
                return Array.Empty<string>();

            return principal.FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList()
                .AsReadOnly();
        }
    }

    public IReadOnlyDictionary<string, string> Claims
    {
        get
        {
            var principal = GetPrincipal();
            if (principal == null)
                return new Dictionary<string, string>();

            return principal.Claims
                .ToDictionary(c => c.Type, c => c.Value)
                .AsReadOnly();
        }
    }

    public bool IsInRole(string role)
    {
        var principal = GetPrincipal();
        return principal?.IsInRole(role) ?? false;
    }

    public bool HasPermission(string permission)
    {
        var principal = GetPrincipal();
        return principal?.HasClaim("permission", permission) ?? false;
    }

    public string? GetClaimValue(string claimType)
    {
        var principal = GetPrincipal();
        return principal?.FindFirst(claimType)?.Value;
    }

    /// <summary>
    /// 设置当前用户（用于WPF模式）
    /// </summary>
    public void SetCurrentUser(ClaimsPrincipal? principal)
    {
        _currentUser.Value = principal;
    }

    private ClaimsPrincipal? GetPrincipal()
    {
        // 从AsyncLocal获取（支持WPF和API模式）
        // 注意：API模式需要在中间件中设置_currentUser.Value
        return _currentUser.Value;
    }
}

