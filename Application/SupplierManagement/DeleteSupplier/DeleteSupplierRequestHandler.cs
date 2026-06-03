using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.SupplierManagement.DeleteSupplier;

internal sealed class DeleteSupplierRequestHandler(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<DeleteSupplierRequest, Result>
{
    private readonly ISupplierRepository _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<Result> Handle(DeleteSupplierRequest request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByPublicIdAsync(request.PublicId, _currentUserAccessor.TenantPublicId, cancellationToken);
        if (supplier == null)
        {
            return Result.Failure(ApplicationErrors.NotFound);
        }
        _supplierRepository.Delete(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
