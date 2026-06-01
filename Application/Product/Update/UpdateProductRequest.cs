using Domain.Core;
using MediatR;

namespace Application.Product.Update;

public sealed record UpdateProductRequest(
    Guid ProductPublicId,
    string Name,
    string? Description,
    decimal Price,
    int MinimumStockQuantity,
    Guid CategoryId,
    Guid SupplierId)
    : IRequest<TResult<UpdateProductResponse>>;
