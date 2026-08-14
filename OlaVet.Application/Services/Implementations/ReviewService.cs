// =============================================
// File: OlaVet.Application/Services/Implementations/ReviewService.cs
// Service implementation for Review business logic
// =============================================

using AutoMapper;
using FluentValidation;
using OlaVet.Application.Common;
using OlaVet.Application.DTOs.Review;
using OlaVet.Application.Services.Interfaces;
using OlaVet.Domain.Common;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;

namespace OlaVet.Application.Services.Implementations;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateVetReviewDto> _createVetReviewValidator;
    private readonly IValidator<CreateLabReviewDto> _createLabReviewValidator;
    private readonly IValidator<CreateStoreReviewDto> _createStoreReviewValidator;
    
    public ReviewService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateVetReviewDto> createVetReviewValidator,
        IValidator<CreateLabReviewDto> createLabReviewValidator,
        IValidator<CreateStoreReviewDto> createStoreReviewValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createVetReviewValidator = createVetReviewValidator;
        _createLabReviewValidator = createLabReviewValidator;
        _createStoreReviewValidator = createStoreReviewValidator;
    }
    
    // =============================================
    // VET REVIEWS
    // =============================================
    
    public async Task<Result<VetReviewDto>> GetVetReviewByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // Get all reviews and filter - IReviewRepository doesn't have GetById
        var reviews = await _unitOfWork.Reviews.GetVetReviewsAsync(0);
        var review = reviews.FirstOrDefault(r => r.VetReviewId == id);
        
        if (review == null)
            return Result<VetReviewDto>.Failure($"Vet review with ID {id} not found");
        
        return Result<VetReviewDto>.Success(_mapper.Map<VetReviewDto>(review));
    }
    
    public async Task<Result<IEnumerable<VetReviewDto>>> GetVetReviewsAsync(int vetId, CancellationToken cancellationToken = default)
    {
        var reviews = await _unitOfWork.Reviews.GetVetReviewsAsync(vetId);
        return Result<IEnumerable<VetReviewDto>>.Success(_mapper.Map<IEnumerable<VetReviewDto>>(reviews));
    }
    
    public async Task<Result<PagedResult<VetReviewDto>>> GetVetReviewsPagedAsync(int vetId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var reviews = await _unitOfWork.Reviews.GetVetReviewsAsync(vetId);
        var reviewList = reviews.ToList();
        
        var pagedItems = reviewList
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        
        var result = new PagedResult<VetReviewDto>(
            _mapper.Map<IEnumerable<VetReviewDto>>(pagedItems),
            reviewList.Count,
            page,
            pageSize
        );
        
        return Result<PagedResult<VetReviewDto>>.Success(result);
    }
    
    public async Task<Result<VetReviewDto>> CreateVetReviewAsync(CreateVetReviewDto dto, CancellationToken cancellationToken = default)
    {
        // Validate
        var validationResult = await _createVetReviewValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return Result<VetReviewDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
        
        // Verify vet exists
        var vet = await _unitOfWork.Vets.GetByIdAsync(dto.VetId, cancellationToken);
        if (vet == null)
            return Result<VetReviewDto>.Failure($"Vet with ID {dto.VetId} not found");
        
        // Verify owner exists
        var owner = await _unitOfWork.PetOwners.GetByIdAsync(dto.PetOwnerId, cancellationToken);
        if (owner == null)
            return Result<VetReviewDto>.Failure($"Pet owner with ID {dto.PetOwnerId} not found");
        
        // Verify appointment exists
        var appointment = await _unitOfWork.VetAppointments.GetByIdAsync(dto.VetAppointmentId, cancellationToken);
        if (appointment == null)
            return Result<VetReviewDto>.Failure($"Appointment with ID {dto.VetAppointmentId} not found");
        
        // Check if review already exists for this appointment
        var existingReview = await _unitOfWork.Reviews.GetVetReviewByAppointmentAsync(dto.VetAppointmentId);
        if (existingReview != null)
            return Result<VetReviewDto>.Failure("A review already exists for this appointment");
        
        var review = _mapper.Map<VetReview>(dto);
        
        // Note: Full implementation requires adding review entities to DbContext
        // This is a placeholder - infrastructure layer would need to support this
        return Result<VetReviewDto>.Failure("Review creation requires infrastructure support");
    }
    
    public async Task<Result> DeleteVetReviewAsync(int id, CancellationToken cancellationToken = default)
    {
        // Note: IReviewRepository doesn't have delete method currently
        return Result.Failure("Review deletion requires infrastructure support");
    }
    
    // =============================================
    // LAB REVIEWS
    // =============================================
    
    public async Task<Result<LabReviewDto>> GetLabReviewByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var reviews = await _unitOfWork.Reviews.GetLabReviewsAsync(0);
        var review = reviews.FirstOrDefault(r => r.LabReviewId == id);
        
        if (review == null)
            return Result<LabReviewDto>.Failure($"Lab review with ID {id} not found");
        
        return Result<LabReviewDto>.Success(_mapper.Map<LabReviewDto>(review));
    }
    
    public async Task<Result<IEnumerable<LabReviewDto>>> GetLabReviewsAsync(int labId, CancellationToken cancellationToken = default)
    {
        var reviews = await _unitOfWork.Reviews.GetLabReviewsAsync(labId);
        return Result<IEnumerable<LabReviewDto>>.Success(_mapper.Map<IEnumerable<LabReviewDto>>(reviews));
    }
    
    public async Task<Result<PagedResult<LabReviewDto>>> GetLabReviewsPagedAsync(int labId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var reviews = await _unitOfWork.Reviews.GetLabReviewsAsync(labId);
        var reviewList = reviews.ToList();
        
        var pagedItems = reviewList
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        
        var result = new PagedResult<LabReviewDto>(
            _mapper.Map<IEnumerable<LabReviewDto>>(pagedItems),
            reviewList.Count,
            page,
            pageSize
        );
        
        return Result<PagedResult<LabReviewDto>>.Success(result);
    }
    
    public async Task<Result<LabReviewDto>> CreateLabReviewAsync(CreateLabReviewDto dto, CancellationToken cancellationToken = default)
    {
        // Validate
        var validationResult = await _createLabReviewValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return Result<LabReviewDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
        
        // Verify lab exists
        var lab = await _unitOfWork.Labs.GetByIdAsync(dto.LabId, cancellationToken);
        if (lab == null)
            return Result<LabReviewDto>.Failure($"Lab with ID {dto.LabId} not found");
        
        // Verify owner exists
        var owner = await _unitOfWork.PetOwners.GetByIdAsync(dto.PetOwnerId, cancellationToken);
        if (owner == null)
            return Result<LabReviewDto>.Failure($"Pet owner with ID {dto.PetOwnerId} not found");
        
        // Check if review already exists
        var existingReview = await _unitOfWork.Reviews.GetLabReviewByAppointmentAsync(dto.LabAppointmentId);
        if (existingReview != null)
            return Result<LabReviewDto>.Failure("A review already exists for this appointment");
        
        return Result<LabReviewDto>.Failure("Review creation requires infrastructure support");
    }
    
    public async Task<Result> DeleteLabReviewAsync(int id, CancellationToken cancellationToken = default)
    {
        return Result.Failure("Review deletion requires infrastructure support");
    }
    
    // =============================================
    // STORE REVIEWS
    // =============================================
    
    public async Task<Result<StoreReviewDto>> GetStoreReviewByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var reviews = await _unitOfWork.Reviews.GetStoreReviewsAsync(0);
        var review = reviews.FirstOrDefault(r => r.StoreReviewId == id);
        
        if (review == null)
            return Result<StoreReviewDto>.Failure($"Store review with ID {id} not found");
        
        return Result<StoreReviewDto>.Success(_mapper.Map<StoreReviewDto>(review));
    }
    
    public async Task<Result<IEnumerable<StoreReviewDto>>> GetStoreReviewsAsync(int storeId, CancellationToken cancellationToken = default)
    {
        var reviews = await _unitOfWork.Reviews.GetStoreReviewsAsync(storeId);
        return Result<IEnumerable<StoreReviewDto>>.Success(_mapper.Map<IEnumerable<StoreReviewDto>>(reviews));
    }
    
    public async Task<Result<PagedResult<StoreReviewDto>>> GetStoreReviewsPagedAsync(int storeId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var reviews = await _unitOfWork.Reviews.GetStoreReviewsAsync(storeId);
        var reviewList = reviews.ToList();
        
        var pagedItems = reviewList
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        
        var result = new PagedResult<StoreReviewDto>(
            _mapper.Map<IEnumerable<StoreReviewDto>>(pagedItems),
            reviewList.Count,
            page,
            pageSize
        );
        
        return Result<PagedResult<StoreReviewDto>>.Success(result);
    }
    
    public async Task<Result<StoreReviewDto>> CreateStoreReviewAsync(CreateStoreReviewDto dto, CancellationToken cancellationToken = default)
    {
        // Validate
        var validationResult = await _createStoreReviewValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return Result<StoreReviewDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
        
        // Verify store exists
        var store = await _unitOfWork.Stores.GetByIdAsync(dto.StoreId, cancellationToken);
        if (store == null)
            return Result<StoreReviewDto>.Failure($"Store with ID {dto.StoreId} not found");
        
        // Verify owner exists
        var owner = await _unitOfWork.PetOwners.GetByIdAsync(dto.PetOwnerId, cancellationToken);
        if (owner == null)
            return Result<StoreReviewDto>.Failure($"Pet owner with ID {dto.PetOwnerId} not found");
        
        // Check if review already exists
        var existingReview = await _unitOfWork.Reviews.GetStoreReviewByOrderAsync(dto.MedicineOrderId);
        if (existingReview != null)
            return Result<StoreReviewDto>.Failure("A review already exists for this order");
        
        return Result<StoreReviewDto>.Failure("Review creation requires infrastructure support");
    }
    
    public async Task<Result> DeleteStoreReviewAsync(int id, CancellationToken cancellationToken = default)
    {
        return Result.Failure("Review deletion requires infrastructure support");
    }
    
    // =============================================
    // RATING DISTRIBUTIONS
    // =============================================
    
    public async Task<Result<RatingDistributionDto>> GetVetRatingDistributionAsync(int vetId, CancellationToken cancellationToken = default)
    {
        var distribution = await _unitOfWork.Reviews.GetRatingDistributionAsync("Vet", vetId);
        
        return Result<RatingDistributionDto>.Success(new RatingDistributionDto
        {
            FiveStars = distribution.FiveStar,
            FourStars = distribution.FourStar,
            ThreeStars = distribution.ThreeStar,
            TwoStars = distribution.TwoStar,
            OneStar = distribution.OneStar,
            TotalReviews = distribution.Total,
            AverageRating = distribution.Total > 0 
                ? (distribution.FiveStar * 5 + distribution.FourStar * 4 + distribution.ThreeStar * 3 + 
                   distribution.TwoStar * 2 + distribution.OneStar) / (double)distribution.Total 
                : 0
        });
    }
    
    public async Task<Result<RatingDistributionDto>> GetLabRatingDistributionAsync(int labId, CancellationToken cancellationToken = default)
    {
        var distribution = await _unitOfWork.Reviews.GetRatingDistributionAsync("Lab", labId);
        
        return Result<RatingDistributionDto>.Success(new RatingDistributionDto
        {
            FiveStars = distribution.FiveStar,
            FourStars = distribution.FourStar,
            ThreeStars = distribution.ThreeStar,
            TwoStars = distribution.TwoStar,
            OneStar = distribution.OneStar,
            TotalReviews = distribution.Total,
            AverageRating = distribution.Total > 0 
                ? (distribution.FiveStar * 5 + distribution.FourStar * 4 + distribution.ThreeStar * 3 + 
                   distribution.TwoStar * 2 + distribution.OneStar) / (double)distribution.Total 
                : 0
        });
    }
    
    public async Task<Result<RatingDistributionDto>> GetStoreRatingDistributionAsync(int storeId, CancellationToken cancellationToken = default)
    {
        var distribution = await _unitOfWork.Reviews.GetRatingDistributionAsync("Store", storeId);
        
        return Result<RatingDistributionDto>.Success(new RatingDistributionDto
        {
            FiveStars = distribution.FiveStar,
            FourStars = distribution.FourStar,
            ThreeStars = distribution.ThreeStar,
            TwoStars = distribution.TwoStar,
            OneStar = distribution.OneStar,
            TotalReviews = distribution.Total,
            AverageRating = distribution.Total > 0 
                ? (distribution.FiveStar * 5 + distribution.FourStar * 4 + distribution.ThreeStar * 3 + 
                   distribution.TwoStar * 2 + distribution.OneStar) / (double)distribution.Total 
                : 0
        });
    }
}
