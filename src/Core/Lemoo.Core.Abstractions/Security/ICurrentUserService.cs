namespace Lemoo.Core.Abstractions.Security;

/// <summary>
/// 当前用户服务接口 - 提供当前认证用户信息
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// 当前用户ID
    /// </summary>
    string? UserId { get; }
    
    /// <summary>
    /// 当前用户名
    /// </summary>
    string? UserName { get; }
    
    /// <summary>
    /// 当前用户邮箱
    /// </summary>
    string? Email { get; }
    
    /// <summary>
    /// 是否已认证
    /// </summary>
    bool IsAuthenticated { get; }
    
    /// <summary>
    /// 用户角色列表
    /// </summary>
    IReadOnlyList<string> Roles { get; }
    
    /// <summary>
    /// 用户声明（Claims）
    /// </summary>
    IReadOnlyDictionary<string, string> Claims { get; }
    
    /// <summary>
    /// 检查用户是否在指定角色中
    /// </summary>
    bool IsInRole(string role);
    
    /// <summary>
    /// 检查用户是否有指定权限
    /// </summary>
    bool HasPermission(string permission);
    
    /// <summary>
    /// 获取声明值
    /// </summary>
    string? GetClaimValue(string claimType);
}

