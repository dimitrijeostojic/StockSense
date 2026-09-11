using Domain.Core;
using MediatR;

namespace Application.SupplierManagement.CreateSupplier;

public sealed record CreateSupplierRequest(
    string Name,
    string ContactName,
    string ContactEmail,
    string SupplierCode,
    string? ContactPhone,
    string? Address,
    string? City,
    string? Country)
    : IRequest<TResult<CreateSupplierResponse>>;
