namespace Lemoo.Core.Common.Exceptions;

/// <summary>
/// 未授权异常
/// </summary>
public class UnauthorizedException : BusinessException
{
    public UnauthorizedException(string message) : base(message)
    {
    }
    
    public UnauthorizedException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

