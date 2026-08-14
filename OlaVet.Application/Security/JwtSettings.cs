// =============================================
// File: OlaVet.Application/Security/JwtSettings.cs
// JWT configuration settings model
// =============================================

namespace OlaVet.Application.Security;

/// <summary>
/// JWT configuration loaded from appsettings.json
/// </summary>
public class JwtSettings
{
    public const string SectionName = "JwtSettings";
    
    /// <summary>
    /// Secret key for signing JWT tokens (min 256 bits / 32 chars)
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Token issuer (your API domain)
    /// </summary>
    public string Issuer { get; set; } = string.Empty;
    
    /// <summary>
    /// Token audience (your client app domain)
    /// </summary>
    public string Audience { get; set; } = string.Empty;
    
    /// <summary>
    /// Access token expiry in hours (default: 24)
    /// </summary>
    public int AccessTokenExpiryHours { get; set; } = 24;
    
    /// <summary>
    /// Refresh token expiry in days (default: 7)
    /// </summary>
    public int RefreshTokenExpiryDays { get; set; } = 7;
}
