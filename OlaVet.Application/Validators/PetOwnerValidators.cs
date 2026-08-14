// =============================================
// File: OlaVet.Application/Validators/PetOwnerValidators.cs
// FluentValidation validators for PetOwner DTOs
// =============================================

using FluentValidation;
using OlaVet.Application.DTOs.PetOwner;

namespace OlaVet.Application.Validators;

public class CreatePetOwnerValidator : AbstractValidator<CreatePetOwnerDto>
{
    public CreatePetOwnerValidator()
    {
        RuleFor(x => x.OwnerName)
            .NotEmpty().WithMessage("Owner name is required")
            .MaximumLength(100).WithMessage("Owner name cannot exceed 100 characters");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");
        
        RuleFor(x => x.ContactNumber)
            .NotEmpty().WithMessage("Contact number is required")
            .Matches(@"^\+?[\d\-\s]+$").WithMessage("Invalid phone number format")
            .MaximumLength(20).WithMessage("Contact number cannot exceed 20 characters");
        
        RuleFor(x => x.HomeAddress)
            .MaximumLength(500).WithMessage("Address cannot exceed 500 characters");
        
        RuleFor(x => x.Age)
            .InclusiveBetween(18, 120).When(x => x.Age.HasValue)
            .WithMessage("Age must be between 18 and 120");
        
        RuleFor(x => x.Gender)
            .Must(g => g == null || new[] { "Male", "Female", "Other" }.Contains(g))
            .WithMessage("Gender must be Male, Female, or Other");
        
        RuleFor(x => x.InitialWalletBalance)
            .GreaterThanOrEqualTo(0).WithMessage("Initial balance cannot be negative");
    }
}

public class UpdatePetOwnerValidator : AbstractValidator<UpdatePetOwnerDto>
{
    public UpdatePetOwnerValidator()
    {
        RuleFor(x => x.OwnerName)
            .NotEmpty().WithMessage("Owner name is required")
            .MaximumLength(100).WithMessage("Owner name cannot exceed 100 characters");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");
        
        RuleFor(x => x.ContactNumber)
            .NotEmpty().WithMessage("Contact number is required")
            .Matches(@"^\+?[\d\-\s]+$").WithMessage("Invalid phone number format")
            .MaximumLength(20).WithMessage("Contact number cannot exceed 20 characters");
        
        RuleFor(x => x.Age)
            .InclusiveBetween(18, 120).When(x => x.Age.HasValue)
            .WithMessage("Age must be between 18 and 120");
    }
}

public class AddFundsValidator : AbstractValidator<AddFundsDto>
{
    public AddFundsValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0")
            .LessThanOrEqualTo(1000000).WithMessage("Amount cannot exceed 1,000,000");
    }
}
