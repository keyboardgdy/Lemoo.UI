namespace Lemoo.Core.Abstractions.Security;

/// <summary>
/// 用户上下文接口 - 提供用户上下文信息
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    string? GetUserId();
    
    /// <summary>
    /// 获取当前用户名
    /// </summary>
    string? GetUserName();
    
    /// <summary>
    /// 获取当前用户邮箱
    /// </summary>
    string? GetEmail();
    
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

