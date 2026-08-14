// =============================================
// File: OlaVet.Domain/Entities/Lookups/RecordType.cs
// Medical record types (Prescription, Report, etc.)
// =============================================

namespace OlaVet.Domain.Entities.Lookups;

/// <summary>
/// Medical record types: Prescription, Report, Invoice, Vaccine
/// </summary>
public class RecordType
{
    public int RecordTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
}
