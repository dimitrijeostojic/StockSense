using FluentValidation;

namespace Application.OrderManagement.CreateOrder;

public sealed class CreateOrderValidator
    : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.SupplierPublicId)
            .NotEmpty()
            .WithMessage("Supplier ID is required.");
        RuleFor(x => x.OrderDate)
            .NotEmpty()
            .WithMessage("Order date is required.");
        RuleFor(x => x.Notes)
            .MaximumLength(255)
            .WithMessage("Notes must not exceed 255 characters.");
        RuleFor(x => x.OrderItemsDto)
            .NotEmpty()
            .WithMessage("At least one order item is required.");

    }
}
