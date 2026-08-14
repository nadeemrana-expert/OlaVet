// =============================================
// File: OlaVet.Domain/Entities/Core/RefreshToken.cs
// Refresh token entity for token rotation
// =============================================

namespace OlaVet.Domain.Entities;

/// <summary>
/// Refresh token for JWT token rotation
/// Each refresh token can only be used once (rotation pattern)
/// </summary>
public class RefreshToken
{
    public int RefreshTokenId { get; set; }
    
    /// <summary>
    /// The actual refresh token value (hashed)
    /// </summary>
    public string Token { get; set; } = string.Empty;
    
    /// <summary>
    /// When this token expires
    /// </summary>
    public DateTime ExpiresAt { get; set; }
    
    /// <summary>
    /// When this token was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// IP address that requested this token
    /// </summary>
    public string? CreatedByIp { get; set; }
    
    /// <summary>
    /// When this token was revoked (null if still valid)
    /// </summary>
    public DateTime? RevokedAt { get; set; }
    
    /// <summary>
    /// IP address that revoked this token
    /// </summary>
    public string? RevokedByIp { get; set; }
    
    /// <summary>
    /// The token that replaced this one (for rotation tracking)
    /// </summary>
    public string? ReplacedByToken { get; set; }
    
    /// <summary>
    /// Reason for revocation
    /// </summary>
    public string? RevokeReason { get; set; }
    
    // Foreign Key
    public int UserId { get; set; }
    
    // Navigation
    public virtual ApplicationUser User { get; set; } = null!;
    
    // =============================================
    // COMPUTED PROPERTIES
    // =============================================
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;
}
