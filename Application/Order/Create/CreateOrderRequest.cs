using Domain.Core;
using MediatR;

namespace Application.Order.Create;

public sealed record CreateOrderRequest(
    Guid SupplierPublicId,
    DateTime OrderDate,
    string? Notes,
    ICollection<OrderItemDto> OrderItemsDto)
    : IRequest<TResult<CreateOrderResponse>>;
