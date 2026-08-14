// =============================================
// File: OlaVet.Domain/Entities/Lab.cs
// Laboratory entity
// =============================================

using OlaVet.Domain.Common;

namespace OlaVet.Domain.Entities;

/// <summary>
/// Laboratory - provides diagnostic testing services
/// </summary>
public class Lab : BaseEntity
{
    public int LabId { get; set; }
    
    public string LabName { get; set; } = string.Empty;
    public string? LabAddress { get; set; }
    public int? WaitTime { get; set; } // in hours
    public int? Experience { get; set; } // years in business
    public string ContactNumber { get; set; } = string.Empty;
    public decimal Discount { get; set; } = 0; // percentage discount
    public string? Specialization { get; set; }
    
    // Navigation properties
    public virtual ICollection<LabAppointment> LabAppointments { get; set; } 
        = new List<LabAppointment>();
    public virtual ICollection<LabReview> LabReviews { get; set; } 
        = new List<LabReview>();
    public virtual ICollection<LabPayment> LabPayments { get; set; } 
        = new List<LabPayment>();
}
