namespace Application.OrderManagement.CreateOrder;

public sealed record OrderItemDto(
    Guid ProductPublicId,
    int Quantity);
