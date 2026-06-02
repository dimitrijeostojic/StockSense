using FluentValidation;

namespace Application.ProductManagement.UpdateProduct;

public sealed class UpdateProductValidator
    : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.ProductPublicId)
            .NotEmpty().WithMessage("ProductPublicId is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(255).WithMessage("Description must not exceed 255 characters.");

        RuleFor(x => x.Price)
                        .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.MinimumStockQuantity)
                        .GreaterThanOrEqualTo(0).WithMessage("MinimumStockQuantity must be greater than or equal to 0.");

        RuleFor(x => x.CategoryId)
                        .NotEmpty().WithMessage("CategoryId is required.");

        RuleFor(x => x.SupplierId)
                        .NotEmpty().WithMessage("SupplierId is required.");
    }
}
