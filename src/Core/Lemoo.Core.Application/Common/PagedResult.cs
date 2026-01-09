namespace Lemoo.Core.Application.Common;

/// <summary>
/// 分页结果
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class PagedResult<T> : Result<IEnumerable<T>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    
    /// <summary>
    /// 异步数据流（用于大数据集流式处理）
    /// </summary>
    public IAsyncEnumerable<T>? AsyncData { get; set; }
    
    public PagedResult(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
        : base(true, data)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
    
    /// <summary>
    /// 创建分页结果（同步数据）
    /// </summary>
    public static PagedResult<T> Create(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
    {
        return new PagedResult<T>(data, pageNumber, pageSize, totalCount);
    }
    
    /// <summary>
    /// 创建分页结果（异步数据流）
    /// </summary>
    public static PagedResult<T> CreateAsync(IAsyncEnumerable<T> asyncData, int pageNumber, int pageSize, int totalCount)
    {
        return new PagedResult<T>(asyncData, pageNumber, pageSize, totalCount);
    }
    
    /// <summary>
    /// 异步数据流构造函数
    /// </summary>
    public PagedResult(IAsyncEnumerable<T> asyncData, int pageNumber, int pageSize, int totalCount)
        : base(true, default(IEnumerable<T>)) // Data为null，因为使用AsyncData
    {
        AsyncData = asyncData;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
    
    /// <summary>
    /// 创建空分页结果
    /// </summary>
    public static PagedResult<T> Empty(int pageNumber = 1, int pageSize = 10)
    {
        return new PagedResult<T>(Array.Empty<T>(), pageNumber, pageSize, 0);
    }
}

