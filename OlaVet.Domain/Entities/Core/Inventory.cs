// =============================================
// File: OlaVet.Domain/Entities/Inventory.cs
// Store inventory (junction between Store and Medicine)
// =============================================

namespace OlaVet.Domain.Entities;

/// <summary>
/// Inventory - tracks medicine stock at each store
/// Represents a Many-to-Many relationship between Store and Medicine
/// </summary>
public class Inventory
{
    public int InventoryId { get; set; }
    
    // Foreign Keys
    public int StoreId { get; set; }
    public int MedicineId { get; set; }
    
    public int Quantity { get; set; } = 0;
    public DateTime? LastRestocked { get; set; }
    
    // =============================================
    // NAVIGATION PROPERTIES
    // =============================================
    
    /// <summary>
    /// The store that stocks this medicine
    /// </summary>
    public virtual Store Store { get; set; } = null!;
    
    /// <summary>
    /// The medicine being stocked
    /// </summary>
    public virtual Medicine Medicine { get; set; } = null!;
}
