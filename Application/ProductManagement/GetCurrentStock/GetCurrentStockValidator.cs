using FluentValidation;

namespace Application.ProductManagement.GetCurrentStock;

public sealed class GetCurrentStockValidator : AbstractValidator<GetCurrentStockRequest>
{
    public GetCurrentStockValidator()
    {
        RuleFor(x => x.ProductPublicId)
            .NotEmpty()
            .WithMessage("Product ID is required.");
    }
}
