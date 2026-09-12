namespace Application.Common.Pdf;

public sealed record OrderPdfData(
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
    IReadOnlyCollection<OrderPdfItem> Items);
