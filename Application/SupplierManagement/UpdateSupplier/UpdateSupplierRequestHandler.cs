using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.SupplierManagement.UpdateSupplier;

internal sealed class UpdateSupplierRequestHandler(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<UpdateSupplierRequest, TResult<UpdateSupplierResponse>>
{
    private readonly ISupplierRepository _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<TResult<UpdateSupplierResponse>> Handle(UpdateSupplierRequest request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByPublicIdAsync(request.SupplierPublicId, _currentUserAccessor.TenantPublicId, cancellationToken);
        if (supplier == null)
        {
            return TResult<UpdateSupplierResponse>.Failure(ApplicationErrors.NotFound);
        }
        supplier = supplier.WithName(request.Name)
            .WithContactName(request.ContactName)
            .WithContactEmail(request.ContactEmail)
            .WithContactPhone(request.ContactPhone);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var response = new UpdateSupplierResponse(
            supplier.PublicId,
            supplier.Name,
            supplier.ContactName,
            supplier.ContactEmail,
            supplier.ContactPhone);

        return TResult<UpdateSupplierResponse>.Success(response);

    }
}
