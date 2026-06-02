using Application.Common.Errors;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.SupplierManagement.GetSupplierById;

internal sealed class GetSupplierByIdRequestHandler(
    ISupplierRepository supplierRepository)
    : IRequestHandler<GetSupplierByIdRequest, TResult<GetSupplierByIdResponse>>
{
    private readonly ISupplierRepository _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));

    public async Task<TResult<GetSupplierByIdResponse>> Handle(GetSupplierByIdRequest request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByPublicIdAsync(request.SupplierPublicId, cancellationToken);
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
