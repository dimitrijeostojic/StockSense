using Domain.Enums;

namespace Application.OrderManagement.GetAllOrders;

public sealed record GetAllOrdersDto(Guid PublicId,
    DateTime OrderDate,
    OrderStatus OrderStatus,
    string SupplierName);
