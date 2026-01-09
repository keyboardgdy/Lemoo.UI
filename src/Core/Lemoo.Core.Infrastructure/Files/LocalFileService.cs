using Lemoo.Core.Abstractions.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Lemoo.Core.Infrastructure.Files;

/// <summary>
/// 本地文件服务实现 - 基于文件系统
/// </summary>
public class LocalFileService : IFileService
{
    private readonly string _basePath;
    private readonly ILogger<LocalFileService> _logger;

    public LocalFileService(IConfiguration configuration, ILogger<LocalFileService> logger)
    {
        _basePath = configuration.GetValue<string>("Lemoo:Files:BasePath") 
            ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
        _logger = logger;
        
        // 确保基础目录存在
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
            _logger.LogInformation("创建文件存储目录: {BasePath}", _basePath);
        }
    }

    public async Task<string> UploadAsync(
        Stream fileStream, 
        string fileName, 
        string? contentType = null,
        string? folder = null,
        CancellationToken cancellationToken = default)
    {
        if (fileStream == null || fileStream.Length == 0)
            throw new ArgumentException("文件流不能为空", nameof(fileStream));
        
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("文件名不能为空", nameof(fileName));

        // 生成唯一文件ID
        var fileId = GenerateFileId(fileName);
        var directory = string.IsNullOrWhiteSpace(folder) 
            ? _basePath 
            : Path.Combine(_basePath, folder);
        
        // 确保目录存在
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var filePath = Path.Combine(directory, fileId);
        
        // 保存文件
        using (var fileFileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await fileStream.CopyToAsync(fileFileStream, cancellationToken);
        }
        
        // 保存文件元数据
        await SaveFileMetadataAsync(fileId, fileName, contentType, fileStream.Length, folder, cancellationToken);
        
        _logger.LogInformation("文件上传成功: {FileId}, 文件名: {FileName}, 大小: {Size} bytes", 
            fileId, fileName, fileStream.Length);
        
        return fileId;
    }

    public async Task<FileDownloadResult> DownloadAsync(
        string fileId, 
        CancellationToken cancellationToken = default)
    {
        var filePath = GetFilePath(fileId);
        
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"文件不存在: {fileId}");

        var metadata = await GetFileMetadataAsync(fileId, cancellationToken);
        var fileSystemInfo = new System.IO.FileInfo(filePath);
        
        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        
        return new FileDownloadResult
        {
            Stream = stream,
            FileName = metadata?.FileName ?? Path.GetFileName(filePath),
            ContentType = metadata?.ContentType ?? "application/octet-stream",
            ContentLength = fileSystemInfo.Length,
            LastModified = fileSystemInfo.LastWriteTime,
            ETag = GenerateETag(filePath)
        };
    }

    public async Task DeleteAsync(
        string fileId, 
        CancellationToken cancellationToken = default)
    {
        var filePath = GetFilePath(fileId);
        
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            _logger.LogInformation("文件删除成功: {FileId}", fileId);
        }
        
        // 删除元数据
        await DeleteFileMetadataAsync(fileId, cancellationToken);
    }

    public Task<bool> ExistsAsync(
        string fileId, 
        CancellationToken cancellationToken = default)
    {
        var filePath = GetFilePath(fileId);
        return Task.FromResult(File.Exists(filePath));
    }

    public async Task<Abstractions.Files.FileInfo?> GetFileInfoAsync(
        string fileId, 
        CancellationToken cancellationToken = default)
    {
        var filePath = GetFilePath(fileId);
        
        if (!File.Exists(filePath))
            return null;

        var metadata = await GetFileMetadataAsync(fileId, cancellationToken);
        var fileSystemInfo = new System.IO.FileInfo(filePath);
        
        return new Abstractions.Files.FileInfo
        {
            FileId = fileId,
            FileName = metadata?.FileName ?? Path.GetFileName(filePath),
            ContentType = metadata?.ContentType ?? "application/octet-stream",
            Size = fileSystemInfo.Length,
            CreatedAt = fileSystemInfo.CreationTime,
            LastModified = fileSystemInfo.LastWriteTime,
            Folder = metadata?.Folder
        };
    }

    public Task<string> GetFileUrlAsync(
        string fileId, 
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        // 本地文件服务返回文件路径
        // 实际应用中可能需要生成临时访问URL
        var filePath = GetFilePath(fileId);
        return Task.FromResult(filePath);
    }

    private string GetFilePath(string fileId)
    {
        // 尝试从元数据获取文件夹信息
        var metadataPath = GetMetadataPath(fileId);
        if (File.Exists(metadataPath))
        {
            // 这里简化处理，实际应该读取元数据文件
            // 为了简化，假设文件直接存储在basePath下
        }
        
        return Path.Combine(_basePath, fileId);
    }

    private string GenerateFileId(string fileName)
    {
        var timestamp = DateTime.UtcNow.Ticks;
        var hash = ComputeHash($"{timestamp}_{fileName}");
        var extension = Path.GetExtension(fileName);
        return $"{hash}{extension}";
    }

    private string ComputeHash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "")
            .Substring(0, 16);
    }

    private string GenerateETag(string filePath)
    {
        var fileSystemInfo = new System.IO.FileInfo(filePath);
        var input = $"{filePath}_{fileSystemInfo.LastWriteTime.Ticks}_{fileSystemInfo.Length}";
        return ComputeHash(input);
    }

    private string GetMetadataPath(string fileId)
    {
        return Path.Combine(_basePath, $"{fileId}.meta");
    }

    private async Task SaveFileMetadataAsync(
        string fileId, 
        string fileName, 
        string? contentType, 
        long size,
        string? folder,
        CancellationToken cancellationToken)
    {
        var metadata = new
        {
            FileId = fileId,
            FileName = fileName,
            ContentType = contentType,
            Size = size,
            Folder = folder,
            CreatedAt = DateTime.UtcNow
        };
        
        var metadataPath = GetMetadataPath(fileId);
        var json = System.Text.Json.JsonSerializer.Serialize(metadata);
        await File.WriteAllTextAsync(metadataPath, json, cancellationToken);
    }

    private async Task<FileMetadata?> GetFileMetadataAsync(string fileId, CancellationToken cancellationToken)
    {
        var metadataPath = GetMetadataPath(fileId);
        if (!File.Exists(metadataPath))
            return null;

        var json = await File.ReadAllTextAsync(metadataPath, cancellationToken);
        return System.Text.Json.JsonSerializer.Deserialize<FileMetadata>(json);
    }

    private Task DeleteFileMetadataAsync(string fileId, CancellationToken cancellationToken)
    {
        var metadataPath = GetMetadataPath(fileId);
        if (File.Exists(metadataPath))
        {
            File.Delete(metadataPath);
        }
        return Task.CompletedTask;
    }

    private class FileMetadata
    {
        public string FileId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public long Size { get; set; }
        public string? Folder { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

