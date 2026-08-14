// =============================================
// File: OlaVet.Domain/Entities/Lookups/StatusType.cs
// Status types for appointments and orders
// =============================================

namespace OlaVet.Domain.Entities.Lookups;

/// <summary>
/// Status types: Scheduled, Completed, Cancelled, etc.
/// Can apply to appointments, orders, etc.
/// </summary>
public class StatusType
{
    public int StatusTypeId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    
    /// <summary>
    /// What this status applies to: "Appointment", "MedicineOrder", etc.
    /// </summary>
    public string? AppliesTo { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    // Navigation properties - one status type used by many entities
    public virtual ICollection<VetAppointment> VetAppointments { get; set; } = new List<VetAppointment>();
    public virtual ICollection<LabAppointment> LabAppointments { get; set; } = new List<LabAppointment>();
    public virtual ICollection<MedicineOrder> MedicineOrders { get; set; } = new List<MedicineOrder>();
}
