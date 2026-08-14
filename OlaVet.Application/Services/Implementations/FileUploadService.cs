// =============================================
// File: OlaVet.Application/Services/Implementations/FileUploadService.cs
// Secure file upload service implementation
// =============================================

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OlaVet.Application.Common;
using OlaVet.Application.DTOs.Auth;
using OlaVet.Application.Security;
using OlaVet.Application.Services.Interfaces;

namespace OlaVet.Application.Services.Implementations;

/// <summary>
/// Secure file upload service with:
/// - File type validation (extension + MIME type + magic bytes)
/// - Size limits
/// - Secure filename generation (prevents path traversal)
/// - Content scanning for dangerous patterns
/// </summary>
public class FileUploadService : IFileUploadService
{
    private readonly FileUploadSettings _settings;
    private readonly ILogger<FileUploadService> _logger;

    // Magic bytes for common file types (virus/content type verification)
    private static readonly Dictionary<string, byte[][]> FileSignatures = new()
    {
        { ".jpg", [new byte[] { 0xFF, 0xD8, 0xFF }] },
        { ".jpeg", [new byte[] { 0xFF, 0xD8, 0xFF }] },
        { ".png", [new byte[] { 0x89, 0x50, 0x4E, 0x47 }] },
        { ".gif", [new byte[] { 0x47, 0x49, 0x46, 0x38 }] },
        { ".pdf", [new byte[] { 0x25, 0x50, 0x44, 0x46 }] }
    };

    public FileUploadService(
        IOptions<FileUploadSettings> settings,
        ILogger<FileUploadService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Upload a file with comprehensive security validation
    /// </summary>
    public async Task<Result<FileUploadResultDto>> UploadFileAsync(IFormFile file, string? subfolder = null)
    {
        // 1. Validate file is not null or empty
        if (file == null || file.Length == 0)
        {
            return Result<FileUploadResultDto>.Failure("No file provided");
        }

        // 2. Validate file size
        if (file.Length > _settings.MaxFileSizeBytes)
        {
            var maxSizeMB = _settings.MaxFileSizeBytes / (1024 * 1024);
            return Result<FileUploadResultDto>.Failure($"File size exceeds maximum allowed size of {maxSizeMB} MB");
        }

        // 3. Validate file extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !_settings.AllowedExtensions.Contains(extension))
        {
            return Result<FileUploadResultDto>.Failure(
                $"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", _settings.AllowedExtensions)}");
        }

        // 4. Validate MIME type
        if (!_settings.AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
        {
            return Result<FileUploadResultDto>.Failure($"MIME type '{file.ContentType}' is not allowed");
        }

        // 5. Validate magic bytes (prevent extension spoofing)
        if (FileSignatures.TryGetValue(extension, out var signatures))
        {
            using var reader = new BinaryReader(file.OpenReadStream());
            var headerBytes = reader.ReadBytes(signatures.Max(s => s.Length));
            
            var isValidSignature = signatures.Any(signature =>
                headerBytes.Length >= signature.Length &&
                headerBytes.Take(signature.Length).SequenceEqual(signature));
            
            if (!isValidSignature)
            {
                _logger.LogWarning("File content does not match extension: {FileName}", file.FileName);
                return Result<FileUploadResultDto>.Failure("File content does not match its extension");
            }
        }

        // 6. Scan for potentially dangerous content (basic check)
        if (extension is ".doc" or ".docx")
        {
            using var streamReader = new StreamReader(file.OpenReadStream());
            var content = await streamReader.ReadToEndAsync();
            
            // Check for macros/scripts in documents
            if (content.Contains("<script", StringComparison.OrdinalIgnoreCase) ||
                content.Contains("vbscript", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Potentially malicious content detected in: {FileName}", file.FileName);
                return Result<FileUploadResultDto>.Failure("File contains potentially unsafe content");
            }
        }

        // 7. Generate secure filename (prevents path traversal attacks)
        var secureFileName = $"{Guid.NewGuid():N}{extension}";
        
        // 8. Build storage path
        var storagePath = _settings.StoragePath;
        if (!string.IsNullOrEmpty(subfolder))
        {
            // Sanitize subfolder to prevent path traversal
            subfolder = subfolder.Replace("..", "").Replace("/", "").Replace("\\", "");
            storagePath = Path.Combine(storagePath, subfolder);
        }

        // 9. Save file (local storage)
        if (!_settings.UseS3Storage)
        {
            Directory.CreateDirectory(storagePath);
            var filePath = Path.Combine(storagePath, secureFileName);
            
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            
            _logger.LogInformation("File uploaded: {StoredFileName} ({Size} bytes)", secureFileName, file.Length);
            
            return Result<FileUploadResultDto>.Success(new FileUploadResultDto
            {
                FileName = file.FileName,
                StoredFileName = secureFileName,
                FileSizeBytes = file.Length,
                ContentType = file.ContentType,
                UploadedAt = DateTime.UtcNow
            });
        }

        // S3 storage would go here (future implementation)
        return Result<FileUploadResultDto>.Failure("S3 storage is not yet implemented");
    }

    /// <summary>
    /// Delete an uploaded file
    /// </summary>
    public Task<Result<bool>> DeleteFileAsync(string storedFileName)
    {
        // Sanitize filename to prevent path traversal
        storedFileName = Path.GetFileName(storedFileName);
        var filePath = Path.Combine(_settings.StoragePath, storedFileName);

        if (!File.Exists(filePath))
        {
            return Task.FromResult(Result<bool>.Failure("File not found"));
        }

        File.Delete(filePath);
        _logger.LogInformation("File deleted: {StoredFileName}", storedFileName);
        return Task.FromResult(Result<bool>.Success(true));
    }

    /// <summary>
    /// Get a signed URL (placeholder for S3 implementation)
    /// </summary>
    public Task<Result<string>> GetSignedUrlAsync(string storedFileName)
    {
        if (!_settings.UseS3Storage)
        {
            return Task.FromResult(Result<string>.Failure("S3 storage is not configured"));
        }

        // S3 signed URL generation would go here
        return Task.FromResult(Result<string>.Failure("S3 signed URLs not yet implemented"));
    }
}
