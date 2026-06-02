using Domain.Core;

namespace Application.ProductManagement.GetAllProducts;

public sealed class GetAllProductsResponse(IEnumerable<GetAllProductsDto> items,
    int totalCount,
    int pageNumber,
    int pageSize)
    : PagedResponse<GetAllProductsDto>(items, totalCount, pageNumber, pageSize);