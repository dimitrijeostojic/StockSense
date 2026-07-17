namespace Application.ProductManagement.GetAllProducts;

public sealed record GetAllProductsDto(
    Guid PublicId,
    string Name,
    string? Description,
    decimal Price,
    int MinimumStockQuantity,
    int ActualStockQuantity,
    Guid CategoryPublicId,
    string CategoryName,
    Guid SupplierPublicId,
    string SupplierName);