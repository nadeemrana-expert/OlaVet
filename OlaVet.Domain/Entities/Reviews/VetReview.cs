// =============================================
// File: OlaVet.Domain/Entities/VetReview.cs
// Review for vet services
// =============================================

namespace OlaVet.Domain.Entities;

/// <summary>
/// Review for a vet after completed appointment
/// </summary>
public class VetReview
{
    public int VetReviewId { get; set; }
    
    // Foreign Keys
    public int VetAppointmentId { get; set; }
    public int PetOwnerId { get; set; }
    public int VetId { get; set; }
    
    // Review Details
    public int Rating { get; set; } // 1-5 stars
    public string? Comments { get; set; }
    public DateTime ReviewDateTime { get; set; } = DateTime.UtcNow;
    
    // =============================================
    // NAVIGATION PROPERTIES
    // =============================================
    
    /// <summary>
    /// The appointment being reviewed (One-to-One)
    /// </summary>
    public virtual VetAppointment VetAppointment { get; set; } = null!;
    
    public virtual PetOwner PetOwner { get; set; } = null!;
    public virtual Vet Vet { get; set; } = null!;
}
