// =============================================
// File: OlaVet.Domain/Entities/Pet.cs
// Pet entity (the patient)
// =============================================

using OlaVet.Domain.Common;

namespace OlaVet.Domain.Entities;

/// <summary>
/// Pet - the patient in our veterinary system
/// </summary>
public class Pet : BaseEntity
{
    public int PetId { get; set; }
    
    // Foreign Key to Owner
    public int PetOwnerId { get; set; }
    
    // Basic Information
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty; // Dog, Cat, Bird, etc.
    public string? Breed { get; set; }
    public int? Age { get; set; }
    public decimal? PetWeight { get; set; } // in kilograms
    public string? Color { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    
    // =============================================
    // NAVIGATION PROPERTIES
    // =============================================
    
    /// <summary>
    /// The owner of this pet (Many-to-One relationship)
    /// Many Pets → One Owner
    /// </summary>
    public virtual PetOwner PetOwner { get; set; } = null!;
    
    /// <summary>
    /// All vet appointments for this pet
    /// </summary>
    public virtual ICollection<VetAppointment> VetAppointments { get; set; } 
        = new List<VetAppointment>();
    
    /// <summary>
    /// All lab appointments for this pet
    /// </summary>
    public virtual ICollection<LabAppointment> LabAppointments { get; set; } 
        = new List<LabAppointment>();
    
    /// <summary>
    /// Medical history/records for this pet
    /// </summary>
    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } 
        = new List<MedicalRecord>();
}
