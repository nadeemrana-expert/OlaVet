// =============================================
// File: OlaVet.Application/DTOs/Review/ReviewDto.cs
// DTOs for Review operations
// =============================================

namespace OlaVet.Application.DTOs.Review;

/// <summary>
/// Base review DTO
/// </summary>
public record ReviewDto
{
    public int ReviewId { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
    public DateTime ReviewDate { get; init; }
    public string ReviewerName { get; init; } = string.Empty;
}

/// <summary>
/// Vet review
/// </summary>
public record VetReviewDto : ReviewDto
{
    public int VetReviewId { get; init; }
    public int VetId { get; init; }
    public string VetName { get; init; } = string.Empty;
}

/// <summary>
/// Lab review
/// </summary>
public record LabReviewDto : ReviewDto
{
    public int LabReviewId { get; init; }
    public int LabId { get; init; }
    public string LabName { get; init; } = string.Empty;
}

/// <summary>
/// Store review
/// </summary>
public record StoreReviewDto : ReviewDto
{
    public int StoreReviewId { get; init; }
    public int StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;
}

/// <summary>
/// Request to create a vet review
/// </summary>
public record CreateVetReviewDto
{
    public int VetId { get; init; }
    public int PetOwnerId { get; init; }
    public int VetAppointmentId { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
}

/// <summary>
/// Request to create a lab review
/// </summary>
public record CreateLabReviewDto
{
    public int LabId { get; init; }
    public int PetOwnerId { get; init; }
    public int LabAppointmentId { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
}

/// <summary>
/// Request to create a store review
/// </summary>
public record CreateStoreReviewDto
{
    public int StoreId { get; init; }
    public int PetOwnerId { get; init; }
    public int MedicineOrderId { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
}

/// <summary>
/// Rating distribution summary
/// </summary>
public record RatingDistributionDto
{
    public int FiveStars { get; init; }
    public int FourStars { get; init; }
    public int ThreeStars { get; init; }
    public int TwoStars { get; init; }
    public int OneStar { get; init; }
    public int TotalReviews { get; init; }
    public double AverageRating { get; init; }
}
