// =============================================
// File: OlaVet.Domain/Interfaces/ILabAppointmentRepository.cs
// Specific repository for LabAppointment entity
// =============================================

using OlaVet.Domain.Entities;

namespace OlaVet.Domain.Interfaces;

/// <summary>
/// Lab appointment-specific repository methods
/// </summary>
public interface ILabAppointmentRepository : IRepository<LabAppointment>
{
    /// <summary>
    /// Get lab appointment with all details
    /// </summary>
    Task<LabAppointment?> GetWithDetailsAsync(int appointmentId);
    
    /// <summary>
    /// Get lab appointments for a specific lab on a date
    /// </summary>
    Task<IEnumerable<LabAppointment>> GetByLabAndDateAsync(int labId, DateTime date);
    
    /// <summary>
    /// Get lab appointments for a pet owner
    /// </summary>
    Task<IEnumerable<LabAppointment>> GetByOwnerIdAsync(int ownerId);
    
    /// <summary>
    /// Get lab appointments for a pet
    /// </summary>
    Task<IEnumerable<LabAppointment>> GetByPetIdAsync(int petId);
    
    /// <summary>
    /// Get upcoming lab appointments
    /// </summary>
    Task<IEnumerable<LabAppointment>> GetUpcomingAsync(int days = 7);
    
    /// <summary>
    /// Get completed lab appointments with results
    /// </summary>
    Task<IEnumerable<LabAppointment>> GetCompletedWithResultsAsync();
    
    /// <summary>
    /// Get appointments awaiting results
    /// </summary>
    Task<IEnumerable<LabAppointment>> GetAwaitingResultsAsync();
}
