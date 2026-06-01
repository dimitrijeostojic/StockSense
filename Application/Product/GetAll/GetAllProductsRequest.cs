using Domain.Core;
using MediatR;

namespace Application.Product.GetAll;

public sealed record GetAllProductsRequest(string? SearchTerm, string? SortBy, bool IsAscending, int PageNumber, int PageSize)
    : PagedRequest(PageNumber, PageSize), IRequest<TResult<GetAllProductsResponse>>;