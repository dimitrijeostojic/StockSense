using FluentValidation;

namespace Application.ProductManagement.DeleteProduct;

public sealed class DeleteProductValidator : AbstractValidator<DeleteProductRequest>
{
    public DeleteProductValidator()
    {
        RuleFor(x => x.PublicId)
            .NotEmpty()
            .WithMessage("Product ID is required.");
    }
}
