// =============================================
// File: OlaVet.Domain/Entities/LabTest.cs
// Lab test catalog (available tests)
// =============================================

using OlaVet.Domain.Common;

namespace OlaVet.Domain.Entities;

/// <summary>
/// Lab test catalog - defines available tests
/// Example: "Complete Blood Count - 1500 PKR - 24 hours"
/// </summary>
public class LabTest : BaseEntity
{
    public int LabTestId { get; set; }
    
    public string LabTestName { get; set; } = string.Empty;
    public string? LabTestType { get; set; }
    public string? LabTestDescription { get; set; }
    public decimal TestFee { get; set; }
    public int? TurnaroundTimeHours { get; set; } // How long until results
    
    // Navigation property
    public virtual ICollection<LabAppointmentTest> LabAppointmentTests { get; set; } 
        = new List<LabAppointmentTest>();
}
