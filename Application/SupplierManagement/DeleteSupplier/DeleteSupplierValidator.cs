using FluentValidation;

namespace Application.SupplierManagement.DeleteSupplier;

public sealed class DeleteSupplierValidator : AbstractValidator<DeleteSupplierRequest>
{
    public DeleteSupplierValidator()
    {
        RuleFor(x => x.PublicId)
            .NotEmpty()
            .WithMessage("Supplier ID is required.");
    }
}
