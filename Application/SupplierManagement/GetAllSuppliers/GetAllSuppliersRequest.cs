using Domain.Core;
using MediatR;

namespace Application.SupplierManagement.GetAllSuppliers;

public sealed record GetAllSuppliersRequest(string? SearchTerm, string? SortBy, bool IsAscending, int PageNumber, int PageSize)
    : PagedRequest(PageNumber, PageSize), IRequest<TResult<GetAllSuppliersResponse>>;