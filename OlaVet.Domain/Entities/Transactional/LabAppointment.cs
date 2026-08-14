// =============================================
// File: OlaVet.Domain/Entities/LabAppointment.cs
// Lab appointment entity
// =============================================

using OlaVet.Domain.Entities.Lookups;

namespace OlaVet.Domain.Entities;

/// <summary>
/// Lab appointment - booking for diagnostic tests
/// </summary>
public class LabAppointment
{
    public int LabAppointmentId { get; set; }
    
    // Foreign Keys
    public int PetId { get; set; }
    public int PetOwnerId { get; set; }
    public int LabId { get; set; }
    public int StatusTypeId { get; set; }
    
    // Appointment Details
    public DateTime AppointmentDateTime { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedDate { get; set; }
    
    // =============================================
    // NAVIGATION PROPERTIES
    // =============================================
    
    public virtual Pet Pet { get; set; } = null!;
    public virtual PetOwner PetOwner { get; set; } = null!;
    public virtual Lab Lab { get; set; } = null!;
    public virtual StatusType StatusType { get; set; } = null!;
    
    /// <summary>
    /// Tests ordered in this appointment
    /// One LabAppointment → Many LabAppointmentTests
    /// </summary>
    public virtual ICollection<LabAppointmentTest> LabAppointmentTests { get; set; } 
        = new List<LabAppointmentTest>();
    
    public virtual LabReview? LabReview { get; set; }
    public virtual LabPayment? LabPayment { get; set; }
}
