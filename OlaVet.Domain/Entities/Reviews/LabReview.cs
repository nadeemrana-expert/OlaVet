// =============================================
// File: OlaVet.Domain/Entities/LabReview.cs
// Review for lab services
// =============================================

namespace OlaVet.Domain.Entities;

/// <summary>
/// Review for a lab after completed appointment
/// </summary>
public class LabReview
{
    public int LabReviewId { get; set; }
    
    // Foreign Keys
    public int LabAppointmentId { get; set; }
    public int PetOwnerId { get; set; }
    public int LabId { get; set; }
    
    // Review Details
    public int Rating { get; set; } // 1-5 stars
    public string? Comments { get; set; }
    public DateTime ReviewDateTime { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual LabAppointment LabAppointment { get; set; } = null!;
    public virtual PetOwner PetOwner { get; set; } = null!;
    public virtual Lab Lab { get; set; } = null!;
}
