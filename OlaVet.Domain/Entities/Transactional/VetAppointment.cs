// =============================================
// File: OlaVet.Domain/Entities/VetAppointment.cs
// Vet appointment entity
// =============================================

using OlaVet.Domain.Entities.Lookups;

namespace OlaVet.Domain.Entities;

/// <summary>
/// Vet appointment - booking for a vet consultation
/// </summary>
public class VetAppointment
{
    public int VetAppointmentId { get; set; }
    
    // Foreign Keys
    public int PetId { get; set; }
    public int PetOwnerId { get; set; }
    public int VetId { get; set; }
    public int VetAppointmentTypeId { get; set; } // Clinic or Video
    public int StatusTypeId { get; set; } // Scheduled, Completed, etc.
    
    // Appointment Details
    public DateTime AppointmentDateTime { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedDate { get; set; }
    
    // =============================================
    // NAVIGATION PROPERTIES
    // =============================================
    
    /// <summary>
    /// The pet this appointment is for
    /// </summary>
    public virtual Pet Pet { get; set; } = null!;
    
    /// <summary>
    /// The owner who booked this appointment
    /// </summary>
    public virtual PetOwner PetOwner { get; set; } = null!;
    
    /// <summary>
    /// The vet providing the service
    /// </summary>
    public virtual Vet Vet { get; set; } = null!;
    
    /// <summary>
    /// Type of appointment (Clinic/Video)
    /// </summary>
    public virtual VetAppointmentType VetAppointmentType { get; set; } = null!;
    
    /// <summary>
    /// Current status of the appointment
    /// </summary>
    public virtual StatusType StatusType { get; set; } = null!;
    
    /// <summary>
    /// Review for this appointment (One-to-One)
    /// Can be null if not reviewed yet
    /// </summary>
    public virtual VetReview? VetReview { get; set; }
    
    /// <summary>
    /// Payment for this appointment (One-to-One)
    /// Can be null if not paid yet
    /// </summary>
    public virtual VetPayment? VetPayment { get; set; }
}
