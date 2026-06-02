using Domain.Core;
using MediatR;

namespace Application.OrderManagement.CreateOrder;

public sealed record CreateOrderRequest(
    Guid SupplierPublicId,
    DateTime OrderDate,
    string? Notes,
    ICollection<OrderItemDto> OrderItemsDto)
    : IRequest<TResult<CreateOrderResponse>>;
