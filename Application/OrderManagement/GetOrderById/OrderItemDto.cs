namespace Application.OrderManagement.GetOrderById;

public sealed record OrderItemDto(
    Guid OrderItemPublicId,
    Guid ProductPublicId,
    string ProductName,
    int Quantity,
    decimal UnitPrice);
