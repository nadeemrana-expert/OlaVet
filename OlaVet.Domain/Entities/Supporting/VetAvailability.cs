// =============================================
// File: OlaVet.Domain/Entities/VetAvailability.cs
// Vet's availability schedule
// =============================================

namespace OlaVet.Domain.Entities;

/// <summary>
/// Vet's availability schedule (time slots)
/// Example: "Monday 9:00 AM - 5:00 PM, 30-minute slots"
/// </summary>
public class VetAvailability
{
    public int AvailabilityId { get; set; }
    
    // Foreign Key
    public int VetId { get; set; }
    
    public string DayOfWeek { get; set; } = string.Empty; // Monday, Tuesday, etc.
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int SlotDurationMinutes { get; set; } = 30;
    public bool IsAvailable { get; set; } = true;
    
    // Navigation property
    public virtual Vet Vet { get; set; } = null!;
}
