// =============================================
// File: OlaVet.Domain/Entities/Lookups/VetAppointmentType.cs
// Appointment types (Clinic, Video)
// =============================================

namespace OlaVet.Domain.Entities.Lookups;

/// <summary>
/// Vet appointment types: Clinic (in-person) or Video (online)
/// </summary>
public class VetAppointmentType
{
    public int VetAppointmentTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public virtual ICollection<VetAppointment> VetAppointments { get; set; } = new List<VetAppointment>();
}
