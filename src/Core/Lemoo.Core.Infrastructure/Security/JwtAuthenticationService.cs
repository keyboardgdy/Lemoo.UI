using Lemoo.Core.Abstractions.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Lemoo.Core.Infrastructure.Security;

/// <summary>
/// JWT认证服务实现
/// </summary>
public class JwtAuthenticationService : IAuthenticationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtAuthenticationService> _logger;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public JwtAuthenticationService(
        IConfiguration configuration,
        ILogger<JwtAuthenticationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _secretKey = configuration.GetValue<string>("Lemoo:Authentication:Jwt:SecretKey") 
            ?? GenerateSecretKey();
        _issuer = configuration.GetValue<string>("Lemoo:Authentication:Jwt:Issuer") 
            ?? "Lemoo";
        _audience = configuration.GetValue<string>("Lemoo:Authentication:Jwt:Audience") 
            ?? "Lemoo";
        _expirationMinutes = configuration.GetValue<int>("Lemoo:Authentication:Jwt:ExpirationMinutes", 60);
    }

    public Task<AuthenticationResult> LoginAsync(
        string username, 
        string password, 
        CancellationToken cancellationToken = default)
    {
        // 这里简化处理，实际应该验证用户名和密码
        // 实际应用中应该查询用户数据库
        
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return Task.FromResult(AuthenticationResult.Failure("用户名或密码不能为空"));
        }

        // TODO: 实际应用中应该验证用户名和密码
        // var user = await _userRepository.GetByUsernameAsync(username);
        // if (user == null || !VerifyPassword(password, user.PasswordHash))
        // {
        //     return AuthenticationResult.Failure("用户名或密码错误");
        // }

        try
        {
            var token = GenerateToken(username, new Dictionary<string, string>
            {
                { ClaimTypes.Name, username },
                { ClaimTypes.NameIdentifier, Guid.NewGuid().ToString() }
            });

            var refreshToken = GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddMinutes(_expirationMinutes);

            _logger.LogInformation("用户登录成功: {Username}", username);

            return Task.FromResult(AuthenticationResult.Success(
                token, 
                refreshToken, 
                expiresAt,
                new Dictionary<string, string>
                {
                    { ClaimTypes.Name, username }
                }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成令牌失败: {Username}", username);
            return Task.FromResult(AuthenticationResult.Failure("登录失败"));
        }
    }

    public Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        // JWT是无状态的，登出通常由客户端删除令牌
        // 如果需要服务端登出，可以使用令牌黑名单
        _logger.LogInformation("用户登出");
        return Task.CompletedTask;
    }

    public Task<AuthenticationResult> RefreshTokenAsync(
        string refreshToken, 
        CancellationToken cancellationToken = default)
    {
        // TODO: 验证刷新令牌
        // 实际应用中应该验证刷新令牌的有效性
        
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Task.FromResult(AuthenticationResult.Failure("刷新令牌不能为空"));
        }

        try
        {
            // 从刷新令牌中提取用户信息
            // 这里简化处理
            var newToken = GenerateToken("user", new Dictionary<string, string>());
            var newRefreshToken = GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddMinutes(_expirationMinutes);

            return Task.FromResult(AuthenticationResult.Success(newToken, newRefreshToken, expiresAt));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刷新令牌失败");
            return Task.FromResult(AuthenticationResult.Failure("刷新令牌失败"));
        }
    }

    public Task<bool> ValidateTokenAsync(
        string token, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            tokenHandler.ValidateToken(token, validationParameters, out _);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private string GenerateToken(string username, Dictionary<string, string> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        var tokenClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        foreach (var claim in claims)
        {
            tokenClaims.Add(new Claim(claim.Key, claim.Value));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(tokenClaims),
            Expires = DateTime.UtcNow.AddMinutes(_expirationMinutes),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GenerateSecretKey()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}

