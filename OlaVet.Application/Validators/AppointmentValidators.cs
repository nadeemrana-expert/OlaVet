// =============================================
// File: OlaVet.Application/Validators/AppointmentValidators.cs
// FluentValidation validators for Appointment DTOs
// =============================================

using FluentValidation;
using OlaVet.Application.DTOs.Appointment;

namespace OlaVet.Application.Validators;

public class CreateVetAppointmentValidator : AbstractValidator<CreateVetAppointmentDto>
{
    public CreateVetAppointmentValidator()
    {
        RuleFor(x => x.PetId)
            .GreaterThan(0).WithMessage("Pet is required");
        
        RuleFor(x => x.VetId)
            .GreaterThan(0).WithMessage("Vet is required");
        
        RuleFor(x => x.AppointmentDate)
            .GreaterThan(DateTime.UtcNow.AddMinutes(-5))
            .WithMessage("Appointment date must be in the future");
        
        RuleFor(x => x.VetAppointmentTypeId)
            .GreaterThan(0).WithMessage("Appointment type is required");
        
        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters");
    }
}

public class CreateLabAppointmentValidator : AbstractValidator<CreateLabAppointmentDto>
{
    public CreateLabAppointmentValidator()
    {
        RuleFor(x => x.PetId)
            .GreaterThan(0).WithMessage("Pet is required");
        
        RuleFor(x => x.LabId)
            .GreaterThan(0).WithMessage("Lab is required");
        
        RuleFor(x => x.AppointmentDate)
            .GreaterThan(DateTime.UtcNow.AddMinutes(-5))
            .WithMessage("Appointment date must be in the future");
        
        RuleFor(x => x.TestIds)
            .NotEmpty().WithMessage("At least one test is required")
            .Must(ids => ids.All(id => id > 0))
            .WithMessage("Invalid test ID");
        
        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters");
    }
}

public class UpdateAppointmentStatusValidator : AbstractValidator<UpdateAppointmentStatusDto>
{
    public UpdateAppointmentStatusValidator()
    {
        RuleFor(x => x.StatusId)
            .GreaterThan(0).WithMessage("Status is required");
        
        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters");
    }
}

public class SearchAppointmentsValidator : AbstractValidator<SearchAppointmentsDto>
{
    public SearchAppointmentsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");
        
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");
        
        RuleFor(x => x)
            .Must(x => !x.FromDate.HasValue || !x.ToDate.HasValue || x.FromDate <= x.ToDate)
            .WithMessage("From date must be before or equal to To date");
    }
}
