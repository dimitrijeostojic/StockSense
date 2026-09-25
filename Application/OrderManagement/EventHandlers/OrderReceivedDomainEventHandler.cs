using Domain.Abstractions;
using Domain.Enums;
using Domain.Events;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.OrderManagement.EventHandlers;

internal sealed class OrderReceivedDomainEventHandler(
    IProductRepository productRepository,
    IGoodsReceiptRepository goodsReceiptRepository,
    IUnitOfWork unitOfWork) : INotificationHandler<OrderReceivedDomainEvent>
{
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    private readonly IGoodsReceiptRepository _goodsReceiptRepository = goodsReceiptRepository ?? throw new ArgumentNullException(nameof(goodsReceiptRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task Handle(OrderReceivedDomainEvent notification, CancellationToken cancellationToken)
    {
        var receipt = await _goodsReceiptRepository.GetByOrderPublicIdAsync(notification.OrderPublicId, notification.TenantPublicId, cancellationToken);

        if (receipt is not null)
        {
            var receiptProductIds = receipt.Items.Select(i => i.ProductId).ToList();
            var products = await _productRepository.GetByIdsAsync(receiptProductIds, notification.TenantPublicId, cancellationToken);
            var productMap = products.ToDictionary(p => p.Id);

            foreach (var item in receipt.Items)
            {
                if (item.ReceivedQuantity <= 0) continue;

                if (!productMap.TryGetValue(item.ProductId, out var product))
                    throw new Exception("Product not found");

                product.AddStockEntry(item.ReceivedQuantity, StockEntryType.In, null);
            }
        }
        else
        {
            var productIds = notification.OrderItems.Select(i => i.ProductId).ToList();
            var products = await _productRepository.GetByIdsAsync(productIds, notification.TenantPublicId, cancellationToken);
            var productMap = products.ToDictionary(p => p.Id);

            foreach (var orderItem in notification.OrderItems)
            {
                if (!productMap.TryGetValue(orderItem.ProductId, out var product))
                    throw new Exception("Product not found");

                product.AddStockEntry(orderItem.Quantity, StockEntryType.In, null);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
