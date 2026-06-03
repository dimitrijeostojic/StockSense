using Application.Common.Interfaces;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.SupplierManagement.CreateSupplier;

internal sealed class CreateSupplierRequestHandler(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserAccessor currentUserAccessor
    )
    : IRequestHandler<CreateSupplierRequest, TResult<CreateSupplierResponse>>
{
    private readonly ISupplierRepository _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<TResult<CreateSupplierResponse>> Handle(CreateSupplierRequest request, CancellationToken cancellationToken)
    {
        var supplierEntity = Domain.Entities.Supplier.CreateSupplier(
            request.Name,
            request.ContactName,
            request.ContactEmail,
            request.ContactPhone,
            _currentUserAccessor.TenantPublicId);

        await _supplierRepository.AddAsync(supplierEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return TResult<CreateSupplierResponse>.Success(new CreateSupplierResponse(
            supplierEntity.PublicId,
            supplierEntity.Name,
            supplierEntity.ContactName,
            supplierEntity.ContactEmail,
            supplierEntity.ContactPhone));
    }
}
