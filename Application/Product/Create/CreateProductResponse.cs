namespace Application.Product.Create;

public sealed record CreateProductResponse(
    Guid PublicId,
    string Name,
    string? Description,
    decimal Price,
    int MinimumStockQuantity,
    Guid CategoryPublicId,
    Guid SupplierPublicId);