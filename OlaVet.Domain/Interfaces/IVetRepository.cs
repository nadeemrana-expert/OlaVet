
// =============================================
// File: OlaVet.Domain/Interfaces/IVetRepository.cs
// Specific repository for Vet entity
// =============================================

using OlaVet.Domain.Common;
using OlaVet.Domain.Entities;

namespace OlaVet.Domain.Interfaces;

public interface IVetRepository : IRepository<Vet>
{
    /// <summary>
    /// Get vet with qualifications and services
    /// </summary>
    Task<Vet?> GetWithDetailsAsync(int vetId);
    
    /// <summary>
    /// Search vets by name or specialization
    /// </summary>
    Task<IEnumerable<Vet>> SearchAsync(string searchTerm);
    
    /// <summary>
    /// Get vets by specialization
    /// </summary>
    Task<IEnumerable<Vet>> GetBySpecializationAsync(string specialization);
    
    /// <summary>
    /// Get available vets for a specific date/time
    /// </summary>
    Task<IEnumerable<Vet>> GetAvailableVetsAsync(DateTime dateTime);
    
    /// <summary>
    /// Get top rated vets
    /// </summary>
    Task<IEnumerable<Vet>> GetTopRatedAsync(int count = 10);
    
    /// <summary>
    /// Get vets with average rating and review count
    /// </summary>
    Task<IEnumerable<VetWithRating>> GetVetsWithRatingsAsync();
    
    /// <summary>
    /// Get vets with ratings - paginated
    /// </summary>
    Task<PagedResult<VetWithRating>> GetVetsWithRatingsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}