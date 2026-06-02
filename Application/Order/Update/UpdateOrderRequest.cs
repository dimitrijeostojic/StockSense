using Domain.Core;
using MediatR;

namespace Application.Order.Update;

public sealed record UpdateOrderRequest(
    Guid PublicId,
    Guid SupplierPublicId,
    DateTime OrderDate,
    string? Notes)
    : IRequest<TResult<UpdateOrderResponse>>;
