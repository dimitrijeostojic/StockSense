using Domain.Enums;

namespace Application.Order.GetById;

public sealed record GetOrderByIdResponse(
    Guid PublicId,
    DateTime OrderDate,
    OrderStatus OrderStatus,
    string? Notes,
    Guid SupplierPublicId,
    string SupplierName,
    IEnumerable<OrderItemDto> OrderItems);
