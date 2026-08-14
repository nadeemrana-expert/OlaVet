// =============================================
// File: OlaVet.Domain/Entities/StorePayment.cs
// Payment for medicine orders
// =============================================

namespace OlaVet.Domain.Entities;

/// <summary>
/// Payment for a medicine order
/// </summary>
public class StorePayment
{
    public int StorePaymentId { get; set; }
    
    // Foreign Keys
    public int MedicineOrderId { get; set; }
    public int PetOwnerId { get; set; }
    public int StoreId { get; set; }
    
    // Payment Details
    public decimal Amount { get; set; }
    public DateTime PaymentDateTime { get; set; } = DateTime.UtcNow;
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    
    // Navigation properties
    public virtual MedicineOrder MedicineOrder { get; set; } = null!;
    public virtual PetOwner PetOwner { get; set; } = null!;
    public virtual Store Store { get; set; } = null!;
}
