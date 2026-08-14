// =============================================
// File: OlaVet.API/Controllers/AuthController.cs
// Authentication & Authorization API endpoints
// =============================================

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OlaVet.Application.DTOs.Auth;
using OlaVet.Application.Services.Interfaces;

namespace OlaVet.API.Controllers;

/// <summary>
/// Authentication controller - handles registration, login, token refresh, and logout
/// Rate limited to prevent brute force attacks
/// </summary>
[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("AuthRateLimit")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user (PetOwner or Vet)
    /// POST: api/auth/register
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var ipAddress = GetIpAddress();
        var result = await _authService.RegisterAsync(dto, ipAddress);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        // Set refresh token in HTTP-only cookie (more secure than sending in body)
        SetRefreshTokenCookie(result.Data!.RefreshToken);

        return Ok(result.Data);
    }

    /// <summary>
    /// Login with email and password
    /// POST: api/auth/login
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var ipAddress = GetIpAddress();
        var result = await _authService.LoginAsync(dto, ipAddress);

        if (!result.IsSuccess)
        {
            return Unauthorized(new { error = result.Error });
        }

        SetRefreshTokenCookie(result.Data!.RefreshToken);

        return Ok(result.Data);
    }

    /// <summary>
    /// Refresh access token using refresh token (with rotation)
    /// POST: api/auth/refresh-token
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        // Try to get refresh token from cookie if not in body
        if (string.IsNullOrEmpty(dto.RefreshToken))
        {
            dto.RefreshToken = Request.Cookies["refreshToken"] ?? string.Empty;
        }

        var ipAddress = GetIpAddress();
        var result = await _authService.RefreshTokenAsync(dto, ipAddress);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        SetRefreshTokenCookie(result.Data!.RefreshToken);

        return Ok(result.Data);
    }

    /// <summary>
    /// Revoke refresh token (logout)
    /// POST: api/auth/revoke-token
    /// </summary>
    [HttpPost("revoke-token")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeToken([FromBody] string? refreshToken)
    {
        // Get token from cookie if not in body
        var token = refreshToken ?? Request.Cookies["refreshToken"];
        
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest(new { error = "Refresh token is required" });
        }

        var ipAddress = GetIpAddress();
        var result = await _authService.RevokeTokenAsync(token, ipAddress);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        // Clear the refresh token cookie
        Response.Cookies.Delete("refreshToken");

        return Ok(new { message = "Token revoked successfully" });
    }

    /// <summary>
    /// Change password for authenticated user
    /// POST: api/auth/change-password
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _authService.ChangePasswordAsync(userId.Value, dto);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        // Clear refresh token cookie (force re-login)
        Response.Cookies.Delete("refreshToken");

        return Ok(new { message = "Password changed successfully. Please login again." });
    }

    /// <summary>
    /// Get current authenticated user profile
    /// GET: api/auth/me
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _authService.GetCurrentUserAsync(userId.Value);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Data);
    }

    // =============================================
    // ADMIN-ONLY ENDPOINTS
    // =============================================

    /// <summary>
    /// Admin-only: Create a staff account (LabTechnician or StoreManager)
    /// POST: api/auth/create-staff-account
    /// </summary>
    [HttpPost("create-staff-account")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateStaffAccount([FromBody] CreateStaffAccountDto dto)
    {
        var result = await _authService.CreateStaffAccountAsync(dto);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Data);
    }

    // =============================================
    // PRIVATE HELPERS
    // =============================================

    /// <summary>
    /// Get the current user's ID from JWT claims
    /// </summary>
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
                          ?? User.FindFirst("sub");
        
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        
        return null;
    }

    /// <summary>
    /// Get client IP address (handles proxies)
    /// </summary>
    private string? GetIpAddress()
    {
        // Check for X-Forwarded-For header (when behind a proxy/load balancer)
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            return Request.Headers["X-Forwarded-For"].FirstOrDefault();
        }
        
        return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
    }

    /// <summary>
    /// Set refresh token as HTTP-only secure cookie
    /// HTTP-only: Can't be accessed by JavaScript (prevents XSS theft)
    /// Secure: Only sent over HTTPS
    /// SameSite: Prevents CSRF attacks
    /// </summary>
    private void SetRefreshTokenCookie(string refreshToken)
    {
        var isDevelopment = HttpContext.RequestServices
            .GetRequiredService<IHostEnvironment>().IsDevelopment();

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,                          // Not accessible via JavaScript
            Secure = !isDevelopment,                   // HTTPS only in production; HTTP OK in dev
            SameSite = isDevelopment                   // Lax in dev for cross-port, Strict in prod
                ? SameSiteMode.Lax 
                : SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}
