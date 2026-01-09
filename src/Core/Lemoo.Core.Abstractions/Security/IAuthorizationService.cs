namespace Lemoo.Core.Abstractions.Security;

/// <summary>
/// 授权服务接口 - 提供权限检查功能
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// 检查用户是否有指定权限
    /// </summary>
    /// <param name="permission">权限名称</param>
    /// <param name="userId">用户ID（可选，默认使用当前用户）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否有权限</returns>
    Task<bool> HasPermissionAsync(
        string permission, 
        string? userId = null, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 检查用户是否在指定角色中
    /// </summary>
    /// <param name="role">角色名称</param>
    /// <param name="userId">用户ID（可选，默认使用当前用户）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否在角色中</returns>
    Task<bool> IsInRoleAsync(
        string role, 
        string? userId = null, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 检查用户是否有指定资源权限
    /// </summary>
    /// <param name="resource">资源标识</param>
    /// <param name="action">操作</param>
    /// <param name="userId">用户ID（可选，默认使用当前用户）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否有权限</returns>
    Task<bool> AuthorizeAsync(
        string resource, 
        string action, 
        string? userId = null, 
        CancellationToken cancellationToken = default);
}

