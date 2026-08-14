// =============================================
// File: OlaVet.Application/Services/Implementations/PetService.cs
// Service implementation for Pet business logic
// =============================================

using AutoMapper;
using FluentValidation;
using OlaVet.Application.Common;
using OlaVet.Application.DTOs.Pet;
using OlaVet.Application.Services.Interfaces;
using OlaVet.Domain.Common;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;

namespace OlaVet.Application.Services.Implementations;

public class PetService : IPetService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePetDto> _createValidator;
    private readonly IValidator<UpdatePetDto> _updateValidator;
    
    public PetService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreatePetDto> createValidator,
        IValidator<UpdatePetDto> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }
    
    public async Task<Result<PetDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var pet = await _unitOfWork.Pets.GetByIdAsync(id, cancellationToken);
        
        if (pet == null)
            return Result<PetDto>.Failure($"Pet with ID {id} not found");
        
        return Result<PetDto>.Success(_mapper.Map<PetDto>(pet));
    }
    
    public async Task<Result<PetWithOwnerDto>> GetWithOwnerAsync(int id, CancellationToken cancellationToken = default)
    {
        var pet = await _unitOfWork.Pets.GetWithOwnerAsync(id);
        
        if (pet == null)
            return Result<PetWithOwnerDto>.Failure($"Pet with ID {id} not found");
        
        return Result<PetWithOwnerDto>.Success(_mapper.Map<PetWithOwnerDto>(pet));
    }
    
    public async Task<Result<PetDetailsDto>> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        var pet = await _unitOfWork.Pets.GetWithMedicalHistoryAsync(id);
        
        if (pet == null)
            return Result<PetDetailsDto>.Failure($"Pet with ID {id} not found");
        
        var dto = _mapper.Map<PetDetailsDto>(pet);
        
        return Result<PetDetailsDto>.Success(dto with
        {
            MedicalHistory = _mapper.Map<List<MedicalRecordDto>>(pet.MedicalRecords),
            TotalAppointments = pet.VetAppointments?.Count ?? 0
        });
    }
    
    public async Task<Result<PagedResult<PetDto>>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.Pets.GetPagedAsync(page, pageSize, null, null, true, cancellationToken);
        
        var dtos = new PagedResult<PetDto>(
            _mapper.Map<IEnumerable<PetDto>>(result.Items),
            result.TotalCount,
            result.Page,
            result.PageSize
        );
        
        return Result<PagedResult<PetDto>>.Success(dtos);
    }
    
    public async Task<Result<IEnumerable<PetDto>>> GetByOwnerIdAsync(int ownerId, CancellationToken cancellationToken = default)
    {
        // Verify owner exists
        var owner = await _unitOfWork.PetOwners.GetByIdAsync(ownerId, cancellationToken);
        if (owner == null)
            return Result<IEnumerable<PetDto>>.Failure($"Pet owner with ID {ownerId} not found");
        
        var pets = await _unitOfWork.Pets.GetByOwnerIdAsync(ownerId);
        return Result<IEnumerable<PetDto>>.Success(_mapper.Map<IEnumerable<PetDto>>(pets));
    }
    
    public async Task<Result<IEnumerable<PetDto>>> GetBySpeciesAsync(string species, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(species))
            return Result<IEnumerable<PetDto>>.Failure("Species is required");
        
        var pets = await _unitOfWork.Pets.GetBySpeciesAsync(species);
        return Result<IEnumerable<PetDto>>.Success(_mapper.Map<IEnumerable<PetDto>>(pets));
    }
    
    public async Task<Result<IEnumerable<PetDto>>> SearchAsync(SearchPetsDto searchDto, CancellationToken cancellationToken = default)
    {
        // IPetRepository doesn't have SearchAsync, so we get all and filter
        var allPets = await _unitOfWork.Pets.GetAllAsync(cancellationToken);
        
        var filtered = allPets.AsEnumerable();
        
        if (!string.IsNullOrEmpty(searchDto.SearchTerm))
            filtered = filtered.Where(p => 
                p.Name.Contains(searchDto.SearchTerm, StringComparison.OrdinalIgnoreCase));
        
        if (!string.IsNullOrEmpty(searchDto.Species))
            filtered = filtered.Where(p => 
                p.Species.Contains(searchDto.Species, StringComparison.OrdinalIgnoreCase));
        
        if (!string.IsNullOrEmpty(searchDto.Breed))
            filtered = filtered.Where(p => 
                p.Breed != null && p.Breed.Contains(searchDto.Breed, StringComparison.OrdinalIgnoreCase));
        
        if (searchDto.OwnerId.HasValue)
            filtered = filtered.Where(p => p.PetOwnerId == searchDto.OwnerId.Value);
        
        return Result<IEnumerable<PetDto>>.Success(_mapper.Map<IEnumerable<PetDto>>(filtered));
    }
    
    public async Task<Result<IEnumerable<MedicalRecordDto>>> GetMedicalHistoryAsync(int petId, CancellationToken cancellationToken = default)
    {
        var pet = await _unitOfWork.Pets.GetByIdAsync(petId, cancellationToken);
        if (pet == null)
            return Result<IEnumerable<MedicalRecordDto>>.Failure($"Pet with ID {petId} not found");
        
        var records = await _unitOfWork.MedicalRecords.GetByPetIdAsync(petId);
        return Result<IEnumerable<MedicalRecordDto>>.Success(_mapper.Map<IEnumerable<MedicalRecordDto>>(records));
    }
    
    public async Task<Result<PetDto>> CreateAsync(CreatePetDto dto, CancellationToken cancellationToken = default)
    {
        // Validate
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return Result<PetDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
        
        // Verify owner exists
        var owner = await _unitOfWork.PetOwners.GetByIdAsync(dto.PetOwnerId, cancellationToken);
        if (owner == null)
            return Result<PetDto>.Failure($"Pet owner with ID {dto.PetOwnerId} not found");
        
        var pet = _mapper.Map<Pet>(dto);
        
        await _unitOfWork.Pets.AddAsync(pet, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<PetDto>.Success(_mapper.Map<PetDto>(pet));
    }
    
    public async Task<Result<PetDto>> UpdateAsync(int id, UpdatePetDto dto, CancellationToken cancellationToken = default)
    {
        // Validate
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return Result<PetDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
        
        var pet = await _unitOfWork.Pets.GetByIdAsync(id, cancellationToken);
        if (pet == null)
            return Result<PetDto>.Failure($"Pet with ID {id} not found");
        
        // Update
        pet.Name = dto.Name;
        pet.Species = dto.Species;
        pet.Breed = dto.Breed;
        pet.Age = dto.Age;
        pet.PetWeight = dto.PetWeight;
        pet.Color = dto.Color;
        pet.Gender = dto.Gender;
        pet.ModifiedDate = DateTime.UtcNow;
        
        _unitOfWork.Pets.Update(pet);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<PetDto>.Success(_mapper.Map<PetDto>(pet));
    }
    
    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var pet = await _unitOfWork.Pets.GetByIdAsync(id, cancellationToken);
        if (pet == null)
            return Result.Failure($"Pet with ID {id} not found");
        
        // Soft delete
        pet.IsActive = false;
        pet.ModifiedDate = DateTime.UtcNow;
        
        _unitOfWork.Pets.Update(pet);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}
