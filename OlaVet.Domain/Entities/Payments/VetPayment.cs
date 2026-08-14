// =============================================
// File: OlaVet.Domain/Entities/VetPayment.cs
// Payment for vet services
// =============================================

namespace OlaVet.Domain.Entities;

/// <summary>
/// Payment for a vet appointment
/// </summary>
public class VetPayment
{
    public int VetPaymentId { get; set; }
    
    // Foreign Keys (linking payment to appointment, owner, and vet)
    public int VetAppointmentId { get; set; }
    public int PetOwnerId { get; set; }
    public int VetId { get; set; }
    
    // Payment Details
    public decimal Amount { get; set; }
    public DateTime PaymentDateTime { get; set; } = DateTime.UtcNow;
    public string? PaymentMethod { get; set; } // Wallet, Card, Cash
    public string? TransactionId { get; set; } // Unique transaction identifier
    
    // =============================================
    // NAVIGATION PROPERTIES
    // =============================================
    
    /// <summary>
    /// The appointment this payment is for (One-to-One)
    /// </summary>
    public virtual VetAppointment VetAppointment { get; set; } = null!;
    
    public virtual PetOwner PetOwner { get; set; } = null!;
    public virtual Vet Vet { get; set; } = null!;
}
