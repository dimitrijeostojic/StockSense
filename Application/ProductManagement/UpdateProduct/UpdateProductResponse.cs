namespace Application.ProductManagement.UpdateProduct;

public sealed record UpdateProductResponse(
    Guid PublicId,
    string Name,
    string? Description,
    decimal Price,
    int MinimumStockQuantity,
    Guid CategoryPublicId,
    string CategoryName,
    Guid SupplierPublicId,
    string SupplierName);