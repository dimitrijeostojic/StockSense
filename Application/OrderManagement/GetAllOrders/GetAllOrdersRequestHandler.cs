using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.OrderManagement.GetAllOrders;

internal sealed class GetAllOrdersRequestHandler(
    IOrderRepository orderRepository)
    : IRequestHandler<GetAllOrdersRequest, TResult<GetAllOrdersResponse>>
{
    private readonly IOrderRepository _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));

    public async Task<TResult<GetAllOrdersResponse>> Handle(GetAllOrdersRequest request, CancellationToken cancellationToken)
    {
        var (orders, totalCount) = await _orderRepository.GetAllAsync(request.SearchTerm, request.SortBy, request.IsAscending, request.PageNumber, request.PageSize, cancellationToken);

        var orderDtos = orders.Select(order => new GetAllOrdersDto(
            order.PublicId,
            order.OrderDate,
            order.OrderStatus,
            order.Supplier!.Name)).ToList();

        var response = new GetAllOrdersResponse(orderDtos, totalCount, request.PageNumber, request.PageSize);
        return TResult<GetAllOrdersResponse>.Success(response);
    }
}
