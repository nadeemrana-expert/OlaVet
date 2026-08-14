// =============================================
// File: OlaVet.Domain/Common/BaseEntity.cs
// Base class for all entities with audit fields
// =============================================

namespace OlaVet.Domain.Common;

/// <summary>
/// Base entity with common audit properties
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// When the entity was created (UTC)
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the entity was last modified (UTC)
    /// </summary>
    public DateTime? ModifiedDate { get; set; }
    
    /// <summary>
    /// Soft delete flag - allows "deleting" without removing from database
    /// </summary>
    public bool IsActive { get; set; } = true;
}
