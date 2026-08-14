// =============================================
// File: OlaVet.Application/Validators/PetValidators.cs
// FluentValidation validators for Pet DTOs
// =============================================

using FluentValidation;
using OlaVet.Application.DTOs.Pet;

namespace OlaVet.Application.Validators;

public class CreatePetValidator : AbstractValidator<CreatePetDto>
{
    public CreatePetValidator()
    {
        RuleFor(x => x.PetOwnerId)
            .GreaterThan(0).WithMessage("Owner ID is required");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Pet name is required")
            .MaximumLength(100).WithMessage("Pet name cannot exceed 100 characters");
        
        RuleFor(x => x.Species)
            .NotEmpty().WithMessage("Species is required")
            .MaximumLength(50).WithMessage("Species cannot exceed 50 characters");
        
        RuleFor(x => x.Breed)
            .MaximumLength(100).WithMessage("Breed cannot exceed 100 characters");
        
        RuleFor(x => x.Age)
            .InclusiveBetween(0, 50).When(x => x.Age.HasValue)
            .WithMessage("Age must be between 0 and 50");
        
        RuleFor(x => x.PetWeight)
            .GreaterThan(0).When(x => x.PetWeight.HasValue)
            .WithMessage("Weight must be greater than 0")
            .LessThanOrEqualTo(500).When(x => x.PetWeight.HasValue)
            .WithMessage("Weight cannot exceed 500 kg");
        
        RuleFor(x => x.Color)
            .MaximumLength(50).WithMessage("Color cannot exceed 50 characters");
        
        RuleFor(x => x.Gender)
            .Must(g => g == null || new[] { "Male", "Female" }.Contains(g))
            .WithMessage("Gender must be Male or Female");
    }
}

public class UpdatePetValidator : AbstractValidator<UpdatePetDto>
{
    public UpdatePetValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Pet name is required")
            .MaximumLength(100).WithMessage("Pet name cannot exceed 100 characters");
        
        RuleFor(x => x.Species)
            .NotEmpty().WithMessage("Species is required")
            .MaximumLength(50).WithMessage("Species cannot exceed 50 characters");
        
        RuleFor(x => x.Breed)
            .MaximumLength(100).WithMessage("Breed cannot exceed 100 characters");
        
        RuleFor(x => x.Age)
            .InclusiveBetween(0, 50).When(x => x.Age.HasValue)
            .WithMessage("Age must be between 0 and 50");
        
        RuleFor(x => x.PetWeight)
            .GreaterThan(0).When(x => x.PetWeight.HasValue)
            .WithMessage("Weight must be greater than 0")
            .LessThanOrEqualTo(500).When(x => x.PetWeight.HasValue)
            .WithMessage("Weight cannot exceed 500 kg");
    }
}

public class SearchPetsValidator : AbstractValidator<SearchPetsDto>
{
    public SearchPetsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");
        
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");
    }
}
