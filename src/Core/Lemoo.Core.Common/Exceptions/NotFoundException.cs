namespace Lemoo.Core.Common.Exceptions;

/// <summary>
/// 未找到异常
/// </summary>
public class NotFoundException : BusinessException
{
    public NotFoundException(string entityName, object key) 
        : base($"实体 '{entityName}' (键: '{key}') 未找到")
    {
    }
    
    public NotFoundException(string message) : base(message)
    {
    }
}

