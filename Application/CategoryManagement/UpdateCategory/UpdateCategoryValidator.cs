using FluentValidation;

namespace Application.CategoryManagement.UpdateCategory;

public sealed class UpdateCategoryValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.CategoryPublicId)
            .NotEmpty()
            .WithMessage("Category ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MaximumLength(100)
            .WithMessage("Category name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(255)
            .WithMessage("Description must not exceed 255 characters.");
    }
}
