// =============================================
// File: OlaVet.Application/Validators/ReviewValidators.cs
// FluentValidation validators for Review DTOs
// =============================================

using FluentValidation;
using OlaVet.Application.DTOs.Review;

namespace OlaVet.Application.Validators;

public class CreateVetReviewValidator : AbstractValidator<CreateVetReviewDto>
{
    public CreateVetReviewValidator()
    {
        RuleFor(x => x.VetId)
            .GreaterThan(0).WithMessage("Vet is required");
        
        RuleFor(x => x.PetOwnerId)
            .GreaterThan(0).WithMessage("Pet owner is required");
        
        RuleFor(x => x.VetAppointmentId)
            .GreaterThan(0).WithMessage("Appointment is required");
        
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");
        
        RuleFor(x => x.Comment)
            .MaximumLength(2000).WithMessage("Comment cannot exceed 2000 characters");
    }
}

public class CreateLabReviewValidator : AbstractValidator<CreateLabReviewDto>
{
    public CreateLabReviewValidator()
    {
        RuleFor(x => x.LabId)
            .GreaterThan(0).WithMessage("Lab is required");
        
        RuleFor(x => x.PetOwnerId)
            .GreaterThan(0).WithMessage("Pet owner is required");
        
        RuleFor(x => x.LabAppointmentId)
            .GreaterThan(0).WithMessage("Appointment is required");
        
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");
        
        RuleFor(x => x.Comment)
            .MaximumLength(2000).WithMessage("Comment cannot exceed 2000 characters");
    }
}

public class CreateStoreReviewValidator : AbstractValidator<CreateStoreReviewDto>
{
    public CreateStoreReviewValidator()
    {
        RuleFor(x => x.StoreId)
            .GreaterThan(0).WithMessage("Store is required");
        
        RuleFor(x => x.PetOwnerId)
            .GreaterThan(0).WithMessage("Pet owner is required");
        
        RuleFor(x => x.MedicineOrderId)
            .GreaterThan(0).WithMessage("Order is required");
        
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");
        
        RuleFor(x => x.Comment)
            .MaximumLength(2000).WithMessage("Comment cannot exceed 2000 characters");
    }
}
