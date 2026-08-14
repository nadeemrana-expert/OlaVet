// =============================================
// File: OlaVet.Application/Security/FileUploadSettings.cs
// File upload security configuration
// =============================================

namespace OlaVet.Application.Security;

/// <summary>
/// File upload security settings loaded from appsettings.json
/// </summary>
public class FileUploadSettings
{
    public const string SectionName = "FileUploadSettings";
    
    /// <summary>
    /// Maximum file size in bytes (default: 5 MB)
    /// </summary>
    public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024;
    
    /// <summary>
    /// Allowed file extensions (e.g., ".jpg", ".png", ".pdf")
    /// </summary>
    public string[] AllowedExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx"];
    
    /// <summary>
    /// Allowed MIME types for extra validation
    /// </summary>
    public string[] AllowedMimeTypes { get; set; } = [
        "image/jpeg", "image/png", "image/gif",
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    ];
    
    /// <summary>
    /// Storage path for uploaded files
    /// </summary>
    public string StoragePath { get; set; } = "uploads";
    
    /// <summary>
    /// Whether to use S3 for storage
    /// </summary>
    public bool UseS3Storage { get; set; } = false;
    
    /// <summary>
    /// S3 bucket name (if using S3)
    /// </summary>
    public string? S3BucketName { get; set; }
    
    /// <summary>
    /// S3 signed URL expiry in minutes
    /// </summary>
    public int S3SignedUrlExpiryMinutes { get; set; } = 60;
}
