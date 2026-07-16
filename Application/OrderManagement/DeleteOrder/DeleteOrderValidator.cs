using FluentValidation;

namespace Application.OrderManagement.DeleteOrder;

public sealed class DeleteOrderValidator : AbstractValidator<DeleteOrderRequest>
{
    public DeleteOrderValidator()
    {
        RuleFor(x => x.OrderPublicId)
            .NotEmpty()
            .WithMessage("Order ID is required.");
    }
}
