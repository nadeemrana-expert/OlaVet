// =============================================
// File: OlaVet.Application/DTOs/Pet/PetDto.cs
// DTOs for Pet operations
// =============================================

namespace OlaVet.Application.DTOs.Pet;

/// <summary>
/// Basic pet response DTO
/// </summary>
public record PetDto
{
    public int PetId { get; init; }
    public int PetOwnerId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Species { get; init; } = string.Empty;
    public string? Breed { get; init; }
    public int? Age { get; init; }
    public decimal? PetWeight { get; init; }
    public string? Color { get; init; }
    public string? Gender { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
}

/// <summary>
/// Pet with owner info
/// </summary>
public record PetWithOwnerDto : PetDto
{
    public string OwnerName { get; init; } = string.Empty;
    public string OwnerContactNumber { get; init; } = string.Empty;
}

/// <summary>
/// Full pet details with medical history
/// </summary>
public record PetDetailsDto : PetWithOwnerDto
{
    public List<MedicalRecordDto> MedicalHistory { get; init; } = new();
    public List<AppointmentSummaryDto> RecentAppointments { get; init; } = new();
    public int TotalAppointments { get; init; }
}

/// <summary>
/// Medical record summary
/// </summary>
public record MedicalRecordDto
{
    public int MedicalRecordId { get; init; }
    public DateTime RecordDate { get; init; }
    public string RecordType { get; init; } = string.Empty;
    public string? Diagnosis { get; init; }
    public string? Treatment { get; init; }
    public string? Notes { get; init; }
    public string? VetName { get; init; }
}

/// <summary>
/// Appointment summary
/// </summary>
public record AppointmentSummaryDto
{
    public int AppointmentId { get; init; }
    public DateTime AppointmentDate { get; init; }
    public string AppointmentType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? VetName { get; init; }
    public decimal? Fee { get; init; }
}

/// <summary>
/// Request DTO for creating a pet
/// </summary>
public record CreatePetDto
{
    public int PetOwnerId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Species { get; init; } = string.Empty;
    public string? Breed { get; init; }
    public int? Age { get; init; }
    public decimal? PetWeight { get; init; }
    public string? Color { get; init; }
    public string? Gender { get; init; }
}

/// <summary>
/// Request DTO for updating a pet
/// </summary>
public record UpdatePetDto
{
    public string Name { get; init; } = string.Empty;
    public string Species { get; init; } = string.Empty;
    public string? Breed { get; init; }
    public int? Age { get; init; }
    public decimal? PetWeight { get; init; }
    public string? Color { get; init; }
    public string? Gender { get; init; }
}

/// <summary>
/// Request for searching pets
/// </summary>
public record SearchPetsDto
{
    public string? SearchTerm { get; init; }
    public string? Species { get; init; }
    public string? Breed { get; init; }
    public int? OwnerId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
