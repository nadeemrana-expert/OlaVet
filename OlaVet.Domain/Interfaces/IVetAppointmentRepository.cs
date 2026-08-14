// =============================================
// File: OlaVet.Domain/Interfaces/IVetAppointmentRepository.cs
// Specific repository for VetAppointment entity
// =============================================

using OlaVet.Domain.Entities;

namespace OlaVet.Domain.Interfaces;

public interface IVetAppointmentRepository : IRepository<VetAppointment>
{
    /// <summary>
    /// Get appointment with all related data (pet, owner, vet)
    /// </summary>
    Task<VetAppointment?> GetWithDetailsAsync(int appointmentId);
    
    /// <summary>
    /// Get appointments for a specific vet on a date
    /// </summary>
    Task<IEnumerable<VetAppointment>> GetByVetAndDateAsync(int vetId, DateTime date);
    
    /// <summary>
    /// Get appointments for a pet owner
    /// </summary>
    Task<IEnumerable<VetAppointment>> GetByOwnerIdAsync(int ownerId);
    
    /// <summary>
    /// Get upcoming appointments (next 7 days)
    /// </summary>
    Task<IEnumerable<VetAppointment>> GetUpcomingAsync(int days = 7);
    
    /// <summary>
    /// Get appointment history for a pet
    /// </summary>
    Task<IEnumerable<VetAppointment>> GetPetHistoryAsync(int petId);
    
    /// <summary>
    /// Check if time slot is available for vet
    /// </summary>
    Task<bool> IsTimeSlotAvailableAsync(int vetId, DateTime dateTime);
    
    /// <summary>
    /// Get available time slots for vet on date
    /// </summary>
    Task<IEnumerable<DateTime>> GetAvailableTimeSlotsAsync(int vetId, DateTime date);
    
    /// <summary>
    /// Get appointments requiring follow-up
    /// </summary>
    Task<IEnumerable<VetAppointment>> GetRequiringFollowUpAsync();
}