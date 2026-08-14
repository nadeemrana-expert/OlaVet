// =============================================
// File: OlaVet.Application/Services/Implementations/VetService.cs
// Service implementation for Vet business logic
// =============================================

using AutoMapper;
using FluentValidation;
using OlaVet.Application.Common;
using OlaVet.Application.DTOs.Review;
using OlaVet.Application.DTOs.Vet;
using OlaVet.Application.Services.Interfaces;
using OlaVet.Domain.Common;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;

namespace OlaVet.Application.Services.Implementations;

public class VetService : IVetService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateVetDto> _createValidator;
    private readonly IValidator<UpdateVetDto> _updateValidator;
    
    public VetService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateVetDto> createValidator,
        IValidator<UpdateVetDto> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }
    
    public async Task<Result<VetDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var vet = await _unitOfWork.Vets.GetByIdAsync(id, cancellationToken);
        
        if (vet == null)
            return Result<VetDto>.Failure($"Vet with ID {id} not found");
        
        return Result<VetDto>.Success(_mapper.Map<VetDto>(vet));
    }
    
    public async Task<Result<VetDetailsDto>> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        var vet = await _unitOfWork.Vets.GetWithDetailsAsync(id);
        
        if (vet == null)
            return Result<VetDetailsDto>.Failure($"Vet with ID {id} not found");
        
        return Result<VetDetailsDto>.Success(_mapper.Map<VetDetailsDto>(vet));
    }
    
    public async Task<Result<PagedResult<VetWithRatingDto>>> GetWithRatingsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var vetsWithRatings = await _unitOfWork.Vets.GetVetsWithRatingsAsync(page, pageSize, cancellationToken);
        
        var dtos = vetsWithRatings.Items.Select(v => new VetWithRatingDto
        {
            VetId = v.VetId,
            VetName = v.VetName,
            Specialization = v.Specialization,
            ClinicLocation = v.ClinicLocation,
            Fee = v.Fee,
            ContactNumber = v.ContactNumber,
            Email = v.Email,
            YearsOfExperience = v.YearsOfExperience,
            LicenseNumber = v.LicenseNumber,
            IsActive = v.IsActive,
            AverageRating = v.AverageRating,
            ReviewCount = v.ReviewCount
        });
        
        var result = new PagedResult<VetWithRatingDto>(dtos, vetsWithRatings.TotalCount, page, pageSize);
        return Result<PagedResult<VetWithRatingDto>>.Success(result);
    }
    
    public async Task<Result<IEnumerable<VetWithRatingDto>>> GetTopRatedAsync(int count, CancellationToken cancellationToken = default)
    {
        if (count <= 0 || count > 100)
            count = 10;
        
        var vetsWithRatings = await _unitOfWork.Vets.GetVetsWithRatingsAsync(1, count, cancellationToken);
        
        var dtos = vetsWithRatings.Items
            .OrderByDescending(v => v.AverageRating)
            .ThenByDescending(v => v.ReviewCount)
            .Select(v => new VetWithRatingDto
            {
                VetId = v.VetId,
                VetName = v.VetName,
                Specialization = v.Specialization,
                ClinicLocation = v.ClinicLocation,
                Fee = v.Fee,
                ContactNumber = v.ContactNumber,
                Email = v.Email,
                YearsOfExperience = v.YearsOfExperience,
                LicenseNumber = v.LicenseNumber,
                IsActive = v.IsActive,
                AverageRating = v.AverageRating,
                ReviewCount = v.ReviewCount
            });
        
        return Result<IEnumerable<VetWithRatingDto>>.Success(dtos);
    }
    
    public async Task<Result<IEnumerable<VetDto>>> GetBySpecializationAsync(string specialization, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(specialization))
            return Result<IEnumerable<VetDto>>.Failure("Specialization is required");
        
        var vets = await _unitOfWork.Vets.GetBySpecializationAsync(specialization);
        return Result<IEnumerable<VetDto>>.Success(_mapper.Map<IEnumerable<VetDto>>(vets));
    }
    
    public async Task<Result<IEnumerable<VetDto>>> SearchAsync(SearchVetsDto searchDto, CancellationToken cancellationToken = default)
    {
        var vets = await _unitOfWork.Vets.SearchAsync(searchDto.SearchTerm ?? "");
        
        // Apply additional filters
        var filtered = vets.AsEnumerable();
        
        if (!string.IsNullOrEmpty(searchDto.Specialization))
            filtered = filtered.Where(v => v.Specialization != null && 
                v.Specialization.Contains(searchDto.Specialization, StringComparison.OrdinalIgnoreCase));
        
        if (searchDto.MaxFee.HasValue)
            filtered = filtered.Where(v => v.Fee <= searchDto.MaxFee.Value);
        
        return Result<IEnumerable<VetDto>>.Success(_mapper.Map<IEnumerable<VetDto>>(filtered));
    }
    
    public async Task<Result<IEnumerable<VetDto>>> GetAvailableAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var availableVets = await _unitOfWork.Vets.GetAvailableVetsAsync(date);
        return Result<IEnumerable<VetDto>>.Success(_mapper.Map<IEnumerable<VetDto>>(availableVets));
    }
    
    public async Task<Result<VetDto>> CreateAsync(CreateVetDto dto, CancellationToken cancellationToken = default)
    {
        // Validate
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return Result<VetDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
        
        // Check for duplicate license number
        if (!string.IsNullOrEmpty(dto.LicenseNumber))
        {
            var existingVets = await _unitOfWork.Vets.SearchAsync(dto.LicenseNumber);
            if (existingVets.Any(v => v.LicenseNumber == dto.LicenseNumber))
                return Result<VetDto>.Failure("A vet with this license number already exists");
        }
        
        var vet = _mapper.Map<Vet>(dto);
        
        await _unitOfWork.Vets.AddAsync(vet, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<VetDto>.Success(_mapper.Map<VetDto>(vet));
    }
    
    public async Task<Result<VetDto>> UpdateAsync(int id, UpdateVetDto dto, CancellationToken cancellationToken = default)
    {
        // Validate
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return Result<VetDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
        
        var vet = await _unitOfWork.Vets.GetByIdAsync(id, cancellationToken);
        if (vet == null)
            return Result<VetDto>.Failure($"Vet with ID {id} not found");
        
        // Update
        vet.VetName = dto.VetName;
        vet.Specialization = dto.Specialization;
        vet.ClinicLocation = dto.ClinicLocation;
        vet.Fee = dto.Fee;
        vet.ContactNumber = dto.ContactNumber;
        vet.Email = dto.Email;
        vet.YearsOfExperience = dto.YearsOfExperience;
        vet.LicenseNumber = dto.LicenseNumber;
        vet.ModifiedDate = DateTime.UtcNow;
        
        _unitOfWork.Vets.Update(vet);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<VetDto>.Success(_mapper.Map<VetDto>(vet));
    }
    
    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var vet = await _unitOfWork.Vets.GetByIdAsync(id, cancellationToken);
        if (vet == null)
            return Result.Failure($"Vet with ID {id} not found");
        
        // Soft delete
        vet.IsActive = false;
        vet.ModifiedDate = DateTime.UtcNow;
        
        _unitOfWork.Vets.Update(vet);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
    
    public async Task<Result<IEnumerable<VetReviewDto>>> GetReviewsAsync(int vetId, CancellationToken cancellationToken = default)
    {
        var vet = await _unitOfWork.Vets.GetByIdAsync(vetId, cancellationToken);
        if (vet == null)
            return Result<IEnumerable<VetReviewDto>>.Failure($"Vet with ID {vetId} not found");
        
        var reviews = await _unitOfWork.Reviews.GetVetReviewsAsync(vetId);
        return Result<IEnumerable<VetReviewDto>>.Success(_mapper.Map<IEnumerable<VetReviewDto>>(reviews));
    }
    
    public async Task<Result<RatingDistributionDto>> GetRatingDistributionAsync(int vetId, CancellationToken cancellationToken = default)
    {
        var vet = await _unitOfWork.Vets.GetByIdAsync(vetId, cancellationToken);
        if (vet == null)
            return Result<RatingDistributionDto>.Failure($"Vet with ID {vetId} not found");
        
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
}
