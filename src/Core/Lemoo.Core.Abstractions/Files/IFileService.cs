namespace Lemoo.Core.Abstractions.Files;

/// <summary>
/// 文件服务接口 - 提供文件上传、下载、删除等功能
/// </summary>
public interface IFileService
{
    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="fileStream">文件流</param>
    /// <param name="fileName">文件名</param>
    /// <param name="contentType">内容类型（MIME类型）</param>
    /// <param name="folder">存储文件夹（可选）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>文件ID或路径</returns>
    Task<string> UploadAsync(
        Stream fileStream, 
        string fileName, 
        string? contentType = null,
        string? folder = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="fileId">文件ID或路径</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>文件流和元数据</returns>
    Task<FileDownloadResult> DownloadAsync(
        string fileId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="fileId">文件ID或路径</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task DeleteAsync(
        string fileId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 检查文件是否存在
    /// </summary>
    /// <param name="fileId">文件ID或路径</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>文件是否存在</returns>
    Task<bool> ExistsAsync(
        string fileId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取文件信息
    /// </summary>
    /// <param name="fileId">文件ID或路径</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>文件信息</returns>
    Task<FileInfo?> GetFileInfoAsync(
        string fileId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取文件URL（用于直接访问）
    /// </summary>
    /// <param name="fileId">文件ID或路径</param>
    /// <param name="expiration">过期时间（可选）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>文件URL</returns>
    Task<string> GetFileUrlAsync(
        string fileId, 
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 文件下载结果
/// </summary>
public class FileDownloadResult
{
    public Stream Stream { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/octet-stream";
    public long ContentLength { get; set; }
    public DateTime? LastModified { get; set; }
    public string? ETag { get; set; }
}

/// <summary>
/// 文件信息
/// </summary>
public class FileInfo
{
    public string FileId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
    public string? Folder { get; set; }
}

