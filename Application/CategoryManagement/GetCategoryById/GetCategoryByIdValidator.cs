using FluentValidation;

namespace Application.CategoryManagement.GetCategoryById;

public sealed class GetCategoryByIdValidator : AbstractValidator<GetCategoryByIdRequest>
{
    public GetCategoryByIdValidator()
    {
        RuleFor(x => x.CategoryPublicId)
            .NotEmpty()
            .WithMessage("Category ID is required.");
    }
}
