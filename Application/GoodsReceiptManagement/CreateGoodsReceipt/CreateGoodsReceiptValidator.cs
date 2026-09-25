using FluentValidation;

namespace Application.GoodsReceiptManagement.CreateGoodsReceipt;

public sealed class CreateGoodsReceiptValidator : AbstractValidator<CreateGoodsReceiptRequest>
{
    public CreateGoodsReceiptValidator()
    {
        RuleFor(x => x.OrderPublicId)
            .NotEmpty()
            .WithMessage("Order ID is required.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes must not exceed 500 characters.");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item is required.");

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(i => i.OrderItemPublicId)
                    .NotEmpty()
                    .WithMessage("Order item ID is required.");
                item.RuleFor(i => i.ReceivedQuantity)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Received quantity must be 0 or greater.");
            });
    }
}
