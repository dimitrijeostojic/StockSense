using FluentValidation;

namespace Application.OrderManagement.CreateOrder;

public sealed class CreateOrderValidator
    : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.SupplierPublicId)
            .NotEmpty()
            .WithMessage("Supplier ID is required.");
        RuleFor(x => x.OrderDate)
            .NotEmpty()
            .WithMessage("Order date is required.");
        RuleFor(x => x.Notes)
            .MaximumLength(255)
            .WithMessage("Notes must not exceed 255 characters.");
        RuleForEach(x => x.OrderItemsDto)
              .ChildRules(item =>
              {
                  item.RuleFor(i => i.ProductPublicId)
                      .NotEmpty()
                      .WithMessage("Product ID is required.");
                  item.RuleFor(i => i.Quantity)
                      .GreaterThan(0)
                      .WithMessage("Quantity must be greater than 0.");
              });

    }
}
