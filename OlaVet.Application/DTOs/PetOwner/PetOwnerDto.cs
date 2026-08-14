// =============================================
// File: OlaVet.Application/DTOs/PetOwner/PetOwnerDto.cs
// DTOs for PetOwner operations
// =============================================

namespace OlaVet.Application.DTOs.PetOwner;

/// <summary>
/// Response DTO for pet owner - what clients receive
/// </summary>
public record PetOwnerDto
{
    public int PetOwnerId { get; init; }
    public string OwnerName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string ContactNumber { get; init; } = string.Empty;
    public string? HomeAddress { get; init; }
    public int? Age { get; init; }
    public string? Gender { get; init; }
    public decimal Wallet { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
}

/// <summary>
/// Full details including pets
/// </summary>
public record PetOwnerDetailsDto : PetOwnerDto
{
    public List<PetSummaryDto> Pets { get; init; } = new();
    public int TotalAppointments { get; init; }
    public decimal TotalSpent { get; init; }
}

/// <summary>
/// Simple pet info for owner details
/// </summary>
public record PetSummaryDto
{
    public int PetId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Species { get; init; } = string.Empty;
    public string? Breed { get; init; }
    public int? Age { get; init; }
}

/// <summary>
/// Request DTO for creating a pet owner
/// </summary>
public record CreatePetOwnerDto
{
    public string OwnerName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string ContactNumber { get; init; } = string.Empty;
    public string? HomeAddress { get; init; }
    public int? Age { get; init; }
    public string? Gender { get; init; }
    public decimal InitialWalletBalance { get; init; }
}

/// <summary>
/// Request DTO for updating a pet owner
/// </summary>
public record UpdatePetOwnerDto
{
    public string OwnerName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string ContactNumber { get; init; } = string.Empty;
    public string? HomeAddress { get; init; }
    public int? Age { get; init; }
    public string? Gender { get; init; }
}

/// <summary>
/// Request DTO for adding funds to wallet
/// </summary>
public record AddFundsDto
{
    public decimal Amount { get; init; }
    public string? PaymentMethod { get; init; }
    public string? TransactionReference { get; init; }
}

/// <summary>
/// Payment summary for owner
/// </summary>
public record OwnerPaymentSummaryDto
{
    public decimal TotalVetPayments { get; init; }
    public decimal TotalLabPayments { get; init; }
    public decimal TotalStorePayments { get; init; }
    public decimal GrandTotal { get; init; }
    public int TotalTransactions { get; init; }
}
