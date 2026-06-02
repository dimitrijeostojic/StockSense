using Domain.Enums;

namespace Application.OrderManagement.GetOrderById;

public sealed record GetOrderByIdResponse(
    Guid PublicId,
    DateTime OrderDate,
    OrderStatus OrderStatus,
    string? Notes,
    Guid SupplierPublicId,
    string SupplierName,
    IEnumerable<OrderItemDto> OrderItems);
