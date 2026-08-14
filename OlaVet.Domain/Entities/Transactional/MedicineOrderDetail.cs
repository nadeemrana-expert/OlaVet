// =============================================
// File: OlaVet.Domain/Entities/MedicineOrderDetail.cs
// Line items in a medicine order
// =============================================

namespace OlaVet.Domain.Entities;

/// <summary>
/// Medicine order detail - a line item in an order
/// Example: 2x Amoxicillin 500mg @ 250 PKR = 500 PKR
/// </summary>
public class MedicineOrderDetail
{
    public int OrderDetailId { get; set; }
    
    // Foreign Keys
    public int MedicineOrderId { get; set; }
    public int MedicineId { get; set; }
    
    // Order Line Details
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    
    /// <summary>
    /// Computed property: Quantity * UnitPrice
    /// Configured as computed column in database
    /// Has private setter for EF Core compatibility
    /// </summary>
    public decimal Subtotal 
    { 
        get => Quantity * UnitPrice;
        private set { /* EF Core requires a setter for computed columns */ }
    }
    
    // Navigation properties
    public virtual MedicineOrder MedicineOrder { get; set; } = null!;
    public virtual Medicine Medicine { get; set; } = null!;
}
