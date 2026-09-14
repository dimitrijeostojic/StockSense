using Domain.Enums;

namespace Application.ProductManagement.UpdateProduct;

public sealed record UpdateProductRequestBody(
    string Name,
    string Sku,
    string? Description,
    decimal Price,
    decimal VatRate,
    int MinimumStockQuantity,
    UnitOfMeasurement UnitOfMeasurement,
    Guid CategoryId,
    Guid SupplierId);
