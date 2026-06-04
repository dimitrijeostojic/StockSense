using Domain.Enums;

namespace Application.OrderManagement.UpdateOrderStatus;

public sealed record UpdateOrderStatusRequestBody(
    OrderStatus Status);