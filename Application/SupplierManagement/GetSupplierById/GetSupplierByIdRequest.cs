using Domain.Core;
using MediatR;

namespace Application.SupplierManagement.GetSupplierById;

public sealed record GetSupplierByIdRequest(Guid SupplierPublicId)
    : IRequest<TResult<GetSupplierByIdResponse>>;