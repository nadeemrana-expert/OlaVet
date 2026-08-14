// =============================================
// File: OlaVet.Domain/Interfaces/IReviewRepository.cs
// Combined review repository interface
// =============================================

using OlaVet.Domain.Entities;

namespace OlaVet.Domain.Interfaces;

/// <summary>
/// Review repository handling all review types
/// </summary>
public interface IReviewRepository
{
    // =============================================
    // VET REVIEWS
    // =============================================
    
    /// <summary>
    /// Get all reviews for a vet
    /// </summary>
    Task<IEnumerable<VetReview>> GetVetReviewsAsync(int vetId);
    
    /// <summary>
    /// Get vet reviews by pet owner
    /// </summary>
    Task<IEnumerable<VetReview>> GetVetReviewsByOwnerAsync(int ownerId);
    
    /// <summary>
    /// Get vet review by appointment
    /// </summary>
    Task<VetReview?> GetVetReviewByAppointmentAsync(int appointmentId);
    
    // =============================================
    // LAB REVIEWS
    // =============================================
    
    /// <summary>
    /// Get all reviews for a lab
    /// </summary>
    Task<IEnumerable<LabReview>> GetLabReviewsAsync(int labId);
    
    /// <summary>
    /// Get lab reviews by pet owner
    /// </summary>
    Task<IEnumerable<LabReview>> GetLabReviewsByOwnerAsync(int ownerId);
    
    /// <summary>
    /// Get lab review by appointment
    /// </summary>
    Task<LabReview?> GetLabReviewByAppointmentAsync(int appointmentId);
    
    // =============================================
    // STORE REVIEWS
    // =============================================
    
    /// <summary>
    /// Get all reviews for a store
    /// </summary>
    Task<IEnumerable<StoreReview>> GetStoreReviewsAsync(int storeId);
    
    /// <summary>
    /// Get store reviews by pet owner
    /// </summary>
    Task<IEnumerable<StoreReview>> GetStoreReviewsByOwnerAsync(int ownerId);
    
    /// <summary>
    /// Get store review by order
    /// </summary>
    Task<StoreReview?> GetStoreReviewByOrderAsync(int orderId);
    
    // =============================================
    // AGGREGATE METHODS
    // =============================================
    
    /// <summary>
    /// Get average rating for any entity type
    /// </summary>
    Task<double> GetAverageRatingAsync(string entityType, int entityId);
    
    /// <summary>
    /// Get review count for any entity type
    /// </summary>
    Task<int> GetReviewCountAsync(string entityType, int entityId);
    
    /// <summary>
    /// Get rating distribution (1-5 stars breakdown)
    /// </summary>
    Task<RatingDistribution> GetRatingDistributionAsync(string entityType, int entityId);
    
    /// <summary>
    /// Get recent reviews across all types
    /// </summary>
    Task<IEnumerable<RecentReview>> GetRecentReviewsAsync(int count = 10);
}

/// <summary>
/// Rating breakdown by star count
/// </summary>
public class RatingDistribution
{
    public int OneStar { get; set; }
    public int TwoStar { get; set; }
    public int ThreeStar { get; set; }
    public int FourStar { get; set; }
    public int FiveStar { get; set; }
    public int Total => OneStar + TwoStar + ThreeStar + FourStar + FiveStar;
}

/// <summary>
/// Recent review DTO for dashboard
/// </summary>
public class RecentReview
{
    public string ReviewType { get; set; } = string.Empty; // Vet, Lab, Store
    public int ReviewId { get; set; }
    public int EntityId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comments { get; set; }
    public DateTime ReviewDateTime { get; set; }
    public string OwnerName { get; set; } = string.Empty;
}
