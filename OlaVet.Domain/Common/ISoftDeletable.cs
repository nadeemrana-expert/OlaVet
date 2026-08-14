// =============================================
// File: OlaVet.Domain/Common/ISoftDeletable.cs
// Interface for soft delete pattern
// =============================================

namespace OlaVet.Domain.Common;

/// <summary>
/// Entities implementing this can be soft-deleted
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedDate { get; set; }
    string? DeletedBy { get; set; }
}
