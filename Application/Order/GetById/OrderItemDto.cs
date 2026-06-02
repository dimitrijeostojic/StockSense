namespace Application.Order.GetById;

public sealed record OrderItemDto(
    Guid ProductPublicId,
    string ProductName,
    int Quantity,
    decimal UnitPrice);
