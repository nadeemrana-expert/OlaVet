// =============================================
// File: OlaVet.Domain/Entities/LabPayment.cs
// Payment for lab services
// =============================================

namespace OlaVet.Domain.Entities;

/// <summary>
/// Payment for a lab appointment
/// </summary>
public class LabPayment
{
    public int LabPaymentId { get; set; }
    
    // Foreign Keys
    public int LabAppointmentId { get; set; }
    public int PetOwnerId { get; set; }
    public int LabId { get; set; }
    
    // Payment Details
    public decimal Amount { get; set; }
    public DateTime PaymentDateTime { get; set; } = DateTime.UtcNow;
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    
    // Navigation properties
    public virtual LabAppointment LabAppointment { get; set; } = null!;
    public virtual PetOwner PetOwner { get; set; } = null!;
    public virtual Lab Lab { get; set; } = null!;
}
