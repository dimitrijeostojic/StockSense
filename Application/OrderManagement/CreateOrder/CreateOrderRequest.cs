using Application.Abstractions.Idempotency;
using Domain.Core;

namespace Application.OrderManagement.CreateOrder;

public sealed record CreateOrderRequest(
    Guid RequestId,
    Guid SupplierPublicId,
    DateTime OrderDate,
    string? Notes,
    ICollection<OrderItemDto> OrderItemsDto) : IdempotentRequest<TResult<CreateOrderResponse>>(RequestId);
