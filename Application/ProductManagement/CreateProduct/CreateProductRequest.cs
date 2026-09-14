using Domain.Core;
using Domain.Enums;
using MediatR;

namespace Application.ProductManagement.CreateProduct;

public sealed record CreateProductRequest(
    string Name,
    string Sku,
    string? Description,
    decimal Price,
    decimal VatRate,
    int MinimumStockQuantity,
    UnitOfMeasurement UnitOfMeasurement,
    Guid CategoryPublicId,
    Guid SupplierPublicId)
    : IRequest<TResult<CreateProductResponse>>;
