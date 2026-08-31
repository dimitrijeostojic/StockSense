using Domain.Core;
using MediatR;

namespace Application.SupplierManagement.GetAllSuppliers;

public sealed class GetAllSuppliersRequest
    : PagedRequest, IRequest<TResult<GetAllSuppliersResponse>>;