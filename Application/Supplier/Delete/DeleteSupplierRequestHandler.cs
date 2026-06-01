using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.Supplier.Delete;

internal sealed class DeleteSupplierRequestHandler(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteSupplierRequest, Result>
{
    private readonly ISupplierRepository _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<Result> Handle(DeleteSupplierRequest request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByPublicIdAsync(request.PublicId, cancellationToken);
        if (supplier == null)
        {
            return Result.Failure(ApplicationErrors.NotFound);
        }
        _supplierRepository.Delete(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
