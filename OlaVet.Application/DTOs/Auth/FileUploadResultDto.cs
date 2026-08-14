// =============================================
// File: OlaVet.Application/DTOs/Auth/FileUploadResultDto.cs
// DTO for file upload result
// =============================================

namespace OlaVet.Application.DTOs.Auth;

public class FileUploadResultDto
{
    public string FileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string? Url { get; set; }
    public long FileSizeBytes { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
