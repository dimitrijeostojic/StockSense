using Domain.Core;
using MediatR;

namespace Application.Supplier.GetById;

public sealed record GetSupplierByIdRequest(Guid SupplierPublicId)
    : IRequest<TResult<GetSupplierByIdResponse>>;