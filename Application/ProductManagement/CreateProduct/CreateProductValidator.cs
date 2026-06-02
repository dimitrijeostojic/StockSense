using FluentValidation;

namespace Application.ProductManagement.CreateProduct;

public sealed class CreateProductValidator
    : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .MaximumLength(100)
            .WithMessage("Product name must not exceed 100 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(255)
            .WithMessage("Product description must not exceed 255 characters.");
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero.");
        RuleFor(x => x.MinimumStockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Minimum stock quantity must be zero or greater.");
        RuleFor(x => x.CategoryPublicId)
            .NotEmpty()
            .WithMessage("Category public ID is required.");
        RuleFor(x => x.SupplierPublicId)
            .NotEmpty()
            .WithMessage("Supplier public ID is required.");
    }
}

