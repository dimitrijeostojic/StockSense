using Domain.Core;

namespace Application.Order.GetAll;

public sealed class GetAllOrdersResponse(
IEnumerable<GetAllOrdersDto> items,
    int totalCount,
    int pageNumber,
    int pageSize
    ) : PagedResponse<GetAllOrdersDto>(items, totalCount, pageNumber, pageSize);