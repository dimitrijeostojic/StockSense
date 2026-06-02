using Domain.Core;
using MediatR;

namespace Application.OrderManagement.UpdateOrder;

public sealed record UpdateOrderRequest(
    Guid PublicId,
    Guid SupplierPublicId,
    DateTime OrderDate,
    string? Notes)
    : IRequest<TResult<UpdateOrderResponse>>;
