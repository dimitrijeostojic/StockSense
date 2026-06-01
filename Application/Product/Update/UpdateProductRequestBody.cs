namespace Application.Product.Update;

public sealed record UpdateProductRequestBody(
    string Name,
    string? Description,
    decimal Price,
    int MinimumStockQuantity,
    Guid CategoryId,
    Guid SupplierId);
