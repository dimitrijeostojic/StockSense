using Domain.Enums;

namespace Application.Order.GetAll;

public sealed record GetAllOrdersDto(Guid PublicId,
    DateTime OrderDate,
    OrderStatus OrderStatus,
    string SupplierName);
