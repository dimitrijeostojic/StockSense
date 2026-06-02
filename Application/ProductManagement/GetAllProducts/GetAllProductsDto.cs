namespace Application.ProductManagement.GetAllProducts;

public sealed record GetAllProductsDto(
    Guid PublicId,
    string Name,
    string? Description,
    decimal Price,
    int MinimumStockQuantity,
    Guid CategoryPublicId,
    string CategoryName,
    Guid SupplierPublicId,
    string SupplierName);