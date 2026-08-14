// =============================================
// File: OlaVet.Application/Services/Implementations/PetOwnerService.cs
// Service implementation for PetOwner business logic
// =============================================

using AutoMapper;
using FluentValidation;
using OlaVet.Application.Common;
using OlaVet.Application.DTOs.PetOwner;
using OlaVet.Application.Exceptions;
using OlaVet.Application.Services.Interfaces;
using OlaVet.Domain.Common;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;

namespace OlaVet.Application.Services.Implementations;

public class PetOwnerService : IPetOwnerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePetOwnerDto> _createValidator;
    private readonly IValidator<UpdatePetOwnerDto> _updateValidator;
    private readonly IValidator<AddFundsDto> _addFundsValidator;
    
    public PetOwnerService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreatePetOwnerDto> createValidator,
        IValidator<UpdatePetOwnerDto> updateValidator,
        IValidator<AddFundsDto> addFundsValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _addFundsValidator = addFundsValidator;
    }
    
    public async Task<Result<PetOwnerDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var owner = await _unitOfWork.PetOwners.GetByIdAsync(id, cancellationToken);
        
        if (owner == null)
            return Result<PetOwnerDto>.Failure($"Pet owner with ID {id} not found");
        
        return Result<PetOwnerDto>.Success(_mapper.Map<PetOwnerDto>(owner));
    }
    
    public async Task<Result<PetOwnerDetailsDto>> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        var owner = await _unitOfWork.PetOwners.GetWithPetsAsync(id);
        
        if (owner == null)
            return Result<PetOwnerDetailsDto>.Failure($"Pet owner with ID {id} not found");
        
        var dto = _mapper.Map<PetOwnerDetailsDto>(owner);
        
        // Get additional stats
        var paymentSummary = await _unitOfWork.Payments.GetOwnerPaymentSummaryAsync(id);
        
        return Result<PetOwnerDetailsDto>.Success(dto with
        {
            TotalSpent = paymentSummary.GrandTotal,
            TotalAppointments = owner.Pets.SelectMany(p => p.VetAppointments ?? Enumerable.Empty<VetAppointment>()).Count()
        });
    }
    
    public async Task<Result<PagedResult<PetOwnerDto>>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.PetOwners.GetPagedAsync(page, pageSize, null, null, true, cancellationToken);
        
        var dtos = new PagedResult<PetOwnerDto>(
            _mapper.Map<IEnumerable<PetOwnerDto>>(result.Items),
            result.TotalCount,
            result.Page,
            result.PageSize
        );
        
        return Result<PagedResult<PetOwnerDto>>.Success(dtos);
    }
    
    public async Task<Result<IEnumerable<PetOwnerDto>>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Result<IEnumerable<PetOwnerDto>>.Failure("Search term is required");
        
        var owners = await _unitOfWork.PetOwners.SearchAsync(searchTerm);
        return Result<IEnumerable<PetOwnerDto>>.Success(_mapper.Map<IEnumerable<PetOwnerDto>>(owners));
    }
    
    public async Task<Result<PetOwnerDto>> CreateAsync(CreatePetOwnerDto dto, CancellationToken cancellationToken = default)
    {
        // Validate
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return Result<PetOwnerDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
        
        // Check for duplicate email
        var existingOwners = await _unitOfWork.PetOwners.SearchAsync(dto.Email);
        if (existingOwners.Any(o => o.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)))
            return Result<PetOwnerDto>.Failure("A pet owner with this email already exists");
        
        // Map and create
        var owner = _mapper.Map<PetOwner>(dto);
        
        await _unitOfWork.PetOwners.AddAsync(owner, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<PetOwnerDto>.Success(_mapper.Map<PetOwnerDto>(owner));
    }
    
    public async Task<Result<PetOwnerDto>> UpdateAsync(int id, UpdatePetOwnerDto dto, CancellationToken cancellationToken = default)
    {
        // Validate
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return Result<PetOwnerDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
        
        // Get existing
        var owner = await _unitOfWork.PetOwners.GetByIdAsync(id, cancellationToken);
        if (owner == null)
            return Result<PetOwnerDto>.Failure($"Pet owner with ID {id} not found");
        
        // Check for duplicate email (if changed)
        if (!owner.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existingOwners = await _unitOfWork.PetOwners.SearchAsync(dto.Email);
            if (existingOwners.Any(o => o.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase) && o.PetOwnerId != id))
                return Result<PetOwnerDto>.Failure("A pet owner with this email already exists");
        }
        
        // Update
        owner.OwnerName = dto.OwnerName;
        owner.Email = dto.Email;
        owner.ContactNumber = dto.ContactNumber;
        owner.HomeAddress = dto.HomeAddress;
        owner.Age = dto.Age;
        owner.Gender = dto.Gender;
        owner.ModifiedDate = DateTime.UtcNow;
        
        _unitOfWork.PetOwners.Update(owner);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<PetOwnerDto>.Success(_mapper.Map<PetOwnerDto>(owner));
    }
    
    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var owner = await _unitOfWork.PetOwners.GetByIdAsync(id, cancellationToken);
        if (owner == null)
            return Result.Failure($"Pet owner with ID {id} not found");
        
        // Soft delete
        owner.IsActive = false;
        owner.ModifiedDate = DateTime.UtcNow;
        
        _unitOfWork.PetOwners.Update(owner);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
    
    public async Task<Result<decimal>> AddFundsAsync(int id, AddFundsDto dto, CancellationToken cancellationToken = default)
    {
        // Validate
        var validationResult = await _addFundsValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return Result<decimal>.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
        
        var owner = await _unitOfWork.PetOwners.GetByIdAsync(id, cancellationToken);
        if (owner == null)
            return Result<decimal>.Failure($"Pet owner with ID {id} not found");
        
        // Add funds
        owner.Wallet += dto.Amount;
        owner.ModifiedDate = DateTime.UtcNow;
        
        _unitOfWork.PetOwners.Update(owner);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<decimal>.Success(owner.Wallet);
    }
    
    public async Task<Result<OwnerPaymentSummaryDto>> GetPaymentSummaryAsync(int id, CancellationToken cancellationToken = default)
    {
        var owner = await _unitOfWork.PetOwners.GetByIdAsync(id, cancellationToken);
        if (owner == null)
            return Result<OwnerPaymentSummaryDto>.Failure($"Pet owner with ID {id} not found");
        
        var summary = await _unitOfWork.Payments.GetOwnerPaymentSummaryAsync(id);
        
        return Result<OwnerPaymentSummaryDto>.Success(new OwnerPaymentSummaryDto
        {
            TotalVetPayments = summary.TotalVetPayments,
            TotalLabPayments = summary.TotalLabPayments,
            TotalStorePayments = summary.TotalStorePayments,
            GrandTotal = summary.GrandTotal,
            TotalTransactions = summary.VetPaymentCount + summary.LabPaymentCount + summary.StorePaymentCount
        });
    }
}
