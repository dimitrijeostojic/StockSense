using FluentValidation;

namespace Application.SupplierManagement.GetSupplierById;

public sealed class GetSupplierByIdValidator : AbstractValidator<GetSupplierByIdRequest>
{
    public GetSupplierByIdValidator()
    {
        RuleFor(x => x.SupplierPublicId)
            .NotEmpty()
            .WithMessage("Supplier ID is required.");
    }
}
