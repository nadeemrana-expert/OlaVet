// =============================================
// File: OlaVet.Application/Services/Interfaces/IPetOwnerService.cs
// Service interface for PetOwner business logic
// =============================================

using OlaVet.Application.Common;
using OlaVet.Application.DTOs.PetOwner;
using OlaVet.Domain.Common;

namespace OlaVet.Application.Services.Interfaces;

public interface IPetOwnerService
{
    Task<Result<PetOwnerDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PetOwnerDetailsDto>> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<PetOwnerDto>>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<PetOwnerDto>>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Result<PetOwnerDto>> CreateAsync(CreatePetOwnerDto dto, CancellationToken cancellationToken = default);
    Task<Result<PetOwnerDto>> UpdateAsync(int id, UpdatePetOwnerDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<decimal>> AddFundsAsync(int id, AddFundsDto dto, CancellationToken cancellationToken = default);
    Task<Result<OwnerPaymentSummaryDto>> GetPaymentSummaryAsync(int id, CancellationToken cancellationToken = default);
}
