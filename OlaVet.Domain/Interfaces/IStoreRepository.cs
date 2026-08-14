// =============================================
// File: OlaVet.Domain/Interfaces/IStoreRepository.cs
// Specific repository for Store entity
// =============================================

using OlaVet.Domain.Entities;

namespace OlaVet.Domain.Interfaces;

/// <summary>
/// Store-specific repository methods
/// </summary>
public interface IStoreRepository : IRepository<Store>
{
    /// <summary>
    /// Get store with inventory
    /// </summary>
    Task<Store?> GetWithInventoryAsync(int storeId);
    
    /// <summary>
    /// Search stores by name or address
    /// </summary>
    Task<IEnumerable<Store>> SearchAsync(string searchTerm);
    
    /// <summary>
    /// Get stores with a specific medicine in stock
    /// </summary>
    Task<IEnumerable<Store>> GetStoresWithMedicineAsync(int medicineId);
    
    /// <summary>
    /// Get top rated stores
    /// </summary>
    Task<IEnumerable<Store>> GetTopRatedAsync(int count = 10);
    
    /// <summary>
    /// Get stores with average rating
    /// </summary>
    Task<IEnumerable<StoreWithRating>> GetStoresWithRatingsAsync();
    
    /// <summary>
    /// Get currently open stores
    /// </summary>
    Task<IEnumerable<Store>> GetOpenStoresAsync();
}

/// <summary>
/// DTO for Store with rating information
/// </summary>
public class StoreWithRating
{
    public int StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string? StoreAddress { get; set; }
    public string ContactNumber { get; set; } = string.Empty;
    public TimeSpan? OpeningTime { get; set; }
    public TimeSpan? ClosingTime { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int OrderCount { get; set; }
}
