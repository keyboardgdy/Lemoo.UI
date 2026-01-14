namespace Lemoo.Core.Abstractions.Persistence;

/// <summary>
/// 分页请求参数
/// </summary>
public record PagedRequest(int PageIndex, int PageSize)
{
    /// <summary>
    /// 默认分页参数
    /// </summary>
    public static PagedRequest Default => new(1, 20);

    /// <summary>
    /// 计算跳过的记录数
    /// </summary>
    public int Skip => (PageIndex - 1) * PageSize;

    /// <summary>
    /// 获取记录数
    /// </summary>
    public int Take => PageSize;
}

/// <summary>
/// 分页结果
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int PageIndex, int PageSize)
{
    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否有上一页
    /// </summary>
    public bool HasPreviousPage => PageIndex > 1;

    /// <summary>
    /// 是否有下一页
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPages;

    /// <summary>
    /// 创建空的分页结果
    /// </summary>
    public static PagedResult<T> Empty(PagedRequest request) =>
        new(Enumerable.Empty<T>(), 0, request.PageIndex, request.PageSize);

    /// <summary>
    /// 从列表创建分页结果
    /// </summary>
    public static PagedResult<T> Create(IEnumerable<T> items, int totalCount, PagedRequest request) =>
        new(items, totalCount, request.PageIndex, request.PageSize);
}

/// <summary>
/// 分页列表结果（使用 List 而非 IEnumerable）
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public record PagedList<T>(IReadOnlyList<T> Items, int TotalCount, int PageIndex, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public static PagedList<T> Empty(PagedRequest request) =>
        new(Array.Empty<T>(), 0, request.PageIndex, request.PageSize);

    public static PagedList<T> Create(IReadOnlyList<T> items, int totalCount, PagedRequest request) =>
        new(items, totalCount, request.PageIndex, request.PageSize);
}
