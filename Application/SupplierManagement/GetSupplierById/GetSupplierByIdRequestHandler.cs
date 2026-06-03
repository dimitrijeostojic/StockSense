using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.SupplierManagement.GetSupplierById;

internal sealed class GetSupplierByIdRequestHandler(
    ISupplierRepository supplierRepository,
    ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<GetSupplierByIdRequest, TResult<GetSupplierByIdResponse>>
{
    private readonly ISupplierRepository _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));

    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<TResult<GetSupplierByIdResponse>> Handle(GetSupplierByIdRequest request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByPublicIdAsync(request.SupplierPublicId, _currentUserAccessor.TenantPublicId, cancellationToken);
        if (supplier == null)
        {
            return TResult<GetSupplierByIdResponse>.Failure(ApplicationErrors.NotFound);
        }

        var response = new GetSupplierByIdResponse(
            supplier.PublicId,
            supplier.Name,
            supplier.ContactName,
            supplier.ContactEmail,
            supplier.ContactPhone
        );

        return TResult<GetSupplierByIdResponse>.Success(response);
    }
}
