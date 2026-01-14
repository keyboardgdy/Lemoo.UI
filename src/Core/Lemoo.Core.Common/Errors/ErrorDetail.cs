namespace Lemoo.Core.Common.Errors;

/// <summary>
/// 错误详情
/// </summary>
public record ErrorDetail
{
    /// <summary>
    /// 错误码
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// 详细描述
    /// </summary>
    public string? Details { get; init; }

    /// <summary>
    /// 元数据
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }

    /// <summary>
    /// 创建错误详情
    /// </summary>
    public static ErrorDetail Create(
        string code,
        string message,
        string? details = null,
        Dictionary<string, object>? metadata = null)
    {
        return new ErrorDetail
        {
            Code = code,
            Message = message,
            Details = details,
            Metadata = metadata
        };
    }

    /// <summary>
    /// 添加元数据
    /// </summary>
    public ErrorDetail WithMetadata(string key, object value)
    {
        var metadata = Metadata ?? new Dictionary<string, object>();
        metadata[key] = value;
        return this with { Metadata = metadata };
    }

    /// <summary>
    /// 添加详细信息
    /// </summary>
    public ErrorDetail WithDetails(string details)
    {
        return this with { Details = details };
    }
}
