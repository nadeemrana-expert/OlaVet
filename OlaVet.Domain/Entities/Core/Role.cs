// =============================================
// File: OlaVet.Domain/Entities/Core/Role.cs
// Role entity for RBAC
// =============================================

using OlaVet.Domain.Common;

namespace OlaVet.Domain.Entities;

/// <summary>
/// Role entity for role-based access control
/// </summary>
public class Role : BaseEntity
{
    public int RoleId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // =============================================
    // NAVIGATION PROPERTIES
    // =============================================
    
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
