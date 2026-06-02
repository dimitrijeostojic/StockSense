using Domain.Enums;

namespace Application.OrderManagement.UpdateOrder;

public sealed record UpdateOrderResponse(Guid PublicId,
    DateTime OrderDate,
    OrderStatus OrderStatus,
    string? Notes,
    Guid SupplierPublicId,
    string SupplierName);
