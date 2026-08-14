// =============================================
// File: OlaVet.Application/Validators/OrderValidators.cs
// FluentValidation validators for Order DTOs
// =============================================

using FluentValidation;
using OlaVet.Application.DTOs.Order;

namespace OlaVet.Application.Validators;

public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.PetOwnerId)
            .GreaterThan(0).WithMessage("Owner is required");
        
        RuleFor(x => x.StoreId)
            .GreaterThan(0).WithMessage("Store is required");
        
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required")
            .Must(items => items.Count <= 50)
            .WithMessage("Order cannot have more than 50 items");
        
        RuleForEach(x => x.Items).SetValidator(new CreateOrderItemValidator());
        
        RuleFor(x => x.DeliveryAddress)
            .MaximumLength(500).WithMessage("Delivery address cannot exceed 500 characters");
    }
}

public class CreateOrderItemValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemValidator()
    {
        RuleFor(x => x.MedicineId)
            .GreaterThan(0).WithMessage("Medicine is required");
        
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Quantity cannot exceed 100");
    }
}

public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusDto>
{
    public UpdateOrderStatusValidator()
    {
        RuleFor(x => x.StatusId)
            .GreaterThan(0).WithMessage("Status is required");
        
        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters");
    }
}
