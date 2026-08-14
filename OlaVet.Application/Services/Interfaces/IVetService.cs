// =============================================
// File: OlaVet.Application/Services/Interfaces/IVetService.cs
// Service interface for Vet business logic
// =============================================

using OlaVet.Application.Common;
using OlaVet.Application.DTOs.Review;
using OlaVet.Application.DTOs.Vet;
using OlaVet.Domain.Common;

namespace OlaVet.Application.Services.Interfaces;

public interface IVetService
{
    Task<Result<VetDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<VetDetailsDto>> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<VetWithRatingDto>>> GetWithRatingsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<VetWithRatingDto>>> GetTopRatedAsync(int count, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<VetDto>>> GetBySpecializationAsync(string specialization, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<VetDto>>> SearchAsync(SearchVetsDto searchDto, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<VetDto>>> GetAvailableAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<Result<VetDto>> CreateAsync(CreateVetDto dto, CancellationToken cancellationToken = default);
    Task<Result<VetDto>> UpdateAsync(int id, UpdateVetDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<VetReviewDto>>> GetReviewsAsync(int vetId, CancellationToken cancellationToken = default);
    Task<Result<RatingDistributionDto>> GetRatingDistributionAsync(int vetId, CancellationToken cancellationToken = default);
}
