using Domain.Enums;

namespace Application.DashboardManagement.Get;

public sealed record DashboardOrderDto(
    Guid PublicId,
    string SupplierName,
    DateTime OrderDate,
    OrderStatus OrderStatus);
