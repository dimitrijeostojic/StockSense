using Domain.Core;
using MediatR;

namespace Application.Supplier.Update;

public sealed record UpdateSupplierRequest(
    Guid SupplierPublicId,
    string Name,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone)
    : IRequest<TResult<UpdateSupplierResponse>>;
