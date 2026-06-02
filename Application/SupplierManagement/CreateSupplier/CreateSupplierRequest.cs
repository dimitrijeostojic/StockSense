using Domain.Core;
using MediatR;

namespace Application.SupplierManagement.CreateSupplier;

public sealed record CreateSupplierRequest(
    string Name,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone)
    : IRequest<TResult<CreateSupplierResponse>>;
