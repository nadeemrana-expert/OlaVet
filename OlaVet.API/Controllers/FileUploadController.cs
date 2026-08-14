// =============================================
// File: OlaVet.API/Controllers/FileUploadController.cs
// Secure file upload API endpoints
// =============================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OlaVet.Application.DTOs.Auth;
using OlaVet.Application.Services.Interfaces;

namespace OlaVet.API.Controllers;

/// <summary>
/// File upload controller with security validation
/// Rate limited to prevent abuse
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[EnableRateLimiting("FileUploadRateLimit")]
public class FileUploadController : ControllerBase
{
    private readonly IFileUploadService _fileUploadService;
    private readonly ILogger<FileUploadController> _logger;

    public FileUploadController(IFileUploadService fileUploadService, ILogger<FileUploadController> logger)
    {
        _fileUploadService = fileUploadService;
        _logger = logger;
    }

    /// <summary>
    /// Upload a file with security validation
    /// POST: api/fileupload
    /// </summary>
    [HttpPost]
    [RequestSizeLimit(5 * 1024 * 1024)] // 5 MB limit at ASP.NET Core level
    [ProducesResponseType(typeof(FileUploadResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string? subfolder = null)
    {
        var result = await _fileUploadService.UploadFileAsync(file, subfolder);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Delete an uploaded file
    /// DELETE: api/fileupload/{fileName}
    /// </summary>
    [HttpDelete("{fileName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string fileName)
    {
        var result = await _fileUploadService.DeleteFileAsync(fileName);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(new { message = "File deleted successfully" });
    }
}
