// =============================================
// File: OlaVet.Application/Services/Interfaces/IAuthService.cs
// Authentication service interface
// =============================================

using OlaVet.Application.Common;
using OlaVet.Application.DTOs.Auth;

namespace OlaVet.Application.Services.Interfaces;

/// <summary>
/// Service for authentication operations (login, register, token management)
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Register a new user
    /// </summary>
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto, string? ipAddress = null);
    
    /// <summary>
    /// Login with email and password
    /// </summary>
    Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto, string? ipAddress = null);
    
    /// <summary>
    /// Refresh an access token using a refresh token (with rotation)
    /// </summary>
    Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto, string? ipAddress = null);
    
    /// <summary>
    /// Revoke a refresh token (logout)
    /// </summary>
    Task<Result<bool>> RevokeTokenAsync(string refreshToken, string? ipAddress = null);
    
    /// <summary>
    /// Change password for authenticated user
    /// </summary>
    Task<Result<bool>> ChangePasswordAsync(int userId, ChangePasswordDto dto);
    
    /// <summary>
    /// Get current user info
    /// </summary>
    Task<Result<AuthResponseDto>> GetCurrentUserAsync(int userId);
    
    /// <summary>
    /// Admin-only: Create a staff account (LabTechnician, StoreManager)
    /// Links to an existing domain entity
    /// </summary>
    Task<Result<AuthResponseDto>> CreateStaffAccountAsync(CreateStaffAccountDto dto);
}
