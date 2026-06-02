using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.SupplierManagement.GetAllSuppliers;

internal sealed class GetAllSuppliersRequestHandler(
    ISupplierRepository supplierRepository)
    : IRequestHandler<GetAllSuppliersRequest, TResult<GetAllSuppliersResponse>>
{
    private readonly ISupplierRepository _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));

    public async Task<TResult<GetAllSuppliersResponse>> Handle(GetAllSuppliersRequest request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _supplierRepository.GetAllAsync(request.SearchTerm, request.SortBy, request.IsAscending, request.PageNumber, request.PageSize, cancellationToken);

        var dtos = items.Select(p => new GetAllSuppliersDto(
            p.PublicId,
            p.Name,
            p.ContactName,
            p.ContactEmail,
            p.ContactPhone));

        var response = new GetAllSuppliersResponse(dtos, totalCount, request.PageNumber, request.PageSize);

        return TResult<GetAllSuppliersResponse>.Success(response);
    }
}
