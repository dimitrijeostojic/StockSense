using FluentValidation;

namespace Application.ProductManagement.GetProductById;

public sealed class GetProductByIdValidator : AbstractValidator<GetProductByIdRequest>
{
    public GetProductByIdValidator()
    {
        RuleFor(x => x.ProductPublicId)
            .NotEmpty()
            .WithMessage("Product ID is required.");
    }
}
