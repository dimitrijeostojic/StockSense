using Domain.Enums;

namespace Application.OrderManagement.CreateOrder;

public sealed record CreateOrderResponse(
    Guid PublicId,
    OrderStatus Status,
    DateTime OrderDate,
    string? Notes,
    Guid SupplierPublicId,
    string SupplierName);