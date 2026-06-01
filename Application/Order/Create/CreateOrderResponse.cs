using Domain.Enums;

namespace Application.Order.Create;

public sealed record CreateOrderResponse(
    Guid PublicId,
    OrderStatus Status,
    DateTime OrderDate,
    string? Notes,
    Guid SupplierPublicId);
