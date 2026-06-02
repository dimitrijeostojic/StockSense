using Domain.Core;
using MediatR;

namespace Application.OrderManagement.DeleteOrder;

public sealed record DeleteOrderRequest(Guid OrderPublicId)
    : IRequest<Result>;
