using FluentValidation;

namespace Application.ProductManagement.GetAllStockEntries;

public sealed class GetAllStockEntriesValidator : AbstractValidator<GetAllStockEntriesRequest>
{
    public GetAllStockEntriesValidator()
    {
        RuleFor(x => x.ProductPublicId)
            .NotEmpty()
            .WithMessage("Product ID is required.");
    }
}
