// =============================================
// File: OlaVet.Domain/Entities/LabAppointmentTest.cs
// Tests within a lab appointment
// =============================================

namespace OlaVet.Domain.Entities;

/// <summary>
/// Lab appointment test - a specific test within an appointment
/// Example: CBC test in appointment #123
/// </summary>
public class LabAppointmentTest
{
    public int LabAppointmentTestId { get; set; }
    
    // Foreign Keys
    public int LabAppointmentId { get; set; }
    public int LabTestId { get; set; }
    
    // Test Results
    public string? TestResult { get; set; }
    public DateTime? ResultDate { get; set; }
    public string? ResultFile { get; set; } // Path to PDF/image
    
    // Navigation properties
    public virtual LabAppointment LabAppointment { get; set; } = null!;
    public virtual LabTest LabTest { get; set; } = null!;
}
