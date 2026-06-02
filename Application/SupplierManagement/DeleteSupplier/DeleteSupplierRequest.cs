using Domain.Core;
using MediatR;

namespace Application.SupplierManagement.DeleteSupplier;

public sealed record DeleteSupplierRequest(Guid PublicId)
    : IRequest<Result>;
