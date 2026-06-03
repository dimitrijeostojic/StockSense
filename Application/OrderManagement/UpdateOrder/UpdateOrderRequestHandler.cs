using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.OrderManagement.UpdateOrder;

internal sealed class UpdateOrderRequestHandler(
    IOrderRepository orderRepository,
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserAccessor currentUserAccessor
    )
    : IRequestHandler<UpdateOrderRequest, TResult<UpdateOrderResponse>>
{
    private readonly IOrderRepository _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    private readonly ISupplierRepository _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<TResult<UpdateOrderResponse>> Handle(UpdateOrderRequest request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByPublicIdAsync(request.PublicId, _currentUserAccessor.TenantPublicId, cancellationToken);
        var supplier = await _supplierRepository.GetByPublicIdAsync(request.SupplierPublicId, _currentUserAccessor.TenantPublicId, cancellationToken);

        if (order is null || supplier is null)
        {
            return TResult<UpdateOrderResponse>.Failure(ApplicationErrors.NotFound);
        }


        order = order.WithOrderDate(request.OrderDate)
                     .WithNotes(request.Notes)
                     .WithSupplierId(supplier.Id);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new UpdateOrderResponse(order.PublicId, order.OrderDate, order.OrderStatus, order.Notes, supplier.PublicId, supplier.Name);
        return TResult<UpdateOrderResponse>.Success(response);
    }
}
