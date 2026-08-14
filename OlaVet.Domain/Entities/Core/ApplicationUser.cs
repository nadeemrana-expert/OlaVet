// =============================================
// File: OlaVet.Domain/Entities/Core/ApplicationUser.cs
// Application user entity for authentication
// =============================================

using OlaVet.Domain.Common;

namespace OlaVet.Domain.Entities;

/// <summary>
/// Application user - handles authentication and authorization
/// Linked to PetOwner or Vet entities via foreign keys
/// </summary>
public class ApplicationUser : BaseEntity
{
    public int UserId { get; set; }
    
    // Authentication Fields
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    
    // Profile
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    
    // Account Status
    public bool EmailConfirmed { get; set; } = false;
    public bool IsLockedOut { get; set; } = false;
    public DateTime? LockoutEnd { get; set; }
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LastLoginDate { get; set; }
    
    // MFA (future)
    public bool TwoFactorEnabled { get; set; } = false;
    public string? TwoFactorSecret { get; set; }
    
    // GDPR
    public bool GdprConsentGiven { get; set; } = false;
    public DateTime? GdprConsentDate { get; set; }
    
    // Linked Entity (a user can be a PetOwner or a Vet)
    public int? PetOwnerId { get; set; }
    public int? VetId { get; set; }
    
    // =============================================
    // NAVIGATION PROPERTIES
    // =============================================
    
    public virtual PetOwner? PetOwner { get; set; }
    public virtual Vet? Vet { get; set; }
    
    /// <summary>
    /// Roles assigned to this user
    /// </summary>
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    
    /// <summary>
    /// Refresh tokens issued to this user
    /// </summary>
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
