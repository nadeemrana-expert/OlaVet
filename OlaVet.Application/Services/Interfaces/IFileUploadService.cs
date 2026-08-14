// =============================================
// File: OlaVet.Application/Services/Interfaces/IFileUploadService.cs
// File upload service interface
// =============================================

using Microsoft.AspNetCore.Http;
using OlaVet.Application.Common;
using OlaVet.Application.DTOs.Auth;

namespace OlaVet.Application.Services.Interfaces;

/// <summary>
/// Service for secure file uploads with validation
/// </summary>
public interface IFileUploadService
{
    /// <summary>
    /// Upload a file with security validation (type, size, content scanning)
    /// </summary>
    Task<Result<FileUploadResultDto>> UploadFileAsync(IFormFile file, string? subfolder = null);
    
    /// <summary>
    /// Delete an uploaded file
    /// </summary>
    Task<Result<bool>> DeleteFileAsync(string storedFileName);
    
    /// <summary>
    /// Get a signed URL for a file (for S3 storage)
    /// </summary>
    Task<Result<string>> GetSignedUrlAsync(string storedFileName);
}
