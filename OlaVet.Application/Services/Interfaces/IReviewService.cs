// =============================================
// File: OlaVet.Application/Services/Interfaces/IReviewService.cs
// Service interface for Review business logic
// =============================================

using OlaVet.Application.Common;
using OlaVet.Application.DTOs.Review;
using OlaVet.Domain.Common;

namespace OlaVet.Application.Services.Interfaces;

public interface IReviewService
{
    // Vet Reviews
    Task<Result<VetReviewDto>> GetVetReviewByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<VetReviewDto>>> GetVetReviewsAsync(int vetId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<VetReviewDto>>> GetVetReviewsPagedAsync(int vetId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<VetReviewDto>> CreateVetReviewAsync(CreateVetReviewDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteVetReviewAsync(int id, CancellationToken cancellationToken = default);
    
    // Lab Reviews
    Task<Result<LabReviewDto>> GetLabReviewByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<LabReviewDto>>> GetLabReviewsAsync(int labId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<LabReviewDto>>> GetLabReviewsPagedAsync(int labId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<LabReviewDto>> CreateLabReviewAsync(CreateLabReviewDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteLabReviewAsync(int id, CancellationToken cancellationToken = default);
    
    // Store Reviews
    Task<Result<StoreReviewDto>> GetStoreReviewByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<StoreReviewDto>>> GetStoreReviewsAsync(int storeId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<StoreReviewDto>>> GetStoreReviewsPagedAsync(int storeId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<StoreReviewDto>> CreateStoreReviewAsync(CreateStoreReviewDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteStoreReviewAsync(int id, CancellationToken cancellationToken = default);
    
    // Rating summaries
    Task<Result<RatingDistributionDto>> GetVetRatingDistributionAsync(int vetId, CancellationToken cancellationToken = default);
    Task<Result<RatingDistributionDto>> GetLabRatingDistributionAsync(int labId, CancellationToken cancellationToken = default);
    Task<Result<RatingDistributionDto>> GetStoreRatingDistributionAsync(int storeId, CancellationToken cancellationToken = default);
}
