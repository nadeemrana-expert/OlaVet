// =============================================
// File: OlaVet.Domain/Interfaces/ILabRepository.cs
// Specific repository for Lab entity
// =============================================

using OlaVet.Domain.Entities;

namespace OlaVet.Domain.Interfaces;

/// <summary>
/// Lab-specific repository methods
/// </summary>
public interface ILabRepository : IRepository<Lab>
{
    /// <summary>
    /// Get lab with all appointments
    /// </summary>
    Task<Lab?> GetWithAppointmentsAsync(int labId);
    
    /// <summary>
    /// Search labs by name or specialization
    /// </summary>
    Task<IEnumerable<Lab>> SearchAsync(string searchTerm);
    
    /// <summary>
    /// Get labs by specialization
    /// </summary>
    Task<IEnumerable<Lab>> GetBySpecializationAsync(string specialization);
    
    /// <summary>
    /// Get top rated labs
    /// </summary>
    Task<IEnumerable<Lab>> GetTopRatedAsync(int count = 10);
    
    /// <summary>
    /// Get labs with average rating
    /// </summary>
    Task<IEnumerable<LabWithRating>> GetLabsWithRatingsAsync();
}

/// <summary>
/// DTO for Lab with rating information
/// </summary>
public class LabWithRating
{
    public int LabId { get; set; }
    public string LabName { get; set; } = string.Empty;
    public string? LabAddress { get; set; }
    public string? Specialization { get; set; }
    public string ContactNumber { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int AppointmentCount { get; set; }
}
