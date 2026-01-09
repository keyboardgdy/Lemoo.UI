namespace Lemoo.Core.Common.Exceptions;

/// <summary>
/// 验证异常
/// </summary>
public class ValidationException : BusinessException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }
    
    public ValidationException() 
        : base("一个或多个验证错误发生")
    {
        Errors = new Dictionary<string, string[]>();
    }
    
    public ValidationException(IReadOnlyDictionary<string, string[]> errors) 
        : this()
    {
        Errors = errors;
    }
    
    public ValidationException(string propertyName, string errorMessage) 
        : this()
    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, new[] { errorMessage } }
        };
    }
}

