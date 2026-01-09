namespace Lemoo.Core.Common.Exceptions;

/// <summary>
/// 业务异常基类
/// </summary>
public class BusinessException : Exception
{
    public string? ErrorCode { get; }
    
    public BusinessException(string message) : base(message)
    {
    }
    
    public BusinessException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
    
    public BusinessException(string errorCode, string message) 
        : base(message)
    {
        ErrorCode = errorCode;
    }
    
    public BusinessException(string errorCode, string message, Exception innerException) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

