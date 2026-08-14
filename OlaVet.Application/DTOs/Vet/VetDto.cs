// =============================================
// File: OlaVet.Application/DTOs/Vet/VetDto.cs
// DTOs for Vet operations
// =============================================

namespace OlaVet.Application.DTOs.Vet;

/// <summary>
/// Basic vet response DTO
/// </summary>
public record VetDto
{
    public int VetId { get; init; }
    public string VetName { get; init; } = string.Empty;
    public string Specialization { get; init; } = string.Empty;
    public string ClinicLocation { get; init; } = string.Empty;
    public decimal Fee { get; init; }
    public string ContactNumber { get; init; } = string.Empty;
    public string? Email { get; init; }
    public int? YearsOfExperience { get; init; }
    public string? LicenseNumber { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// Vet with rating info
/// </summary>
public record VetWithRatingDto : VetDto
{
    public double AverageRating { get; init; }
    public int ReviewCount { get; init; }
}

/// <summary>
/// Full vet details with qualifications and services
/// </summary>
public record VetDetailsDto : VetWithRatingDto
{
    public List<QualificationDto> Qualifications { get; init; } = new();
    public List<ServiceDto> Services { get; init; } = new();
    public List<AvailabilityDto> Availability { get; init; } = new();
}

/// <summary>
/// Qualification info
/// </summary>
public record QualificationDto
{
    public int EducationQualificationId { get; init; }
    public string DegreeName { get; init; } = string.Empty;
    public string InstituteName { get; init; } = string.Empty;
    public int YearObtained { get; init; }
}

/// <summary>
/// Service offered by vet
/// </summary>
public record ServiceDto
{
    public int ServiceId { get; init; }
    public string ServiceName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal? ServiceFee { get; init; }
}

/// <summary>
/// Vet availability slot
/// </summary>
public record AvailabilityDto
{
    public int VetAvailabilityId { get; init; }
    public string DayOfWeek { get; init; } = string.Empty;
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public bool IsAvailable { get; init; }
}

/// <summary>
/// Request DTO for creating a vet
/// </summary>
public record CreateVetDto
{
    public string VetName { get; init; } = string.Empty;
    public string Specialization { get; init; } = string.Empty;
    public string ClinicLocation { get; init; } = string.Empty;
    public decimal Fee { get; init; }
    public string ContactNumber { get; init; } = string.Empty;
    public string? Email { get; init; }
    public int? YearsOfExperience { get; init; }
    public string? LicenseNumber { get; init; }
}

/// <summary>
/// Request DTO for updating a vet
/// </summary>
public record UpdateVetDto
{
    public string VetName { get; init; } = string.Empty;
    public string Specialization { get; init; } = string.Empty;
    public string ClinicLocation { get; init; } = string.Empty;
    public decimal Fee { get; init; }
    public string ContactNumber { get; init; } = string.Empty;
    public string? Email { get; init; }
    public int? YearsOfExperience { get; init; }
    public string? LicenseNumber { get; init; }
}

/// <summary>
/// Request for searching vets
/// </summary>
public record SearchVetsDto
{
    public string? SearchTerm { get; init; }
    public string? Specialization { get; init; }
    public decimal? MaxFee { get; init; }
    public double? MinRating { get; init; }
    public bool? IsAvailableNow { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
