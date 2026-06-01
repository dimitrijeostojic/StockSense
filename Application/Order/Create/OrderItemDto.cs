namespace Application.Order.Create;

public sealed record OrderItemDto(
    Guid ProductPublicId,
    int Quantity,
    decimal UnitPrice);
