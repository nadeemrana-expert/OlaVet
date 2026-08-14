// =============================================
// File: OlaVet.Application/Services/Interfaces/ITokenService.cs
// Token generation service interface
// =============================================

using OlaVet.Domain.Entities;

namespace OlaVet.Application.Services.Interfaces;

/// <summary>
/// Service for JWT token generation and validation
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generate a JWT access token for a user
    /// </summary>
    string GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
    
    /// <summary>
    /// Generate a secure refresh token
    /// </summary>
    string GenerateRefreshToken();
    
    /// <summary>
    /// Get the user ID from an expired access token (for refresh flow)
    /// </summary>
    int? GetUserIdFromExpiredToken(string token);
}
