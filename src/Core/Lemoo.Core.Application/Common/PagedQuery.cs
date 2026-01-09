using Lemoo.Core.Abstractions.CQRS;

namespace Lemoo.Core.Application.Common;

/// <summary>
/// 分页查询基类
/// </summary>
/// <typeparam name="TResponse">响应类型</typeparam>
public abstract class PagedQuery<TResponse> : IQuery<PagedResult<TResponse>>
{
    /// <summary>
    /// 页码（从1开始）
    /// </summary>
    public int PageNumber { get; set; } = 1;
    
    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// 最大每页大小
    /// </summary>
    public int MaxPageSize { get; set; } = 100;
    
    /// <summary>
    /// 排序字段
    /// </summary>
    public string? SortBy { get; set; }
    
    /// <summary>
    /// 排序方向（asc/desc）
    /// </summary>
    public string? SortDirection { get; set; } = "asc";
    
    /// <summary>
    /// 验证并规范化分页参数
    /// </summary>
    public virtual void Normalize()
    {
        if (PageNumber < 1)
            PageNumber = 1;
        
        if (PageSize < 1)
            PageSize = 10;
        
        if (PageSize > MaxPageSize)
            PageSize = MaxPageSize;
        
        if (string.IsNullOrWhiteSpace(SortDirection))
            SortDirection = "asc";
        else
            SortDirection = SortDirection.ToLowerInvariant();
        
        if (SortDirection != "asc" && SortDirection != "desc")
            SortDirection = "asc";
    }
}

