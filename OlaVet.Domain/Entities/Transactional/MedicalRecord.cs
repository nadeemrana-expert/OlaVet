// =============================================
// File: OlaVet.Domain/Entities/MedicalRecord.cs
// Medical record (medical history)
// =============================================

using OlaVet.Domain.Entities.Lookups;

namespace OlaVet.Domain.Entities;

/// <summary>
/// Medical record - documents pet's medical history
/// </summary>
public class MedicalRecord
{
    public int RecordId { get; set; }
    
    // Foreign Keys
    public int PetId { get; set; }
    public int PetOwnerId { get; set; }
    public int RecordTypeId { get; set; }
    public int? VetId { get; set; } // Optional: which vet created this
    
    // Record Details
    public DateTime RecordDate { get; set; } = DateTime.UtcNow;
    public string? Diagnosis { get; set; }
    public string? TreatmentDescription { get; set; }
    public string? AttachmentPath { get; set; } // Path to file
    
    // Navigation properties
    public virtual Pet Pet { get; set; } = null!;
    public virtual PetOwner PetOwner { get; set; } = null!;
    public virtual RecordType RecordType { get; set; } = null!;
    public virtual Vet? Vet { get; set; } // Nullable: not all records have a vet
}
