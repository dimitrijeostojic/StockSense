using Domain.Core;
using Domain.Enums;
using MediatR;

namespace Application.SupplierManagement.CreateSupplier;

public sealed record CreateSupplierRequest(
    string Name,
    string ContactName,
    string ContactEmail,
    string SupplierCode,
    Currency Currency,
    string? ContactPhone,
    string? Address,
    string? City,
    string? Country)
    : IRequest<TResult<CreateSupplierResponse>>;
