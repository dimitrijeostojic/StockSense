using Domain.Core;
using MediatR;

namespace Application.SupplierManagement.UpdateSupplier;

public sealed record UpdateSupplierRequest(
    Guid SupplierPublicId,
    string Name,
    string ContactName,
    string ContactEmail,
    string SupplierCode,
    string? ContactPhone,
    string? Address,
    string? City,
    string? Country)
    : IRequest<TResult<UpdateSupplierResponse>>;
