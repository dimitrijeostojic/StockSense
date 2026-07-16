using FluentValidation;

namespace Application.ProductManagement.GetStockEntryByProductId;

public sealed class GetStockEntryByIdValidator : AbstractValidator<GetStockEntryByIdRequest>
{
    public GetStockEntryByIdValidator()
    {
        RuleFor(x => x.ProductPublicId)
            .NotEmpty()
            .WithMessage("Product ID is required.");

        RuleFor(x => x.StockEntryPublicId)
            .NotEmpty()
            .WithMessage("Stock entry ID is required.");
    }
}
