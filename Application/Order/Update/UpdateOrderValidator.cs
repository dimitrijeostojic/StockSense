using FluentValidation;

namespace Application.Order.Update;

public sealed class UpdateOrderValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderValidator()
    {
    }
}
