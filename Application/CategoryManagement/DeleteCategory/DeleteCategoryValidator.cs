using FluentValidation;

namespace Application.CategoryManagement.DeleteCategory;

public sealed class DeleteCategoryValidator : AbstractValidator<DeleteCategoryRequest>
{
    public DeleteCategoryValidator()
    {
        RuleFor(x => x.CategoryPublicId)
            .NotEmpty()
            .WithMessage("Category ID is required.");
    }
}
