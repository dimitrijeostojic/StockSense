using Domain.Enums;

namespace Application.Order.GetAll;

public sealed record OrderDto(Guid PublicId,
    DateTime OrderDate,
    OrderStatus OrderStatus,
    string SupplierName);
