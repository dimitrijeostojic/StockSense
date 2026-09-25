using Application.Abstractions.Services;
using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.Enums;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.OrderManagement.UpdateOrderStatus;

internal sealed class UpdateOrderStatusRequestHandler(
    IOrderRepository orderRepository,
    IGoodsReceiptRepository goodsReceiptRepository,
    ICurrentUserAccessor currentUserAccessor,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateOrderStatusRequest, TResult<UpdateOrderStatusResponse>>
{
    private readonly IOrderRepository _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    private readonly IGoodsReceiptRepository _goodsReceiptRepository = goodsReceiptRepository ?? throw new ArgumentNullException(nameof(goodsReceiptRepository));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<TResult<UpdateOrderStatusResponse>> Handle(UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        var tenantPublicId = _currentUserAccessor.TenantPublicId;

        var order = await _orderRepository.GetByPublicIdAsync(request.OrderPublicId, tenantPublicId, cancellationToken);
        if (order is null)
            return TResult<UpdateOrderStatusResponse>.Failure(ApplicationErrors.NotFound);

        if (request.Status == OrderStatus.Received)
        {
            var receiptExists = await _goodsReceiptRepository.ExistsForOrderAsync(request.OrderPublicId, tenantPublicId, cancellationToken);
            if (!receiptExists)
                return TResult<UpdateOrderStatusResponse>.Failure(ApplicationErrors.GoodsReceiptRequired);
        }

        var orderResult = order.WithOrderStatus(request.Status, tenantPublicId);
        if (!orderResult.IsSuccess)
            return TResult<UpdateOrderStatusResponse>.Failure(ApplicationErrors.InvalidOrderStatusTransition);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return TResult<UpdateOrderStatusResponse>.Success(new UpdateOrderStatusResponse(order.PublicId, order.OrderStatus));
    }
}
