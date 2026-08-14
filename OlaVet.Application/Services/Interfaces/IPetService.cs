// =============================================
// File: OlaVet.Application/Services/Interfaces/IPetService.cs
// Service interface for Pet business logic
// =============================================

using OlaVet.Application.Common;
using OlaVet.Application.DTOs.Pet;
using OlaVet.Domain.Common;

namespace OlaVet.Application.Services.Interfaces;

public interface IPetService
{
    Task<Result<PetDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PetWithOwnerDto>> GetWithOwnerAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PetDetailsDto>> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<PetDto>>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<PetDto>>> GetByOwnerIdAsync(int ownerId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<PetDto>>> GetBySpeciesAsync(string species, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<PetDto>>> SearchAsync(SearchPetsDto searchDto, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<MedicalRecordDto>>> GetMedicalHistoryAsync(int petId, CancellationToken cancellationToken = default);
    Task<Result<PetDto>> CreateAsync(CreatePetDto dto, CancellationToken cancellationToken = default);
    Task<Result<PetDto>> UpdateAsync(int id, UpdatePetDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
