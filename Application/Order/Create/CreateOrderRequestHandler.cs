using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.Order.Create;

internal sealed class CreateOrderRequestHandler(
    IOrderRepository orderRepository,
    ISupplierRepository supplierRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateOrderRequest, TResult<CreateOrderResponse>>
{
    private readonly IOrderRepository _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    private readonly ISupplierRepository _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<TResult<CreateOrderResponse>> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByPublicIdAsync(request.SupplierPublicId, cancellationToken);
        if (supplier == null)
        {
            return TResult<CreateOrderResponse>.Failure(ApplicationErrors.NotFound);
        }
        var order = Domain.Entities.Order.CreateOrder(supplier.Id, request.OrderDate, request.Notes);

        foreach (var item in request.OrderItemsDto)
        {
            var product = await _productRepository.GetByPublicIdAsync(item.ProductPublicId, cancellationToken);
            if (product == null)
            {
                return TResult<CreateOrderResponse>.Failure(ApplicationErrors.NotFound);
            }
            order.AddItem(product.Id, item.Quantity, product.Price);
        }
        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return TResult<CreateOrderResponse>.Success(new CreateOrderResponse(order.PublicId, order.OrderStatus, order.OrderDate, order.Notes, supplier.PublicId, supplier.Name));
    }
}
