// =============================================
// File: OlaVet.Domain/Entities/Core/RolePermission.cs
// Join entity for Role-Permission many-to-many
// =============================================

namespace OlaVet.Domain.Entities;

/// <summary>
/// Join table linking Roles to Permissions (many-to-many)
/// </summary>
public class RolePermission
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    
    // Navigation
    public virtual Role Role { get; set; } = null!;
    public virtual Permission Permission { get; set; } = null!;
}
