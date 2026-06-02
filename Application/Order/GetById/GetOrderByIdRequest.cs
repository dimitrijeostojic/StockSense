using Domain.Core;
using MediatR;

namespace Application.Order.GetById;

public sealed record GetOrderByIdRequest(Guid OrderPublicId)
    : IRequest<TResult<GetOrderByIdResponse>>;
