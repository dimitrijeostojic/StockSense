using FluentValidation;

namespace Application.ProductManagement.CreateStockEntry;

public sealed class CreateStockEntryValidator
    : AbstractValidator<CreateStockEntryRequest>
{
    public CreateStockEntryValidator()
    {
        RuleFor(x => x.ProductPublicId).NotEmpty().WithMessage("ProductPublicId is required.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        RuleFor(x => x.Notes).MaximumLength(255).WithMessage("Notes cannot exceed 255 characters.");
        RuleFor(x => x.StockEntryType).IsInEnum().WithMessage("StockEntryType must be a valid enum value.");

    }
}

