using Lemoo.Core.Abstractions.Security;

namespace Lemoo.Core.Infrastructure.Security;

/// <summary>
/// 用户上下文实现
/// </summary>
public class UserContext : IUserContext
{
    private readonly ICurrentUserService _currentUserService;

    public UserContext(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public string? GetUserId() => _currentUserService.UserId;

    public string? GetUserName() => _currentUserService.UserName;

    public string? GetEmail() => _currentUserService.Email;

    public bool IsInRole(string role) => _currentUserService.IsInRole(role);

    public bool HasPermission(string permission) => _currentUserService.HasPermission(permission);

    public string? GetClaimValue(string claimType) => _currentUserService.GetClaimValue(claimType);
}

