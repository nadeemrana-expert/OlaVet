// =============================================
// File: OlaVet.Domain/Interfaces/IPetOwnerRepository.cs
// Specific repository for PetOwner entity
// =============================================

using OlaVet.Domain.Entities;

namespace OlaVet.Domain.Interfaces;

/// <summary>
/// PetOwner-specific repository methods
/// Extends generic repository with custom queries
/// </summary>
public interface IPetOwnerRepository : IRepository<PetOwner>
{
    /// <summary>
    /// Get pet owner with all their pets
    /// </summary>
    Task<PetOwner?> GetWithPetsAsync(int ownerId);
    
    /// <summary>
    /// Get pet owner with pets and appointments
    /// </summary>
    Task<PetOwner?> GetWithPetsAndAppointmentsAsync(int ownerId);
    
    /// <summary>
    /// Search pet owners by name or email
    /// </summary>
    Task<IEnumerable<PetOwner>> SearchAsync(string searchTerm);
    
    /// <summary>
    /// Get owners registered within date range
    /// </summary>
    Task<IEnumerable<PetOwner>> GetRegisteredBetweenAsync(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Get top owners by wallet balance
    /// </summary>
    Task<IEnumerable<PetOwner>> GetTopByWalletAsync(int count = 10);
    
    /// <summary>
    /// Get active owners with low wallet balance (for notifications)
    /// </summary>
    Task<IEnumerable<PetOwner>> GetLowBalanceOwnersAsync(decimal threshold = 1000);
}