using Domain.Core;
using MediatR;

namespace Application.Order.GetAll;

public sealed record GetAllOrdersRequest(int PageNumber, int PageSize)
    : PagedRequest(PageNumber, PageSize), IRequest<TResult<GetAllOrdersResponse>>;