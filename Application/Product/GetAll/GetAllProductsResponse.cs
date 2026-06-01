using Domain.Core;

namespace Application.Product.GetAll;

public sealed class GetAllProductsResponse(IEnumerable<GetAllProductsDto> items,
    int totalCount,
    int pageNumber,
    int pageSize)
    : PagedResponse<GetAllProductsDto>(items, totalCount, pageNumber, pageSize);