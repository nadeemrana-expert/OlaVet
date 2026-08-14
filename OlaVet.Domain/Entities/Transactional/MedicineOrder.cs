// =============================================
// File: OlaVet.Domain/Entities/MedicineOrder.cs
// Medicine order (shopping cart)
// =============================================

using OlaVet.Domain.Entities.Lookups;

namespace OlaVet.Domain.Entities;

/// <summary>
/// Medicine order - an order placed by a pet owner
/// </summary>
public class MedicineOrder
{
    public int MedicineOrderId { get; set; }
    
    // Foreign Keys
    public int PetOwnerId { get; set; }
    public int StoreId { get; set; }
    public int StatusTypeId { get; set; }
    
    // Order Details
    public DateTime OrderDateTime { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public string? DeliveryAddress { get; set; }
    public DateTime? DeliveredDate { get; set; }
    
    // =============================================
    // NAVIGATION PROPERTIES
    // =============================================
    
    public virtual PetOwner PetOwner { get; set; } = null!;
    public virtual Store Store { get; set; } = null!;
    public virtual StatusType StatusType { get; set; } = null!;
    
    /// <summary>
    /// Line items in this order (the medicines ordered)
    /// One Order → Many OrderDetails
    /// </summary>
    public virtual ICollection<MedicineOrderDetail> MedicineOrderDetails { get; set; } 
        = new List<MedicineOrderDetail>();
    
    public virtual StoreReview? StoreReview { get; set; }
    public virtual StorePayment? StorePayment { get; set; }
}
