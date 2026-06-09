namespace Application.DashboardManagement.Get;

public sealed record DashboardProductDto(
    Guid PublicId,
    string Name,
    decimal Price,
    int MinimumStockQuantity,
    string CategoryName,
    string SupplierName);
