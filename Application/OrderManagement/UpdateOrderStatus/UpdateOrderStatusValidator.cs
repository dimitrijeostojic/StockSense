using Domain.Enums;
using FluentValidation;

namespace Application.OrderManagement.UpdateOrderStatus;

public sealed class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusValidator()
    {
        RuleFor(x => x.OrderPublicId)
            .NotEmpty()
            .WithMessage("Order ID is required.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage($"Status must be one of: {string.Join(", ", Enum.GetNames<OrderStatus>())}.");
    }
}
