using FluentValidation;

namespace Application.Supplier.Create;

public sealed class CreateSupplierValidator
    : AbstractValidator<CreateSupplierRequest>
{
    public CreateSupplierValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Supplier name is required.")
            .MaximumLength(100)
            .WithMessage("Supplier name must not exceed 100 characters.");
        RuleFor(x => x.ContactName)
            .MaximumLength(255)
            .WithMessage("Supplier contact name must not exceed 255 characters.");
        RuleFor(x => x.ContactEmail)
            .MaximumLength(255)
            .WithMessage("Supplier contact email must not exceed 255 characters.");
        RuleFor(x => x.ContactPhone)
            .MaximumLength(20)
            .WithMessage("Supplier contact phone must not exceed 20 characters.");
    }
}

