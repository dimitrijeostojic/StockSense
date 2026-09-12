using FluentValidation;

namespace Application.TenantManagement.Update;

public sealed class UpdateTenantRequestValidator : AbstractValidator<UpdateTenantRequest>
{
    public UpdateTenantRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(255).WithMessage("Name must not exceed 255 characters.");
        RuleFor(x => x.Address)
            .MaximumLength(255).WithMessage("Address must not exceed 255 characters.");
    }
}
