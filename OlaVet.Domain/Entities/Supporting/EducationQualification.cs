// =============================================
// File: OlaVet.Domain/Entities/EducationQualification.cs
// Vet's education qualifications
// =============================================

namespace OlaVet.Domain.Entities;

/// <summary>
/// Educational qualifications for a veterinarian
/// Example: DVM from University of Veterinary Sciences
/// </summary>
public class EducationQualification
{
    public int EducationId { get; set; }
    
    // Foreign Key to Vet
    public int VetId { get; set; }
    
    public string QualificationName { get; set; } = string.Empty;
    public string? Institute { get; set; }
    public int? YearOfDegree { get; set; }
    
    // =============================================
    // NAVIGATION PROPERTY (Many-to-One)
    // =============================================
    /// <summary>
    /// The vet this qualification belongs to
    /// EF Core convention: Property name matches FK
    /// </summary>
    public virtual Vet Vet { get; set; } = null!; // null! = not null but may be null during construction
}
