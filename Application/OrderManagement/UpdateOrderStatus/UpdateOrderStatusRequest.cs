using Domain.Core;
using Domain.Enums;
using MediatR;

namespace Application.OrderManagement.UpdateOrderStatus;

public sealed record UpdateOrderStatusRequest(
    Guid OrderPublicId,
    OrderStatus Status) : IRequest<TResult<UpdateOrderStatusResponse>>;
