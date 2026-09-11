using Domain.Enums;

namespace Application.ProductManagement.UpdateProduct;

public sealed record UpdateProductResponse(
    Guid PublicId,
    string Name,
    string Sku,
    string? Description,
    decimal Price,
    int MinimumStockQuantity,
    UnitOfMeasurement UnitOfMeasure,
    Guid CategoryPublicId,
    string CategoryName,
    Guid SupplierPublicId,
    string SupplierName);
