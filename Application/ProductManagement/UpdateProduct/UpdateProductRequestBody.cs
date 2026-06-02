namespace Application.ProductManagement.UpdateProduct;

public sealed record UpdateProductRequestBody(
    string Name,
    string? Description,
    decimal Price,
    int MinimumStockQuantity,
    Guid CategoryId,
    Guid SupplierId);
