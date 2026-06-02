using Domain.Core;
using MediatR;

namespace Application.ProductManagement.GetAllProducts;

public sealed record GetAllProductsRequest(string? SearchTerm, string? SortBy, bool IsAscending, int PageNumber, int PageSize)
    : PagedRequest(PageNumber, PageSize), IRequest<TResult<GetAllProductsResponse>>;