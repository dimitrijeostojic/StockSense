using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.Order.GetById;

internal sealed class GetOrderByIdRequestHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<GetOrderByIdRequest, TResult<GetOrderByIdResponse>>
{
    private readonly IOrderRepository _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<TResult<GetOrderByIdResponse>> Handle(GetOrderByIdRequest request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByPublicIdAsync(request.OrderPublicId, cancellationToken);
        if (order == null)
        {
            return TResult<GetOrderByIdResponse>.Failure(ApplicationErrors.NotFound);
        }

        var orderItemsDto = order.OrderItems.Select(oi => new OrderItemDto(oi.Product!.PublicId, oi.Product.Name, oi.Quantity, oi.UnitPrice)).ToList();

        var response = new GetOrderByIdResponse(order.PublicId, order.OrderDate, order.OrderStatus, order.Notes, order.Supplier!.PublicId, order.Supplier.Name, orderItemsDto);
        return TResult<GetOrderByIdResponse>.Success(response);
    }
}
