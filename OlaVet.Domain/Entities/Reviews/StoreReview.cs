// =============================================
// File: OlaVet.Domain/Entities/StoreReview.cs
// Review for store/pharmacy
// =============================================

namespace OlaVet.Domain.Entities;

/// <summary>
/// Review for a store after medicine order delivery
/// </summary>
public class StoreReview
{
    public int StoreReviewId { get; set; }
    
    // Foreign Keys
    public int MedicineOrderId { get; set; }
    public int PetOwnerId { get; set; }
    public int StoreId { get; set; }
    
    // Review Details
    public int Rating { get; set; } // 1-5 stars
    public string? Comments { get; set; }
    public DateTime ReviewDateTime { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual MedicineOrder MedicineOrder { get; set; } = null!;
    public virtual PetOwner PetOwner { get; set; } = null!;
    public virtual Store Store { get; set; } = null!;
}
