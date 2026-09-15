namespace Application.OrderManagement.CreateOrder;

public sealed record CreateOrderRequestBody(
    Guid SupplierPublicId,
    DateTime OrderDate,
    string? Notes,
    ICollection<OrderItemDto> OrderItemsDto);
