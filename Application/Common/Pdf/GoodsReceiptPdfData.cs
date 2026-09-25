namespace Application.Common.Pdf;

public sealed record GoodsReceiptPdfData(
    string OrderPublicId,
    DateTime OrderDate,
    DateTime ReceivedAt,
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
    IReadOnlyCollection<GoodsReceiptPdfItem> Items);
