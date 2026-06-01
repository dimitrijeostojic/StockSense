using Domain.Core;
using MediatR;

namespace Application.Supplier.Delete;

public sealed record DeleteSupplierRequest(Guid PublicId)
    : IRequest<Result>;
