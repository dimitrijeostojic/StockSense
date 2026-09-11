namespace Application.ProductManagement.BulkImport;

public sealed class ProductImportRowDto
{
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int MinimumStockQuantity { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string UnitOfMeasurement { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string SupplierCode { get; set; } = string.Empty;
}
