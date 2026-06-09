using Application.Common.Interfaces;
using Domain.Abstractions;
using Domain.Events;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.OrderManagement.EventHandlers;

internal sealed class OrderReceivedDomainEventHandler(
    ICurrentUserAccessor currentUserAccessor,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    IMediator mediator) : INotificationHandler<OrderReceivedDomainEvent>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task Handle(OrderReceivedDomainEvent notification, CancellationToken cancellationToken)
    {
        var productIds = notification.OrderItems.Select(i => i.ProductId).ToList();
        var products = await _productRepository.GetByIdsAsync(productIds, _currentUserAccessor.TenantPublicId, cancellationToken);
        var productMap = products.ToDictionary(p => p.Id);

        foreach (var orderItem in notification.OrderItems)
        {
            if (!productMap.TryGetValue(orderItem.ProductId, out var product))
            {
                throw new Exception("Product not found");
            }
            product.AddStockEntry(orderItem.Quantity, Domain.Enums.StockEntryType.In, null);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        foreach (var product in products)
        {
            foreach (var domainEvent in product.DomainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
            product.ClearDomainEvents();
        }
    }
}
