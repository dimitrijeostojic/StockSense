using Application.Abstractions.Services;
using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.SupplierManagement.DeleteSupplier;

internal sealed class DeleteSupplierRequestHandler(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserAccessor currentUserAccessor,
    IProductRepository productRepository,
    IOrderRepository orderRepository)
    : IRequestHandler<DeleteSupplierRequest, Result>
{
    private readonly ISupplierRepository _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    private readonly IOrderRepository _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));

    public async Task<Result> Handle(DeleteSupplierRequest request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByPublicIdAsync(request.PublicId, _currentUserAccessor.TenantPublicId, cancellationToken);
        if (supplier == null)
        {
            return Result.Failure(ApplicationErrors.NotFound);
        }
        var hasProducts = await _productRepository.AnyBySupplierIdAsync(supplier.Id, cancellationToken);
        if (hasProducts)
        {
            return Result.Failure(ApplicationErrors.SupplierHasProducts);
        }
        var hasOrders = await _orderRepository.AnyBySupplierIdAsync(supplier.Id, cancellationToken);
        if (hasOrders)
        {
            return Result.Failure(ApplicationErrors.SupplierHasOrders);
        }
        await _supplierRepository.DeleteAsync(supplier, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
