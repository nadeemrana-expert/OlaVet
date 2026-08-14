// =============================================
// File: OlaVet.Domain/Entities/Core/UserRole.cs
// Join entity for User-Role many-to-many
// =============================================

namespace OlaVet.Domain.Entities;

/// <summary>
/// Join table linking Users to Roles (many-to-many)
/// </summary>
public class UserRole
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    
    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
}
