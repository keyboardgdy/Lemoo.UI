namespace Lemoo.Core.Abstractions.Security;

/// <summary>
/// 认证服务接口 - 提供用户认证功能
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="password">密码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>认证结果，包含访问令牌</returns>
    Task<AuthenticationResult> LoginAsync(
        string username, 
        string password, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 登出
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    Task LogoutAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 刷新令牌
    /// </summary>
    /// <param name="refreshToken">刷新令牌</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>新的认证结果</returns>
    Task<AuthenticationResult> RefreshTokenAsync(
        string refreshToken, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 验证令牌
    /// </summary>
    /// <param name="token">访问令牌</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>验证结果</returns>
    Task<bool> ValidateTokenAsync(
        string token, 
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 认证结果
/// </summary>
public class AuthenticationResult
{
    public bool IsSuccess { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? ErrorMessage { get; set; }
    public IReadOnlyDictionary<string, string>? Claims { get; set; }
    
    public static AuthenticationResult Success(
        string accessToken, 
        string? refreshToken = null, 
        DateTime? expiresAt = null,
        IReadOnlyDictionary<string, string>? claims = null)
    {
        return new AuthenticationResult
        {
            IsSuccess = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            Claims = claims
        };
    }
    
    public static AuthenticationResult Failure(string errorMessage)
    {
        return new AuthenticationResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

