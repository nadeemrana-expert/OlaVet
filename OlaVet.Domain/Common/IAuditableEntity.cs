// =============================================
// File: OlaVet.Domain/Common/IAuditableEntity.cs
// Interface for entities that track who created/modified them
// =============================================

namespace OlaVet.Domain.Common;

/// <summary>
/// Entities implementing this will track user who created/modified
/// </summary>
public interface IAuditableEntity
{
    string? CreatedBy { get; set; }
    string? ModifiedBy { get; set; }
}
