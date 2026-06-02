using Domain.Core;
using MediatR;

namespace Application.Order.Delete;

public sealed record DeleteOrderRequest(Guid OrderPublicId)
    : IRequest<Result>;
