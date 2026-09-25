using Application.Abstractions.Services;
using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.Entities;
using Domain.Enums;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.GoodsReceiptManagement.CreateGoodsReceipt;

internal sealed class CreateGoodsReceiptRequestHandler(
    IOrderRepository orderRepository,
    IGoodsReceiptRepository goodsReceiptRepository,
    ICurrentUserAccessor currentUserAccessor,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateGoodsReceiptRequest, TResult<CreateGoodsReceiptResponse>>
{
    private readonly IOrderRepository _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    private readonly IGoodsReceiptRepository _goodsReceiptRepository = goodsReceiptRepository ?? throw new ArgumentNullException(nameof(goodsReceiptRepository));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<TResult<CreateGoodsReceiptResponse>> Handle(CreateGoodsReceiptRequest request, CancellationToken cancellationToken)
    {
        var tenantPublicId = _currentUserAccessor.TenantPublicId;

        var order = await _orderRepository.GetByPublicIdAsync(request.OrderPublicId, tenantPublicId, cancellationToken);
        if (order is null)
        {
            return TResult<CreateGoodsReceiptResponse>.Failure(ApplicationErrors.NotFound);
        }

        if (order.OrderStatus != OrderStatus.Confirmed)
        {
            return TResult<CreateGoodsReceiptResponse>.Failure(ApplicationErrors.OrderNotConfirmed);
        }

        if (await _goodsReceiptRepository.ExistsForOrderAsync(request.OrderPublicId, tenantPublicId, cancellationToken))
        {
            return TResult<CreateGoodsReceiptResponse>.Failure(ApplicationErrors.GoodsReceiptAlreadyExists);
        }

        var receipt = GoodsReceipt.Create(order.Id, order.PublicId, tenantPublicId, request.Notes);

        foreach (var itemDto in request.Items)
        {
            var orderItem = order.OrderItems.FirstOrDefault(oi => oi.PublicId == itemDto.OrderItemPublicId);
            if (orderItem is null)
            {
                return TResult<CreateGoodsReceiptResponse>.Failure(ApplicationErrors.NotFound);
            }

            if (itemDto.ReceivedQuantity > orderItem.Quantity)
            {
                return TResult<CreateGoodsReceiptResponse>.Failure(ApplicationErrors.ReceivedQuantityExceedsOrdered);
            }

            receipt.AddItem(orderItem.Id, orderItem.ProductId, orderItem.Quantity, itemDto.ReceivedQuantity);
        }

        await _goodsReceiptRepository.AddAsync(receipt, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return TResult<CreateGoodsReceiptResponse>.Success(new CreateGoodsReceiptResponse(receipt.PublicId, order.PublicId, receipt.ReceivedAt));
    }
}
