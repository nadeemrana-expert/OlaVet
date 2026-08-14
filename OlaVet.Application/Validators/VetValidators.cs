// =============================================
// File: OlaVet.Application/Validators/VetValidators.cs
// FluentValidation validators for Vet DTOs
// =============================================

using FluentValidation;
using OlaVet.Application.DTOs.Vet;

namespace OlaVet.Application.Validators;

public class CreateVetValidator : AbstractValidator<CreateVetDto>
{
    public CreateVetValidator()
    {
        RuleFor(x => x.VetName)
            .NotEmpty().WithMessage("Vet name is required")
            .MaximumLength(100).WithMessage("Vet name cannot exceed 100 characters");
        
        RuleFor(x => x.Specialization)
            .NotEmpty().WithMessage("Specialization is required")
            .MaximumLength(100).WithMessage("Specialization cannot exceed 100 characters");
        
        RuleFor(x => x.ClinicLocation)
            .NotEmpty().WithMessage("Clinic location is required")
            .MaximumLength(500).WithMessage("Clinic location cannot exceed 500 characters");
        
        RuleFor(x => x.Fee)
            .GreaterThan(0).WithMessage("Fee must be greater than 0")
            .LessThanOrEqualTo(100000).WithMessage("Fee cannot exceed 100,000");
        
        RuleFor(x => x.ContactNumber)
            .NotEmpty().WithMessage("Contact number is required")
            .Matches(@"^\+?[\d\-\s]+$").WithMessage("Invalid phone number format");
        
        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email format");
        
        RuleFor(x => x.YearsOfExperience)
            .InclusiveBetween(0, 60).When(x => x.YearsOfExperience.HasValue)
            .WithMessage("Years of experience must be between 0 and 60");
        
        RuleFor(x => x.LicenseNumber)
            .MaximumLength(50).WithMessage("License number cannot exceed 50 characters");
    }
}

public class UpdateVetValidator : AbstractValidator<UpdateVetDto>
{
    public UpdateVetValidator()
    {
        RuleFor(x => x.VetName)
            .NotEmpty().WithMessage("Vet name is required")
            .MaximumLength(100).WithMessage("Vet name cannot exceed 100 characters");
        
        RuleFor(x => x.Specialization)
            .NotEmpty().WithMessage("Specialization is required")
            .MaximumLength(100).WithMessage("Specialization cannot exceed 100 characters");
        
        RuleFor(x => x.ClinicLocation)
            .NotEmpty().WithMessage("Clinic location is required")
            .MaximumLength(500).WithMessage("Clinic location cannot exceed 500 characters");
        
        RuleFor(x => x.Fee)
            .GreaterThan(0).WithMessage("Fee must be greater than 0")
            .LessThanOrEqualTo(100000).WithMessage("Fee cannot exceed 100,000");
        
        RuleFor(x => x.ContactNumber)
            .NotEmpty().WithMessage("Contact number is required")
            .Matches(@"^\+?[\d\-\s]+$").WithMessage("Invalid phone number format");
        
        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email format");
    }
}

public class SearchVetsValidator : AbstractValidator<SearchVetsDto>
{
    public SearchVetsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");
        
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");
        
        RuleFor(x => x.MaxFee)
            .GreaterThan(0).When(x => x.MaxFee.HasValue)
            .WithMessage("Max fee must be greater than 0");
        
        RuleFor(x => x.MinRating)
            .InclusiveBetween(1, 5).When(x => x.MinRating.HasValue)
            .WithMessage("Min rating must be between 1 and 5");
    }
}
