using FluentValidation;

namespace Application.SupplierManagement.CreateSupplier;

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
            .NotEmpty()
            .WithMessage("Contact name is required.")
            .MaximumLength(255)
            .WithMessage("Supplier contact name must not exceed 255 characters.");
        RuleFor(x => x.ContactEmail)
            .NotEmpty()
            .WithMessage("Contact email is required.")
            .MaximumLength(255)
            .WithMessage("Supplier contact email must not exceed 255 characters.");
        RuleFor(x => x.SupplierCode)
            .NotEmpty()
            .WithMessage("Supplier code is required.")
            .MaximumLength(20)
            .WithMessage("Supplier code must not exceed 20 characters.");
        RuleFor(x => x.ContactPhone)
            .MaximumLength(12)
            .WithMessage("Supplier contact phone must not exceed 12 characters.");
    }
}

