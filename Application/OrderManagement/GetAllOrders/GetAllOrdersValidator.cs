using FluentValidation;

namespace Application.OrderManagement.GetAllOrders;

public sealed class GetAllOrdersValidator : AbstractValidator<GetAllOrdersRequest>
{
    public GetAllOrdersValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page size must be at least 1.")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must not exceed 100.");
    }
}
