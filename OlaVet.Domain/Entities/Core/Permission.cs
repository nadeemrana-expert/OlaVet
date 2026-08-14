// =============================================
// File: OlaVet.Domain/Entities/Core/Permission.cs
// Permission entity for granular access control
// =============================================

using OlaVet.Domain.Common;

namespace OlaVet.Domain.Entities;

/// <summary>
/// Permission entity for fine-grained authorization
/// Examples: "pets.read", "appointments.create", "vets.manage"
/// </summary>
public class Permission : BaseEntity
{
    public int PermissionId { get; set; }
    
    /// <summary>
    /// Permission name in dot notation (e.g., "pets.read", "vets.manage")
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Human-readable description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Category for grouping (e.g., "Pets", "Appointments", "Admin")
    /// </summary>
    public string? Category { get; set; }
    
    // Navigation
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
