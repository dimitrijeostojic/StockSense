using Domain.Core;
using MediatR;

namespace Application.Order.GetAll;

public sealed record GetAllOrdersRequest(string? SearchTerm, string? SortBy, bool IsAscending, int PageNumber, int PageSize)
    : PagedRequest(PageNumber, PageSize), IRequest<TResult<GetAllOrdersResponse>>;