// =============================================
// File: OlaVet.Domain/Entities/Store.cs
// Pharmacy/Store entity
// =============================================

using OlaVet.Domain.Common;

namespace OlaVet.Domain.Entities;

/// <summary>
/// Store (Pharmacy) - sells medicines and pet supplies
/// </summary>
public class Store : BaseEntity
{
    public int StoreId { get; set; }
    
    public string StoreName { get; set; } = string.Empty;
    public string? StoreAddress { get; set; }
    public DateTime? Since { get; set; } // Establishment date
    public TimeSpan? OpeningTime { get; set; }
    public TimeSpan? ClosingTime { get; set; }
    public string ContactNumber { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual ICollection<Inventory> Inventories { get; set; } 
        = new List<Inventory>();
    public virtual ICollection<MedicineOrder> MedicineOrders { get; set; } 
        = new List<MedicineOrder>();
    public virtual ICollection<StoreReview> StoreReviews { get; set; } 
        = new List<StoreReview>();
    public virtual ICollection<StorePayment> StorePayments { get; set; } 
        = new List<StorePayment>();
}
