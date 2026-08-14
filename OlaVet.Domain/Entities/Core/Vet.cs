// =============================================
// File: OlaVet.Domain/Entities/Vet.cs
// Veterinarian entity
// =============================================

using OlaVet.Domain.Common;

namespace OlaVet.Domain.Entities;

/// <summary>
/// Veterinarian - provides medical services for pets
/// </summary>
public class Vet : BaseEntity
{
    public int VetId { get; set; }
    
    // Professional Information
    public string VetName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string? ClinicLocation { get; set; }
    public decimal Fee { get; set; } // Consultation fee
    
    // Contact Information
    public string ContactNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    
    // Credentials
    public int? YearsOfExperience { get; set; }
    public string? LicenseNumber { get; set; }
    
    // =============================================
    // NAVIGATION PROPERTIES
    // =============================================
    
    /// <summary>
    /// Educational qualifications of the vet
    /// One Vet → Many Qualifications (One-to-Many)
    /// </summary>
    public virtual ICollection<EducationQualification> EducationQualifications { get; set; } 
        = new List<EducationQualification>();
    
    /// <summary>
    /// Services offered by this vet
    /// One Vet → Many Services
    /// </summary>
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    
    /// <summary>
    /// Availability schedule (time slots)
    /// One Vet → Many Availability slots
    /// </summary>
    public virtual ICollection<VetAvailability> Availabilities { get; set; } 
        = new List<VetAvailability>();
    
    /// <summary>
    /// All appointments for this vet
    /// </summary>
    public virtual ICollection<VetAppointment> VetAppointments { get; set; } 
        = new List<VetAppointment>();
    
    /// <summary>
    /// Medical records created by this vet
    /// </summary>
    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } 
        = new List<MedicalRecord>();
    
    /// <summary>
    /// Reviews received
    /// </summary>
    public virtual ICollection<VetReview> VetReviews { get; set; } = new List<VetReview>();
    
    /// <summary>
    /// Payment records
    /// </summary>
    public virtual ICollection<VetPayment> VetPayments { get; set; } = new List<VetPayment>();
}
