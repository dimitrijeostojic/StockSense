namespace Application.Common.Pdf;

public sealed record PurchaseOrderPdfData(
    string OrderPublicId,
    DateTime OrderDate,
    string TenantName,
    string TenantPib,
    string? TenantAddress,
    byte[]? TenantLogo,
    string SupplierName,
    string? SupplierAddress,
    string? SupplierCity,
    string? SupplierCountry,
    string? Notes,
    string CurrencyCode,
    IReadOnlyCollection<PurchaseOrderPdfItem> Items);
