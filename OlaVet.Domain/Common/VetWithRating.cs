// =============================================
// File: OlaVet.Domain/Common/VetWithRating.cs
// DTO for Vet with rating information
// =============================================

namespace OlaVet.Domain.Interfaces;

/// <summary>
/// Data Transfer Object for Vet with calculated rating
/// Used when returning vet information with average rating
/// </summary>
public class VetWithRating
{
    public int VetId { get; set; }
    public string VetName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string? ClinicLocation { get; set; }
    public decimal Fee { get; set; }
    public string ContactNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int? YearsOfExperience { get; set; }
    public string? LicenseNumber { get; set; }
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Average rating from reviews (1-5 scale)
    /// </summary>
    public double AverageRating { get; set; }
    
    /// <summary>
    /// Total number of reviews
    /// </summary>
    public int ReviewCount { get; set; }
    
    /// <summary>
    /// Total number of completed appointments
    /// </summary>
    public int AppointmentCount { get; set; }
}
