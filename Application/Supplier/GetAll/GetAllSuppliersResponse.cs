using Domain.Core;

namespace Application.Supplier.GetAll;

public sealed class GetAllSuppliersResponse(IEnumerable<GetAllSuppliersDto> items,
    int totalCount,
    int pageNumber,
    int pageSize)
    : PagedResponse<GetAllSuppliersDto>(items, totalCount, pageNumber, pageSize);