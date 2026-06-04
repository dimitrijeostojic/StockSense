using Domain.Enums;

namespace Application.OrderManagement.UpdateOrderStatus;

public sealed record UpdateOrderStatusResponse(Guid OrderId, OrderStatus NewStatus);
