using Domain.Core;
using Domain.Enums;
using MediatR;

namespace Application.ProductManagement.UpdateProduct;

public sealed record UpdateProductRequest(
    Guid ProductPublicId,
    string Name,
    string Sku,
    string? Description,
    decimal Price,
    int MinimumStockQuantity,
    UnitOfMeasurement UnitOfMeasurement,
    Guid CategoryId,
    Guid SupplierId)
    : IRequest<TResult<UpdateProductResponse>>;
