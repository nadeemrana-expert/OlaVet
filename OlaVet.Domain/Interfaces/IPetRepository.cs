
// =============================================
// File: OlaVet.Domain/Interfaces/IPetRepository.cs
// Specific repository for Pet entity
// =============================================

using OlaVet.Domain.Entities;

namespace OlaVet.Domain.Interfaces;

public interface IPetRepository : IRepository<Pet>
{
    /// <summary>
    /// Get pet with owner information
    /// </summary>
    Task<Pet?> GetWithOwnerAsync(int petId);
    
    /// <summary>
    /// Get pet with medical history
    /// </summary>
    Task<Pet?> GetWithMedicalHistoryAsync(int petId);
    
    /// <summary>
    /// Get all pets for an owner
    /// </summary>
    Task<IEnumerable<Pet>> GetByOwnerIdAsync(int ownerId);
    
    /// <summary>
    /// Get pets by species
    /// </summary>
    Task<IEnumerable<Pet>> GetBySpeciesAsync(string species);
    
    /// <summary>
    /// Get pets due for checkup (no appointment in X days)
    /// </summary>
    Task<IEnumerable<Pet>> GetDueForCheckupAsync(int daysThreshold = 180);
}
