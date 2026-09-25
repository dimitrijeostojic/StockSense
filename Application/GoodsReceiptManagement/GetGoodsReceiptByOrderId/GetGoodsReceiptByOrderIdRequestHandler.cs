using Application.Abstractions.Services;
using Application.Common.Errors;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.GoodsReceiptManagement.GetGoodsReceiptByOrderId;

internal sealed class GetGoodsReceiptByOrderIdRequestHandler(
    IGoodsReceiptRepository goodsReceiptRepository,
    ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<GetGoodsReceiptByOrderIdRequest, TResult<GetGoodsReceiptByOrderIdResponse>>
{
    private readonly IGoodsReceiptRepository _goodsReceiptRepository = goodsReceiptRepository ?? throw new ArgumentNullException(nameof(goodsReceiptRepository));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<TResult<GetGoodsReceiptByOrderIdResponse>> Handle(GetGoodsReceiptByOrderIdRequest request, CancellationToken cancellationToken)
    {
        var receipt = await _goodsReceiptRepository.GetByOrderPublicIdAsync(request.OrderPublicId, _currentUserAccessor.TenantPublicId, cancellationToken);
        if (receipt is null)
        {
            return TResult<GetGoodsReceiptByOrderIdResponse>.Failure(ApplicationErrors.NotFound);
        }

        var items = receipt.Items.Select(i => new GoodsReceiptItemDto(
            i.OrderItem!.Product!.PublicId,
            i.OrderItem.Product.Name,
            i.OrderItem.Product.Sku,
            i.OrderedQuantity,
            i.ReceivedQuantity));

        return TResult<GetGoodsReceiptByOrderIdResponse>.Success(
            new GetGoodsReceiptByOrderIdResponse(receipt.PublicId, receipt.OrderPublicId, receipt.ReceivedAt, receipt.Notes, items));
    }
}
