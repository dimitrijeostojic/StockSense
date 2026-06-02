using Domain.Core;
using MediatR;

namespace Application.OrderManagement.GetOrderById;

public sealed record GetOrderByIdRequest(Guid OrderPublicId)
    : IRequest<TResult<GetOrderByIdResponse>>;
