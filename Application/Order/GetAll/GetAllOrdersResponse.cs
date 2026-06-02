using Domain.Core;

namespace Application.Order.GetAll;

public sealed class GetAllOrdersResponse(
IEnumerable<OrderDto> items,
    int totalCount,
    int pageNumber,
    int pageSize
    ) : PagedResponse<OrderDto>(items, totalCount, pageNumber, pageSize);