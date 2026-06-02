using FluentValidation;

namespace Application.Order.Update;

public sealed class UpdateOrderValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderValidator()
    {
        RuleFor(x => x.SupplierPublicId)
            .NotEmpty().WithMessage("SupplierPublicId is required.");
        RuleFor(x => x.OrderDate)
            .NotEmpty().WithMessage("OrderDate is required.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("OrderDate cannot be in the future.");
        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
        RuleFor(x => x.PublicId)
            .NotEmpty().WithMessage("PublicId is required.");
    }
}
