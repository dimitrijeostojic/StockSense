using FluentValidation;

namespace Application.OrderManagement.GetOrderById;

public sealed class GetOrderByIdValidator : AbstractValidator<GetOrderByIdRequest>
{
    public GetOrderByIdValidator()
    {
        RuleFor(x => x.OrderPublicId)
            .NotEmpty()
            .WithMessage("Order ID is required.");
    }
}
