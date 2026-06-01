namespace Application.Product.GetById;

public sealed record GetProductByIdResponse(
    Guid PublicId,
    string Name,
    string? Description,
    decimal Price,
    int MinimumStockQuantity,
    Guid CategoryPublicId,
    string CategoryName,
    Guid SupplierPublicId,
    string SupplierName);
