using Domain.Core;
using MediatR;

namespace Application.OrderManagement.GetAllOrders;

public sealed class GetAllOrdersRequest
    : PagedRequest, IRequest<TResult<GetAllOrdersResponse>>;