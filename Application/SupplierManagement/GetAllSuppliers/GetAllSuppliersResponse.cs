using Domain.Core;

namespace Application.SupplierManagement.GetAllSuppliers;

public sealed class GetAllSuppliersResponse(IEnumerable<GetAllSuppliersDto> items,
    int totalCount,
    int pageNumber,
    int pageSize)
    : PagedResponse<GetAllSuppliersDto>(items, totalCount, pageNumber, pageSize);