// =============================================
// File: OlaVet.Domain/Entities/Medicine.cs
// Medicine catalog
// =============================================

using OlaVet.Domain.Common;
using OlaVet.Domain.Entities.Lookups;

namespace OlaVet.Domain.Entities;

/// <summary>
/// Medicine catalog - defines available medicines
/// </summary>
public class Medicine : BaseEntity
{
    public int MedicineId { get; set; }
    
    public string MedicineName { get; set; } = string.Empty;
    public int? MG { get; set; } // Milligrams
    public decimal Price { get; set; }
    
    // Foreign Key to MedicineType
    public int? MedicineTypeId { get; set; }
    
    public string? Manufacturer { get; set; }
    public string? Description { get; set; }
    public bool RequiresPrescription { get; set; } = true;
    
    // =============================================
    // NAVIGATION PROPERTIES
    // =============================================
    
    /// <summary>
    /// The type of medicine (Tablet, Capsule, etc.)
    /// Many Medicines → One MedicineType
    /// </summary>
    public virtual MedicineType? MedicineType { get; set; }
    
    /// <summary>
    /// Inventory records (which stores stock this medicine)
    /// </summary>
    public virtual ICollection<Inventory> Inventories { get; set; } 
        = new List<Inventory>();
    
    /// <summary>
    /// Order details where this medicine appears
    /// </summary>
    public virtual ICollection<MedicineOrderDetail> MedicineOrderDetails { get; set; } 
        = new List<MedicineOrderDetail>();
}
