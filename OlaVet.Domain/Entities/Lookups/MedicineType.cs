// =============================================
// File: OlaVet.Domain/Entities/Lookups/MedicineType.cs
// Lookup table for medicine types (Tablet, Capsule, etc.)
// =============================================

namespace OlaVet.Domain.Entities.Lookups;

/// <summary>
/// Medicine types: Tablet, Capsule, Injection, etc.
/// </summary>
public class MedicineType
{
    public int MedicineTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    // Navigation property: One MedicineType → Many Medicines
    public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
}
